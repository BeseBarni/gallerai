using System.Data;
using Gallerai.Application.Extensions;
using Gallerai.Application.Interfaces;
using Gallerai.Domain.Enums;
using Gallerai.SharedKernel.Events;
using Gallerai.SharedKernel.Models;
using Gallerai.SharedKernel.Settings;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Gallerai.Application.Features.Images;

public static class ImagesUploaded
{
    public record ImageUploadedR2(string Key, long Size, string Bucket, DateTime Timestamp);
    public record Request(ImageUploadedR2[]? Events);
    public record Command(ImageUploadedR2[] Events) : IRequest<Result<Response>>;
    public record Response(int ProcessedCount, int FailedCount, string[] FailedKeys);

    public sealed class Handler(IGalleraiDbContext context, CloudflareR2Settings cloudflareR2Settings, IPublishEndpoint publishEndpoint, ICacheService cacheService) : IRequestHandler<Command, Result<Response>>
    {
        public async Task<Result<Response>> Handle(Command request, CancellationToken ct)
        {
            var processedCount = 0;
            var ignoredCount = 0;

            var failedKeys = new List<string>();

            if (request.Events == null || request.Events.Length == 0)
                return Result<Response>.Success(new Response(0, 0, Array.Empty<string>()));

            var keys = request.Events.Select(e => e.Key).Distinct().ToArray();

            if (keys.Length == 0) return Result<Response>.Success(new Response(0, 0, Array.Empty<string>()));

            Array.Sort(keys);

            var images = await context.Images
                .Where(x => keys.Contains(x.R2Key))
                .ToDictionaryAsync(i => i.R2Key!, ct);

            foreach (var uploadEvent in request.Events)
            {
                (var flowControl, var counters) = await ProcessImage(processedCount, ignoredCount, failedKeys, images, uploadEvent, ct);
                processedCount = counters.processedCount;
                ignoredCount = counters.ignoredCount;

                if (!flowControl)
                    continue;
            }

            await context.SaveChangesAsync(ct);

            return Result<Response>.Success(new Response(
                processedCount,
                failedKeys.Count,
                failedKeys.ToArray()
            ));

        }

        private async Task<(bool flowControl, (int processedCount, int ignoredCount) value)> ProcessImage(int processedCount, int ignoredCount, List<string> failedKeys, Dictionary<string, Domain.Entities.ImageEntities.Image> images, ImageUploadedR2 uploadEvent, CancellationToken ct)
        {
            if (images.TryGetValue(uploadEvent.Key, out var image))
            {
                var transitioned = await cacheService.TryTransitionStatusAsync(image.GetImageStatusCacheKey(), ImageStatus.UPLOADING, ImageStatus.WAITING_FOR_ANALYSIS);

                if (!transitioned)
                {
                    ignoredCount++;
                    return (flowControl: false, value: (processedCount, ignoredCount));
                }

                await publishEndpoint.Publish(new StartAIInferenceEvent(
                        image.ImageId,
                        image.UserId,
                        image.GetFullPath(cloudflareR2Settings.PublicURL)
                    ), ct);

                await publishEndpoint.Publish(new ImageUploadedEvent(
                    image.ImageId, uploadEvent.Size, uploadEvent.Timestamp
                    ), ct);

                processedCount++;

            }
            else
            {
                failedKeys.Add(uploadEvent.Key);
            }

            return (flowControl: true, value: (processedCount, ignoredCount));
        }
    }
}


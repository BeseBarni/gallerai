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
    public record ImageUploadedEvent(string Key, long Size, string Bucket, DateTime Timestamp);
    public record Request(ImageUploadedEvent[] Events);
    public record Command(ImageUploadedEvent[] Events) : IRequest<Result<Response>>;
    public record Response(int ProcessedCount, int FailedCount, string[] FailedKeys);

    public sealed class Handler(IGalleraiDbContext context, CloudflareR2Settings cloudflareR2Settings, IPublishEndpoint publishEndpoint) : IRequestHandler<Command, Result<Response>>
    {
        public async Task<Result<Response>> Handle(Command request, CancellationToken ct)
        {
            var processedCount = 0;
            var ignoredCount = 0;
            var failedKeys = new List<string>();

            var keys = request.Events.Select(e => e.Key).Distinct().ToArray();

            if (keys.Length == 0) return Result<Response>.Success(new Response(0, 0, Array.Empty<string>()));

            Array.Sort(keys);

            using var transaction = await context.Database.BeginTransactionAsync(ct);

            try
            {
                var images = await context.Images
                    .Include(x => x.Status)
                    .Where(x => keys.Contains(x.R2Key))
                    .ToDictionaryAsync(i => i.R2Key!, ct);

                await context.LockImagesAndStatuses(keys, ct);

                foreach (var uploadEvent in request.Events)
                {
                    (var flowControl, (processedCount, ignoredCount)) = await ProcessImage(processedCount, ignoredCount, failedKeys, images, uploadEvent, ct);

                    if (!flowControl)
                        continue;
                }

                await context.SaveChangesAsync(ct);
                await transaction.CommitAsync(ct);

                return Result<Response>.Success(new Response(
                    processedCount,
                    failedKeys.Count,
                    failedKeys.ToArray()
                ));
            }
            catch
            {
                await transaction.RollbackAsync(ct);
                throw;
            }
        }

        private async Task<(bool flowControl, (int processedCount, int ignoredCount) value)> ProcessImage(int processedCount, int ignoredCount, List<string> failedKeys, Dictionary<string, Domain.Entities.ImageEntities.Image> images, ImageUploadedEvent uploadEvent, CancellationToken ct)
        {
            if (images.TryGetValue(uploadEvent.Key, out var image))
            {
                if (image.Status.Status == ImageStatus.WAITING_FOR_ANALYSIS)
                {
                    ignoredCount++;
                    return (flowControl: false, value: default);
                }

                var imageEvent = image.MarkAsUploaded(uploadEvent.Size, uploadEvent.Timestamp);

                await context.ImageEvents.AddAsync(imageEvent, ct);

                await publishEndpoint.Publish(new StartAIInferenceEvent(
                    image.ImageId,
                    image.GetFullPath(cloudflareR2Settings.PublicURL)
                ), ct);

                processedCount++;
            }
            else
            {
                failedKeys.Add(uploadEvent.Key);
            }

            return (flowControl: true, value: default);
        }
    }
}


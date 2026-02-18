using System.Data;
using System.Diagnostics;
using Gallerai.Application.Extensions;
using Gallerai.Application.Interfaces;
using Gallerai.SharedKernel.Activity;
using Gallerai.SharedKernel.DTOs;
using Gallerai.SharedKernel.Enums;
using Gallerai.SharedKernel.Events;
using Gallerai.SharedKernel.Models;
using Gallerai.SharedKernel.Settings;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Context.Propagation;

namespace Gallerai.Application.Features.Images;

public static class ImagesUploaded
{
    public record ImageUploadedR2(string Key, long Size, string Bucket, string Traceparent, DateTime Timestamp);
    public record Request(ImageUploadedR2[]? Events);
    public record Command(ImageUploadedR2[] Events);
    public record Response(int ProcessedCount, int FailedCount, string[] FailedKeys);

    public sealed class Handler(IGalleraiDbContext context, CloudflareR2Settings cloudflareR2Settings, IPublishEndpoint publishEndpoint, ICacheService cacheService, INotificationService notificationService)
    {
        public async Task<Result<Response>> HandleAsync(Command request, CancellationToken ct)
        {
            using var activity = GalleraiActivity.GalleraiActivitySource.StartActivity("ProcessImageBatch");

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
            List<Task> notificationTasks = new();
            List<Task> publishTasks = new();
            foreach (var uploadEvent in request.Events)
            {
                (var flowControl, var counters) = await ProcessImage(processedCount, ignoredCount, failedKeys, images, uploadEvent, notificationTasks, publishTasks, ct);
                processedCount = counters.processedCount;
                ignoredCount = counters.ignoredCount;

                if (!flowControl)
                    continue;
            }

            await Task.WhenAll(publishTasks);

            await context.SaveChangesAsync(ct);

            await Task.WhenAll(notificationTasks);

            return Result<Response>.Success(new Response(
                processedCount,
                failedKeys.Count,
                failedKeys.ToArray()
            ));

        }

        private async Task<(bool flowControl, (int processedCount, int ignoredCount) value)> ProcessImage(int processedCount, int ignoredCount, List<string> failedKeys, Dictionary<string, Domain.Entities.ImageEntities.Image> images, ImageUploadedR2 uploadEvent, List<Task> notificationTasks, List<Task> publishTasks, CancellationToken ct)
        {
            if (images.TryGetValue(uploadEvent.Key, out var image))
            {
                var transitioned = await cacheService.TryTransitionStatusAsync(image.GetImageStatusCacheKey(), ImageStatus.UPLOADING, ImageStatus.ANALYZING);

                if (!transitioned)
                {
                    ignoredCount++;
                    return (flowControl: false, value: (processedCount, ignoredCount));
                }

                var parentContext = Propagators.DefaultTextMapPropagator.Extract(
                    default,
                    uploadEvent.Traceparent,
                    (carrier, key) => key == "traceparent" ? [carrier] : Array.Empty<string>());

                using (var imageActivity = GalleraiActivity.GalleraiActivitySource.StartActivity("ProcessUploadedImage", ActivityKind.Internal, parentContext.ActivityContext))
                {
                    imageActivity?.SetTag("gallerai.image_id", image.ImageId);
                    imageActivity?.SetTag("gallerai.r2_key", uploadEvent.Key);
                    var notification = new ImageUpdateNotification(ImageStatus.ANALYZING);

                    notification.ImageId = image.ImageId;

                    var notificationTask = notificationService.NotifyUserUpdate(image.UserId, notification);
                    notificationTasks.Add(notificationTask);

                    var startAIInferencePublishTask = publishEndpoint.Publish(new StartAIInferenceEvent(
                            image.ImageId,
                            image.UserId,
                            image.GetFullPath(cloudflareR2Settings.PublicURL)
                        ), ct);

                    var imageUploadedPublishTask = publishEndpoint.Publish(new ImageUploadedEvent(
                        image.ImageId, uploadEvent.Size, uploadEvent.Timestamp
                        ), ct);

                    publishTasks.Add(startAIInferencePublishTask);
                    publishTasks.Add(imageUploadedPublishTask);
                }
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


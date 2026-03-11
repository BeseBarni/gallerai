using System.Data;
using System.Diagnostics;
using Gallerai.Application.Extensions;
using Gallerai.Application.Helpers;
using Gallerai.Application.Interfaces;
using Gallerai.Domain.Entities.ImageEntities;
using Gallerai.SharedKernel.Activity;
using Gallerai.SharedKernel.DTOs;
using Gallerai.SharedKernel.Enums;
using Gallerai.SharedKernel.Events;
using Gallerai.SharedKernel.Models;
using Gallerai.SharedKernel.Settings;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Gallerai.Application.Features.Images;

public static class ImagesUploaded
{
    public record ImageUploadedR2(string Key, long Size, string Bucket, string Traceparent, DateTime Timestamp);
    public record Request(ImageUploadedR2[]? Events);
    public record Command(ImageUploadedR2[] Events);
    public record Response(int ProcessedCount, int FailedCount, string[] FailedKeys);
    internal readonly record struct ProcessingTasks(
        ValueTask NotificationTask,
        Task StartAIInferencePublishTask,
        Task ImageUploadedPublishTask,
        bool IsSkipped = false,
        string? FailedKey = null)
    {
        internal static ProcessingTasks Skipped(string? key = null) => new(
            ValueTask.CompletedTask,
            Task.CompletedTask,
            Task.CompletedTask,
            IsSkipped: true,
            FailedKey: key);
    }
    public sealed class Handler(IGalleraiDbContext context, CloudflareR2Settings cloudflareR2Settings, IPublishEndpoint publishEndpoint, ICacheService cacheService, INotificationService notificationService)
    {
        public async Task<Result<Response>> HandleAsync(Command request, CancellationToken ct)
        {
            using var activity = GalleraiActivity.GalleraiActivitySource.StartActivity("ProcessImageBatch");

            if (request.Events == null || request.Events.Length == 0)
                return Result<Response>.Success(new Response(0, 0, []));

            var events = request.Events;

            var keys = events.Select(e => e.Key).Distinct().ToArray();

            if (keys.Length == 0) return Result<Response>.Success(new Response(0, 0, []));

            Array.Sort(keys);

            var images = await context.Images
                .AsNoTracking()
                .Where(x => keys.Contains(x.R2Key))
                .Select(i => new { i.ImageId, i.UserId, i.R2Key })
                .ToDictionaryAsync(i => i.R2Key!, i => new Image()
                {
                    R2Key = i.R2Key,
                    ImageId = i.ImageId,
                    UserId = i.UserId,
                }, ct);

            var imageCacheKeys = events
                .Where(e => images.ContainsKey(e.Key))
                .Select(e => images[e.Key].GetImageStatusCacheKey())
                .ToArray();

            var imageCacheTransitionResults = await cacheService.TryTransitionStatusBatchAsync(imageCacheKeys, ImageStatus.UPLOADING, ImageStatus.ANALYZING);

            var transitionMap = imageCacheKeys
                .Zip(imageCacheTransitionResults, (key, success) => new { key, success })
                .ToDictionary(x => x.key, x => x.success);

            var processingTasks = events.Select(async (uploadEvent, index) =>
            {
                if (!images.TryGetValue(uploadEvent.Key, out var image))
                    return ProcessingTasks.Skipped(uploadEvent.Key);

                var cacheKey = image.GetImageStatusCacheKey();

                if (!transitionMap.TryGetValue(cacheKey, out var transitioned) || !transitioned)
                    return ProcessingTasks.Skipped();

                var parentContext = TelemetryHelpers.GetParentContext(uploadEvent.Traceparent);

                using var imageActivity = GalleraiActivity.GalleraiActivitySource.StartActivity("ProcessUploadedImage", ActivityKind.Internal, parentContext.ActivityContext);

                imageActivity?.SetTag("gallerai.image_id", image.ImageId);
                imageActivity?.SetTag("gallerai.r2_key", uploadEvent.Key);

                var notificationTask = notificationService.NotifyUserUpdate(image.UserId, new ImageUpdateNotification(ImageStatus.ANALYZING) { ImageId = image.ImageId });

                var startAIInferencePublishTask = publishEndpoint.Publish(new StartAIInferenceEvent(
                        image.ImageId,
                        image.UserId,
                        image.GetFullPath(cloudflareR2Settings.PublicURL)
                    ), ct);

                var imageUploadedPublishTask = publishEndpoint.Publish(new ImageUploadedEvent(
                    image.ImageId, uploadEvent.Size, uploadEvent.Timestamp
                    ), ct);

                return new ProcessingTasks(notificationTask, startAIInferencePublishTask, imageUploadedPublishTask);

            });

            var results = (await Task.WhenAll(processingTasks)).ToList();
            var notSkippedResults = results.Where(r => !r.IsSkipped).ToList();
            var publishTasks = notSkippedResults.SelectMany(r => new[] { r.ImageUploadedPublishTask, r.StartAIInferencePublishTask });

            var notificationTasks = notSkippedResults.Select(r => r.NotificationTask.AsTask());

            var failedKeys = results.Select(r => r.FailedKey).OfType<string>().ToArray();

            await Task.WhenAll(publishTasks);

            await context.SaveChangesAsync(ct);

            await Task.WhenAll(notificationTasks);

            return Result<Response>.Success(new Response(
                notSkippedResults.Count,
                failedKeys.Length,
                failedKeys
            ));

        }
    }
}


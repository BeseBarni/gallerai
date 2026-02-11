using Gallerai.Application.Interfaces;
using Gallerai.Domain.Entities.ImageEntities;
using Gallerai.Domain.Enums;
using Gallerai.SharedKernel.Events;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Gallerai.Application.Features.Images.Consumers;

public class AIInferenceFinishedEventConsumer(IGalleraiDbContext dbContext, INotificationService notificationService) : IConsumer<AIInferenceFinishedEvent>
{
    public async Task Consume(ConsumeContext<AIInferenceFinishedEvent> context)
    {
        var message = context.Message;
        var ct = context.CancellationToken;

        if (message is null) return;

        var image = await dbContext.Images
            .Include(p => p.Status)
            .Include(p => p.Analysis)
            .FirstOrDefaultAsync(p => p.ImageId == message.imageId, ct);

        if (image is null) return;

        // Idempotency check
        if (image.Status.Status is ImageStatus.READY or ImageStatus.ERROR)
            return;

        if (!message.isSuccess)
        {
            await dbContext.TryAddEventAsync(image.MarkAsError(), ct);
            return;
        }

        var analysis = new ImageAnalysis(message.result!.Score, message.result.Critique);
        var imageEvent = image.MarkAsAnalyzed(analysis);

        if (await dbContext.TryAddEventAsync(imageEvent, ct))
        {
            await notificationService.NotifyUserUpdate("", message.result);
        }
    }


}


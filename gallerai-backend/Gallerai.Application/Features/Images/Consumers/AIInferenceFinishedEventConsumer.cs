using Gallerai.Application.Interfaces;
using Gallerai.Domain.Entities.ImageEntities;
using Gallerai.SharedKernel.Events;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Gallerai.Application.Features.Images.Consumers;

public class AIInferenceFinishedEventConsumer(IGalleraiDbContext dbContext, INotificationService notificationService) : IConsumer<AIInferenceFinishedEvent>
{
    public async Task Consume(ConsumeContext<AIInferenceFinishedEvent> context)
    {
        var message = context.Message;

        if (message is null) return;

        if (!message.isSuccess)
        {

        }

        var image = await dbContext.Images
            .Include(p => p.Status)
            .Include(p => p.Analysis)
            .FirstOrDefaultAsync(p => p.ImageId == message.imageId);

        if (image is null) return;

        var analysis = new ImageAnalysis(message.result!.Score, message.result.Critique);

        //image.MarkAsAnalyzed(analysis);

        await dbContext.SaveChangesAsync();

        await notificationService.NotifyUserUpdate("", message.result);
    }
}

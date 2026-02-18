using Gallerai.Application.Interfaces;
using Gallerai.Domain.Entities.ImageEntities;
using Gallerai.SharedKernel.Enums;
using Gallerai.SharedKernel.Events;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Gallerai.Application.Features.Images.Consumers;

public class AIInferenceFinishedEventConsumer(IGalleraiDbContext dbContext) : IConsumer<AIInferenceFinishedEvent>
{
    public async Task Consume(ConsumeContext<AIInferenceFinishedEvent> context)
    {
        var message = context.Message;
        var ct = context.CancellationToken;

        if (message is null) return;

        var image = await dbContext.Images
            .Include(p => p.ImageEvents)
            .Include(p => p.Analysis)
            .FirstOrDefaultAsync(p => p.ImageId == message.ImageId, ct);

        if (image is null) return;

        var status = image.ImageEvents.OrderBy(p => p.LastUpdate).FirstOrDefault();
        if (status is null) return;

        if (status.Status is ImageStatus.READY or ImageStatus.ANALYSIS_ERROR)
            return;

        if (message.Result is null)
        {
            return;
        }

        if (!message.IsSuccess || message.Result.Status == ImageStatus.ANALYSIS_ERROR || message.Result.Score is null || message.Result.Critique is null)
        {
            await dbContext.TryAddEventAsync(image.MarkAsError(), ct);
            return;
        }

        var analysis = new ImageAnalysis((double)message.Result.Score, message.Result.Critique);
        var imageEvent = image.MarkAsAnalyzed(analysis);

        await dbContext.TryAddEventAsync(imageEvent, ct);
    }


}


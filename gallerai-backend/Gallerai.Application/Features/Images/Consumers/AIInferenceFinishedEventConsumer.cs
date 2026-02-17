using Gallerai.Application.Interfaces;
using Gallerai.Domain.Entities.ImageEntities;
using Gallerai.Domain.Enums;
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
            .Include(p => p.Status)
            .Include(p => p.Analysis)
            .FirstOrDefaultAsync(p => p.ImageId == message.ImageId, ct);

        if (image is null) return;

        if (image.Status.Status is ImageStatus.READY or ImageStatus.ERROR)
            return;

        if (!message.IsSuccess)
        {
            await dbContext.TryAddEventAsync(image.MarkAsError(), ct);
            return;
        }

        var analysis = new ImageAnalysis(message.Result!.Score, message.Result.Critique);
        var imageEvent = image.MarkAsAnalyzed(analysis);

        await dbContext.TryAddEventAsync(imageEvent, ct);
    }


}


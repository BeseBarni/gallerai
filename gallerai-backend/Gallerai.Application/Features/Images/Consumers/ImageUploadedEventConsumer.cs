using Gallerai.Application.Interfaces;
using Gallerai.SharedKernel.Events;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Gallerai.Application.Features.Images.Consumers;

public class ImageUploadedEventConsumer(IGalleraiDbContext dbContext) : IConsumer<ImageUploadedEvent>
{
    public async Task Consume(ConsumeContext<ImageUploadedEvent> context)
    {
        var image = await dbContext.Images.FirstOrDefaultAsync(i => i.ImageId == context.Message.ImageId);

        if (image is null)
        {
            return;
        }

        var imageEvent = image.MarkAsUploaded(context.Message.Size, context.Message.TimeStamp);

        await dbContext.TryAddEventAsync(imageEvent);
    }
}

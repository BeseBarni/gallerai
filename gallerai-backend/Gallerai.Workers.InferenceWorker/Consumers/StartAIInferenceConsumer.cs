using Gallerai.SharedKernel.Events;
using Gallerai.Workers.InferenceWorker.Persistance;
using Gallerai.Workers.InferenceWorker.Services;
using MassTransit;

namespace Gallerai.Workers.InferenceWorker.Consumers;

public sealed class StartAIInferenceConsumer(
    ILogger<StartAIInferenceConsumer> logger,
    IInferenceService inferenceService,
    IPublishEndpoint publishEndpoint,
    WorkerDbContext dbContext) : IConsumer<StartAIInferenceEvent>
{
    public async Task Consume(ConsumeContext<StartAIInferenceEvent> context)
    {
        var message = context.Message;

        logger.LogInformation("🤖 Starting AI inference for image: {Id} | URL: {Url}",
            message.Id,
            message.PublicUrl);

        var result = await inferenceService.AnalyzeImageAsync(
            message.PublicUrl,
            context.CancellationToken);

        AIInferenceFinishedEvent publishEvent;

        if (result is null)
        {
            logger.LogWarning("⚠️ AI inference failed for image: {Id} | URL: {Url}",
                message.Id,
                message.PublicUrl);

            publishEvent = new AIInferenceFinishedEvent(message.Id, message.UserId, false, null);
        }
        else
        {
            result.ImageId = message.Id;
            publishEvent = new AIInferenceFinishedEvent(message.Id, message.UserId, true, result);
        }


        await publishEndpoint.Publish(publishEvent, context.CancellationToken);
        await dbContext.SaveChangesAsync(context.CancellationToken);

        logger.LogInformation("✅ AI inference completed for image: {Id} | Result: {Result}",
            message.Id,
            result);
    }
}

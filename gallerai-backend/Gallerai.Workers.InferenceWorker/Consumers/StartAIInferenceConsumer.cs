using Gallerai.SharedKernel.Events;
using Gallerai.Workers.InferenceWorker.Services;
using MassTransit;

namespace Gallerai.Workers.InferenceWorker.Consumers;

public sealed class StartAIInferenceConsumer(
    ILogger<StartAIInferenceConsumer> logger,
    IInferenceService inferenceService) : IConsumer<StartAIInferenceEvent>
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

        logger.LogInformation("✅ AI inference completed for image: {Id} | Result: {Result}",
            message.Id,
            result);
    }
}

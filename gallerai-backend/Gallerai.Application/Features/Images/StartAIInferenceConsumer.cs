using Gallerai.SharedKernel.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Gallerai.Application.Features.Images;

public class StartAIInferenceConsumer : IConsumer<StartAIInferenceEvent>
{
    private readonly ILogger<StartAIInferenceConsumer> _logger;

    public StartAIInferenceConsumer(ILogger<StartAIInferenceConsumer> logger)
    {
        _logger = logger;
    }

    public Task Consume(ConsumeContext<StartAIInferenceEvent> context)
    {
        _logger.LogInformation("🤖 Received Image for AI Processing: {Id} | URL: {Url}",
            context.Message.Id,
            context.Message.publicUrl);

        return Task.CompletedTask;
    }
}

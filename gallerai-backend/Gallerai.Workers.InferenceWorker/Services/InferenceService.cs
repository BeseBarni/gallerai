using System.ClientModel;
using Gallerai.SharedKernel.Settings;
using OpenAI;
using OpenAI.Chat;

namespace Gallerai.Workers.InferenceWorker.Services;

public interface IInferenceService
{
    Task<string> AnalyzeImageAsync(string imageUrl, CancellationToken cancellationToken = default);
}

public sealed class InferenceService : IInferenceService
{
    private readonly ChatClient _chatClient;

    public InferenceService(InferenceClientSettings config)
    {
        var openAiClient = new OpenAIClient(
            new ApiKeyCredential(config.ApiKey),
            new OpenAIClientOptions { Endpoint = new Uri(config.Endpoint) });

        _chatClient = openAiClient.GetChatClient(config.ModelId);
    }

    public async Task<string> AnalyzeImageAsync(string imageUrl, CancellationToken cancellationToken = default)
    {
        var imagePart = ChatMessageContentPart.CreateImagePart(new Uri(imageUrl));
        var textPart = ChatMessageContentPart.CreateTextPart("Describe this image and provide 5 tags.");

        ChatCompletion completion = await _chatClient.CompleteChatAsync(
            [new UserChatMessage(textPart, imagePart)],
            cancellationToken: cancellationToken);

        return completion.Content[0].Text;
    }
}

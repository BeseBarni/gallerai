using System.ClientModel;
using System.Text.Json;
using Gallerai.SharedKernel.Consts;
using Gallerai.SharedKernel.DTOs;
using Gallerai.SharedKernel.Enums;
using Gallerai.SharedKernel.Settings;
using OpenAI;
using OpenAI.Chat;

namespace Gallerai.Workers.InferenceWorker.Services;

public interface IInferenceService
{
    Task<ImageUpdateNotification> AnalyzeImageAsync(string imageUrl, CancellationToken cancellationToken = default);
}

public sealed class InferenceService : IInferenceService
{
    private readonly ChatClient _chatClient;
    private readonly ILogger<InferenceService> _logger;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public InferenceService(InferenceClientSettings config, ILogger<InferenceService> logger)
    {
        _logger = logger;
        var openAiClient = new OpenAIClient(
            new ApiKeyCredential(config.ApiKey),
            new OpenAIClientOptions { Endpoint = new Uri(config.Endpoint) });

        _chatClient = openAiClient.GetChatClient(config.ModelId);
    }

    public async Task<ImageUpdateNotification> AnalyzeImageAsync(string imageUrl, CancellationToken cancellationToken = default)
    {
        try
        {
            var imagePart = ChatMessageContentPart.CreateImagePart(new Uri(imageUrl));
            var textPart = ChatMessageContentPart.CreateTextPart(ChatConsts.UserPrompt);

            ChatCompletion completion = await _chatClient.CompleteChatAsync(
                [new UserChatMessage(textPart, imagePart)],
                cancellationToken: cancellationToken);

            var jsonResponse = completion.Content[0].Text;

            jsonResponse = CleanJsonResponse(jsonResponse);

            var result = JsonSerializer.Deserialize<ImageUpdateNotification>(jsonResponse, JsonOptions);

            if (result is null)
            {
                _logger.LogError("Deserialization returned null for response: {Response}", jsonResponse);
                return new ImageUpdateNotification(ImageStatus.ANALYSIS_ERROR);
            }

            result.Status = ImageStatus.READY;

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to analyze image: {Url}", imageUrl);
            return new ImageUpdateNotification(ImageStatus.ANALYSIS_ERROR);
        }
    }

    private string CleanJsonResponse(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return "{}";

        // Remove markdown code blocks if present
        if (input.Contains("```json"))
            input = input.Split("```json")[1].Split("```")[0];
        else if (input.Contains("```"))
            input = input.Split("```")[1];

        // Remove python-style comments (# ...) that your training process left behind
        // This regex looks for # and removes everything until the end of the line
        input = System.Text.RegularExpressions.Regex.Replace(input, @"#.*$", "", System.Text.RegularExpressions.RegexOptions.Multiline);

        return input.Trim();
    }
}

public sealed class FakeInferenceService : IInferenceService
{
    public async Task<ImageUpdateNotification> AnalyzeImageAsync(string imageUrl, CancellationToken cancellationToken = default)
    {
        await Task.Delay(2000, cancellationToken); // Simulate some processing delay
        return await Task.FromResult(new ImageUpdateNotification(1, "critique", ImageStatus.READY));
    }
}


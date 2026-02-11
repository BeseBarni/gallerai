using System.ClientModel;
using System.Text.Json;
using Gallerai.SharedKernel.Consts;
using Gallerai.SharedKernel.DTOs;
using Gallerai.SharedKernel.Settings;
using OpenAI;
using OpenAI.Chat;

namespace Gallerai.Workers.InferenceWorker.Services;

public interface IInferenceService
{
    Task<AIInferenceResult> AnalyzeImageAsync(string imageUrl, CancellationToken cancellationToken = default);
}

public sealed class InferenceService : IInferenceService
{
    private readonly ChatClient _chatClient;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public InferenceService(InferenceClientSettings config)
    {
        var openAiClient = new OpenAIClient(
            new ApiKeyCredential(config.ApiKey),
            new OpenAIClientOptions { Endpoint = new Uri(config.Endpoint) });

        _chatClient = openAiClient.GetChatClient(config.ModelId);
    }

    public async Task<AIInferenceResult> AnalyzeImageAsync(string imageUrl, CancellationToken cancellationToken = default)
    {
        var imagePart = ChatMessageContentPart.CreateImagePart(new Uri(imageUrl));
        var textPart = ChatMessageContentPart.CreateTextPart(ChatConsts.UserPrompt);

        ChatCompletion completion = await _chatClient.CompleteChatAsync(
            [new UserChatMessage(textPart, imagePart)],
            cancellationToken: cancellationToken);

        var jsonResponse = completion.Content[0].Text;

        jsonResponse = CleanJsonResponse(jsonResponse);

        try
        {
            return JsonSerializer.Deserialize<AIInferenceResult>(jsonResponse, JsonOptions)
                ?? throw new InvalidOperationException("Deserialization returned null");
        }
        catch (JsonException ex)
        {
            // Log the raw response here so you can see exactly what failed
            throw new Exception($"Failed to parse AI response: {jsonResponse}", ex);
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
    public Task<AIInferenceResult> AnalyzeImageAsync(string imageUrl, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new AIInferenceResult(1, "critique"));
    }
}


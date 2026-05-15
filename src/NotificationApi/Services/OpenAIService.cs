using Microsoft.Extensions.Configuration;
using NotificationApi.Dtos;
using System.Text.Json;
using NotificationApi.Interfaces;
//namespace NotificationApi.Services;
using Microsoft.Extensions.Options;
using OpenAI;
using OpenAI.Chat;
using System.Net.Http.Headers;
using NotificationApi.Options;
using System.Text;

namespace NotificationApi.Services;

public class OpenAIService : ILLMService

{

    private readonly HttpClient _httpClient;

    private readonly IConfiguration _config;

    public OpenAIService(HttpClient httpClient, IConfiguration config)

    {

        _httpClient = httpClient;

        _config = config;

    }

    public async Task<LLMResponse> GenerateMessageAsync(NotificationRequest request)
    {
        var apiKey = _config["OpenAI:ApiKey"];

        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", apiKey);

        var prompt = $$"""
You are a log classification system.

Return ONLY valid JSON.

Do NOT use markdown, code blocks, or explanation.

Format:
{
  "severity": "INFO | WARNING | ERROR | CRITICAL",
  "message": "short professional message"
}

Input:
Source: {{request.Source}}
Level: {{request.Level}}
Message: {{request.Message}}
""";

        var requestBody = new
        {
            model = "gpt-4o-mini",
            messages = new[]
            {
            new
            {
                role = "user",
                content = prompt
            }
        }
        };

        var response = await _httpClient.PostAsync(
            "https://api.openai.com/v1/chat/completions",
            new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json")
        );

        var responseString = await response.Content.ReadAsStringAsync();

        Console.WriteLine("OPENAI RAW RESPONSE:");
        Console.WriteLine(responseString);

        //using var doc = JsonDocument.Parse(responseString);

        using var doc = JsonDocument.Parse(responseString);

        // 🚨 1. Handle OpenAI error safely
        if (doc.RootElement.TryGetProperty("error", out var error))
        {
            var errorMessage = error.GetProperty("message").GetString();
            throw new Exception($"OpenAI Error: {errorMessage}");
        }

        // 🚨 2. Safe extraction of response
        if (!doc.RootElement.TryGetProperty("choices", out var choices) ||
            choices.GetArrayLength() == 0)
        {
            throw new Exception("Invalid OpenAI response: missing choices");
        }

        var content = doc.RootElement
      .GetProperty("choices")[0]
      .GetProperty("message")
      .GetProperty("content")
      .GetString();

        if (string.IsNullOrWhiteSpace(content))
            throw new Exception("Empty LLM response");

        // 🚨 REMOVE markdown code fences
        content = content
            .Replace("```json", "")
            .Replace("```", "")
            .Trim();

        var result = JsonSerializer.Deserialize<LLMResponse>(
    content!,
    new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true
    }
);
        Console.WriteLine("RAW LLM: " + content);
        Console.WriteLine("DESERIALIZED: " + JsonSerializer.Serialize(result));
        if (result == null)
        {
            throw new Exception("LLM returned null response: " + content);
        }

        return result;
    }

}
// public class OpenAIService : ILLMService
// {
//     private readonly HttpClient _httpClient;
//     private readonly OpenAIOptions _options;

//     public OpenAIService(
//         HttpClient httpClient,
//         IOptions<OpenAIOptions> options)
//     {
//         _httpClient = httpClient;
//         _options = options.Value;
//     }

//     public async Task<string> GenerateMessageAsync(
//         NotificationRequest request)
//     {
//         _httpClient.DefaultRequestHeaders.Authorization =
//             new System.Net.Http.Headers.AuthenticationHeaderValue(
//                 "Bearer",
//                 _options.ApiKey);

//         var prompt = $"""
//             Analyze this system notification and generate a short professional alert message.

//             Source: {request.Source}
//             Level: {request.Level}
//             Message: {request.Message}
//             Timestamp: {request.Timestamp}
//             """;

//         var requestBody = new
//         {
//             model = "gpt-4.1-mini",
//             messages = new[]
//             {
//                 new
//                 {
//                     role = "user",
//                     content = prompt
//                 }
//             }
//         };

//         var json = JsonSerializer.Serialize(requestBody);

//         var response = await _httpClient.PostAsync(
//             "https://api.openai.com/v1/chat/completions",
//             new StringContent(
//                 json,
//                 Encoding.UTF8,
//                 "application/json"));

//         response.EnsureSuccessStatusCode();

//         var responseContent =
//             await response.Content.ReadAsStringAsync();

//         using var document =
//             JsonDocument.Parse(responseContent);

//         return document
//             .RootElement
//             .GetProperty("choices")[0]
//             .GetProperty("message")
//             .GetProperty("content")
//             .GetString() ?? "Warning detected";
//     }
// }
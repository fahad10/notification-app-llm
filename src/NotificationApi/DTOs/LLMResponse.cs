using System.Text.Json.Serialization;
namespace NotificationApi.Dtos;

public class LLMResponse
{
    [JsonPropertyName("severity")]

    public string Severity { get; set; }

    [JsonPropertyName("message")]

    public string Message { get; set; }
}
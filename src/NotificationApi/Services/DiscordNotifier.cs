using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using NotificationApi.Interfaces;
using NotificationApi.Options;

namespace NotificationApi.Services;

public class DiscordNotifier : IDiscordNotifier
{
    private readonly HttpClient _httpClient;
    private readonly DiscordOptions _options;

    public DiscordNotifier(
        HttpClient httpClient,
        IOptions<DiscordOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
    }

    public async Task SendMessageAsync(string message)
    {
        var payload = new
        {
            content = message
        };

        var json = JsonSerializer.Serialize(payload);

        var response = await _httpClient.PostAsync(
            _options.WebhookUrl,
            new StringContent(json, Encoding.UTF8, "application/json"));

        response.EnsureSuccessStatusCode();
    }
}
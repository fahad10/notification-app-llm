using NotificationApi.Dtos;
using NotificationApi.Enums;
using NotificationApi.Interfaces;

namespace NotificationApi.Services;

public class NotificationProcessor : INotificationProcessor
{
    private readonly ILLMService _llmService;
    private readonly IDiscordNotifier _discordNotifier;
    private readonly IRateLimiter _rateLimiter;
    private readonly ILogger<NotificationProcessor> _logger;

    public NotificationProcessor(
        ILLMService llmService,
        IDiscordNotifier discordNotifier,
        IRateLimiter rateLimiter,
        ILogger<NotificationProcessor> logger)
    {
        _llmService = llmService;
        _discordNotifier = discordNotifier;
        _rateLimiter = rateLimiter;
        _logger = logger;
    }

    public async Task ProcessAsync(NotificationRequest request)
    {
        if (request.Level < NotificationLevel.Warning)
        {
            _logger.LogInformation("Info notification ignored");
            return;
        }

        if (!_rateLimiter.TryConsume())
        {
            throw new InvalidOperationException("Rate limit exceeded");
        }

        var llmResult = await _llmService.GenerateMessageAsync(request);
        var severity = llmResult.Severity?.ToLowerInvariant();
        string formattedMessage = severity switch
        {
            "critical" => $"🚨 CRITICAL: {llmResult.Message}",
            "error" => $"❌ ERROR: {llmResult.Message}",
            "warning" => $"⚠️ WARNING: {llmResult.Message}",
            _ => $"ℹ️ INFO: {llmResult.Message}"
        };

        await _discordNotifier.SendMessageAsync(formattedMessage);

        _logger.LogInformation(
            "Forwarded to Discord with severity: {Severity}",
            llmResult.Severity
        );
    }
}
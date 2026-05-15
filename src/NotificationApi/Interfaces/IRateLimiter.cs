namespace NotificationApi.Interfaces;

public interface IRateLimiter
{
    bool TryConsume();
}
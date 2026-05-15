using NotificationApi.Interfaces;

namespace NotificationApi.RateLimiting;

public class InMemoryRateLimiter : IRateLimiter
{
    private readonly Queue<DateTime> _timestamps = new();

    private readonly object _lock = new();

    private const int LIMIT = 10;

    public bool TryConsume()
    {
        lock (_lock)
        {
            var now = DateTime.UtcNow;

            while (_timestamps.Count > 0 &&
                   (now - _timestamps.Peek()).TotalMinutes >= 1)
            {
                _timestamps.Dequeue();
            }

            if (_timestamps.Count >= LIMIT)
            {
                return false;
            }

            _timestamps.Enqueue(now);

            return true;
        }
    }
}
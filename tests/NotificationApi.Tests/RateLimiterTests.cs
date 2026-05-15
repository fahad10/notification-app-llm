using FluentAssertions;
using NotificationApi.RateLimiting;
namespace NotificationApi.Tests;

public class RateLimiterTests
{
    [Fact]
    public void Should_Allow_Only_10_Requests_Per_Minute()
    {
        
        var limiter = new InMemoryRateLimiter();

        
        for (int i = 0; i < 10; i++)
        {
            limiter.TryConsume().Should().BeTrue();
        }

        var result = limiter.TryConsume();

        
        result.Should().BeFalse();
    }
}
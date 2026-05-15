using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NotificationApi.Dtos;
using NotificationApi.Enums;
using NotificationApi.Interfaces;
using NotificationApi.Services;

namespace NotificationApi.Tests;

public class NotificationProcessorTests
{
    private readonly Mock<ILLMService> _llmMock = new();
    private readonly Mock<IDiscordNotifier> _discordMock = new();
    private readonly Mock<IRateLimiter> _rateLimiterMock = new();
    private readonly Mock<ILogger<NotificationProcessor>> _loggerMock = new();

    [Fact]
    public async Task Should_Ignore_Info_Level()
    {
        // Arrange
        var processor = new NotificationProcessor(
            _llmMock.Object,
            _discordMock.Object,
            _rateLimiterMock.Object,
            _loggerMock.Object);

        var request = new NotificationRequest
        {
            Source = "test-service",
            Level = NotificationLevel.Info,
            Message = "Everything OK",
            Timestamp = DateTime.UtcNow
        };

        // Act
        await processor.ProcessAsync(request);

        // Assert
        _discordMock.Verify(
            x => x.SendMessageAsync(It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task Should_Send_Discord_Message_For_Warning()
    {
        // Arrange
        _rateLimiterMock.Setup(x => x.TryConsume())
            .Returns(true);

        _llmMock.Setup(x => x.GenerateMessageAsync(It.IsAny<NotificationRequest>()))
            .ReturnsAsync(new LLMResponse
            {
                Severity = "WARNING",
                Message = "Database latency detected"
            });

        var processor = new NotificationProcessor(
            _llmMock.Object,
            _discordMock.Object,
            _rateLimiterMock.Object,
            _loggerMock.Object);

        var request = new NotificationRequest
        {
            Source = "payment-service",
            Level = NotificationLevel.Warning,
            Message = "Latency issue",
            Timestamp = DateTime.UtcNow
        };

        // Act
        await processor.ProcessAsync(request);

        // Assert
        _discordMock.Verify(
            x => x.SendMessageAsync(It.IsAny<string>()),
            Times.Once);
    }

    [Fact]
    public async Task Should_Throw_When_Rate_Limit_Exceeded()
    {
        
        _rateLimiterMock.Setup(x => x.TryConsume())
            .Returns(false);

        var processor = new NotificationProcessor(
            _llmMock.Object,
            _discordMock.Object,
            _rateLimiterMock.Object,
            _loggerMock.Object);

        var request = new NotificationRequest
        {
            Source = "payment-service",
            Level = NotificationLevel.Warning,
            Message = "Too many requests",
            Timestamp = DateTime.UtcNow
        };

        
        Func<Task> act = async () => await processor.ProcessAsync(request);

        
        await act.Should()
            .ThrowAsync<InvalidOperationException>();
    }
}
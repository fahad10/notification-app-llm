using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;

namespace NotificationApi.Tests;

public class NotificationApiIntegrationTests :
    IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public NotificationApiIntegrationTests(
        WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Post_Notification_Returns_Accepted()
    {
        
        var payload = new
        {
            source = "payment-service",
            level = 1,
            message = "Payment latency detected",
            timestamp = DateTime.UtcNow
        };

        
        var response = await _client.PostAsJsonAsync(
            "/notifications",
            payload);

        
        response.StatusCode.Should()
            .Be(HttpStatusCode.Accepted);
    }
}
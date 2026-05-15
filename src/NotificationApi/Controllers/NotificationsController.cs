using Microsoft.AspNetCore.Mvc;
using NotificationApi.Dtos;
using NotificationApi.Interfaces;

namespace NotificationApi.Controllers;

[ApiController]
[Route("notifications")]
public class NotificationsController : ControllerBase
{
    private readonly INotificationProcessor _processor;

    public NotificationsController(
        INotificationProcessor processor)
    {
        _processor = processor;
    }

    [HttpPost]
    public async Task<IActionResult> Post(
        NotificationRequest request)
    {
        try
        {
            await _processor.ProcessAsync(request);

            return Accepted();
        }
        catch (InvalidOperationException ex)
        {
            return StatusCode(429, ex.Message);
        }
    }
}
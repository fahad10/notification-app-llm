using NotificationApi.Enums;
namespace NotificationApi.Dtos;

public class NotificationRequest

{

    public string Source { get; set; } = string.Empty;

    public NotificationLevel Level { get; set; }

    public string Message { get; set; } = string.Empty;

    public DateTime Timestamp { get; set; }

}
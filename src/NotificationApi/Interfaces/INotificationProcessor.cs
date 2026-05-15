using NotificationApi.Dtos;

namespace NotificationApi.Interfaces;

public interface INotificationProcessor
{
    Task ProcessAsync(NotificationRequest request);
}
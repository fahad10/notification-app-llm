namespace NotificationApi.Interfaces;

public interface IDiscordNotifier
{
    Task SendMessageAsync(string message);
}
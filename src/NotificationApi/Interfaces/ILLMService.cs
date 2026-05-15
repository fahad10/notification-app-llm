using NotificationApi.Dtos;

namespace NotificationApi.Interfaces;

public interface ILLMService

{

    Task<LLMResponse> GenerateMessageAsync(NotificationRequest request);
}
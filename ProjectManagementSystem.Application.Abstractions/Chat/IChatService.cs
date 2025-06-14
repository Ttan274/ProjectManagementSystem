using ProjectManagementSystem.Application.Abstractions.Chat.Dto;

namespace ProjectManagementSystem.Application.Abstractions.Chat
{
    public interface IChatService
    {
        Task<bool> SaveMessageAsync(ChatMessageDto chatMessage);
        Task<List<ChatMessageDto>> GetChatHistoryAsync(Guid user1, Guid user2);
    }
}

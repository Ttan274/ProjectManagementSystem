using Microsoft.AspNetCore.SignalR;
using ProjectManagementSystem.Application.Abstractions.Chat;
using ProjectManagementSystem.Application.Abstractions.Chat.Dto;

namespace ProjectManagementSystem.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IChatService _chatService;

        public ChatHub(IChatService chatService)
        {
            _chatService = chatService;
        }

        public async Task SenMessageAsync(ChatMessageDto chatMessage)
        {
            await _chatService.SaveMessageAsync(chatMessage);

            await Clients.Caller.SendAsync("MessageSent", chatMessage);

            await Clients.User(chatMessage.ReceiverId.ToString()).SendAsync("MessageReceived", chatMessage);
        }
    }
}

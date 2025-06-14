using ProjectManagementSystem.Application.Abstractions.Base;

namespace ProjectManagementSystem.Application.Abstractions.Chat.Dto
{
    public class ChatMessageDto : BaseDto
    {
        public string? Message { get; set; }

        //Connections
        public Guid SenderId { get; set; }
        public Guid ReceiverId { get; set; }
    }
}

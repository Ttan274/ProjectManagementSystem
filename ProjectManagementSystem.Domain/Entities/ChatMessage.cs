using ProjectManagementSystem.Domain.Common;

namespace ProjectManagementSystem.Domain.Entities
{
    public class ChatMessage : BaseEntity
    {
        public string? Message { get; set; }

        //Connections
        public Guid SenderId { get; set; }
        public AppUser? Sender { get; set; }
        public Guid ReceiverId { get; set; }
        public AppUser? Receiver { get; set; }
    }
}

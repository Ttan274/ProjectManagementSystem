using ProjectManagementSystem.Application.Abstractions.Repositories.Chat;
using ProjectManagementSystem.Persistance.DbContext;

namespace ProjectManagementSystem.Persistance.Repositories.Chat
{
    public class ChatWriteRepository : WriteRepository<Domain.Entities.ChatMessage>, IChatWriteRepository
    {
        public ChatWriteRepository(AppDbContext context) : base(context)
        {
        }
    }
}

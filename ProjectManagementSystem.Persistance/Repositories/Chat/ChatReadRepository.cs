using ProjectManagementSystem.Application.Abstractions.Repositories.Chat;
using ProjectManagementSystem.Persistance.DbContext;

namespace ProjectManagementSystem.Persistance.Repositories.Chat
{
    public class ChatReadRepository : ReadRepository<Domain.Entities.ChatMessage>, IChatReadRepository
    {
        public ChatReadRepository(AppDbContext context) : base(context)
        {
        }
    }
}

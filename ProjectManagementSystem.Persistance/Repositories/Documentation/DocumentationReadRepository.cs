using ProjectManagementSystem.Application.Abstractions.Repositories.Documentation;
using ProjectManagementSystem.Persistance.DbContext;

namespace ProjectManagementSystem.Persistance.Repositories.Documentation
{
    public class DocumentationReadRepository : ReadRepository<Domain.Entities.Documentation>, IDocumentationReadRepository
    {
        public DocumentationReadRepository(AppDbContext context) : base(context)
        {
        }
    }
}

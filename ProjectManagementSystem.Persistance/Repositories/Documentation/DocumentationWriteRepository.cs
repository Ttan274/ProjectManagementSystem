using ProjectManagementSystem.Application.Abstractions.Repositories.Documentation;
using ProjectManagementSystem.Persistance.DbContext;

namespace ProjectManagementSystem.Persistance.Repositories.Documentation
{
    public class DocumentationWriteRepository : WriteRepository<Domain.Entities.Documentation>, IDocumentationWriteRepository
    {
        public DocumentationWriteRepository(AppDbContext context) : base(context)
        {
        }
    }
}

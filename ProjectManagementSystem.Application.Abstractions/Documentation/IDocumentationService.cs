using ProjectManagementSystem.Application.Abstractions.Documentation.Dto;

namespace ProjectManagementSystem.Application.Abstractions.Documentation
{
    public interface IDocumentationService
    {
        Task<bool> CreateDocumentation(DocumentationDto documentation);
        Task<Domain.Entities.Documentation> GetDocumentationByTaskId(Guid id);
    }
}

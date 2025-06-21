using ProjectManagementSystem.Application.Abstractions.Documentation.Dto;

namespace ProjectManagementSystem.Application.Abstractions.Documentation
{
    public interface IDocumentationService
    {
        Task<bool> CreateDocumentation(DocumentationDto documentation);
        Task<bool> DeleteDocumentation(Guid id);
        Task<List<DocumentationDto>> GetAllDocumentations();
    }
}

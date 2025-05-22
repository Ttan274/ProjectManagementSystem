using AutoMapper;
using ProjectManagementSystem.Application.Abstractions.Documentation;
using ProjectManagementSystem.Application.Abstractions.Repositories.Documentation;

namespace ProjectManagementSystem.Application.Documentation
{
    public class DocumentationService : IDocumentationService
    {
        private readonly IDocumentationReadRepository _documentationReadRepository;
        private readonly IDocumentationWriteRepository _documentationWriteRepository;
        private readonly IMapper _mapper;

        public DocumentationService(IDocumentationReadRepository documentationReadRepository, IDocumentationWriteRepository documentationWriteRepository, IMapper mapper)
        {
            _documentationReadRepository = documentationReadRepository;
            _documentationWriteRepository = documentationWriteRepository;
            _mapper = mapper;
        }
    }
}

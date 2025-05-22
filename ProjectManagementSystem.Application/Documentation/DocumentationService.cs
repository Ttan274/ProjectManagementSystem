using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ProjectManagementSystem.Application.Abstractions.Documentation;
using ProjectManagementSystem.Application.Abstractions.Documentation.Dto;
using ProjectManagementSystem.Application.Abstractions.Repositories.Documentation;
using ProjectManagementSystem.Application.Abstractions.Repositories.Task;
using ProjectManagementSystem.Application.Abstractions.Task;

namespace ProjectManagementSystem.Application.Documentation
{
    public class DocumentationService : IDocumentationService
    {
        private readonly IDocumentationReadRepository _documentationReadRepository;
        private readonly IDocumentationWriteRepository _documentationWriteRepository;
        private readonly ITaskReadRepository _taskReadRepository;
        private readonly IMapper _mapper;

        public DocumentationService(IDocumentationReadRepository documentationReadRepository, IDocumentationWriteRepository documentationWriteRepository,
            ITaskReadRepository taskReadRepository, IMapper mapper)
        {
            _documentationReadRepository = documentationReadRepository;
            _documentationWriteRepository = documentationWriteRepository;
            _taskReadRepository = taskReadRepository;
            _mapper = mapper;
        }

        public async Task<bool> CreateDocumentation(DocumentationDto documentation)
        {
            if (documentation is null)
                return false;

            try
            {
                var task = await _taskReadRepository.GetByIdAsync(documentation.TaskId);
                var mappedResult = _mapper.Map<DocumentationDto, Domain.Entities.Documentation>(documentation);
                mappedResult.Task = task;

                var response = await _documentationWriteRepository.AddAsync(mappedResult);

                return response;
            }
            catch (Exception)
            {
                return false;
            }
        }


        //Buradki sıkıntı bütün dökümasyonlar dönüyo sadece aynı departman içindekiler dönmesi lazım
        public async Task<List<DocumentationDto>> GetAllDocumentations()
        {
            try
            {
                var documentations = await _documentationReadRepository.GetQueryable()
                                                                       .Where(x => x.Status)
                                                                       .Include(x => x.Task)
                                                                       .Include(x => x.Task.AppUser)
                                                                       .OrderByDescending(x => x.CreatedDatee).ToListAsync();
                if (documentations is null)
                    return [];

                var mappedResult = _mapper.Map<List<Domain.Entities.Documentation>, List<DocumentationDto>>(documentations);

                return mappedResult;
            }
            catch (Exception)
            {
                return [];
            }
        }
    }
}

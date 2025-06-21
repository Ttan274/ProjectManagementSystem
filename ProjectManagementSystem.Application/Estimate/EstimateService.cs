using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ProjectManagementSystem.Application.Abstractions.Estimate;
using ProjectManagementSystem.Application.Abstractions.Estimate.Dto;
using ProjectManagementSystem.Application.Abstractions.Repositories.Estimate;
using ProjectManagementSystem.Application.Abstractions.Repositories.Project;
using ProjectManagementSystem.Application.Abstractions.Task.Dto;
using ProjectManagementSystem.Common.Enums;
using ProjectManagementSystem.Common.ServiceResponse;

namespace ProjectManagementSystem.Application.Estimate
{
    public class EstimateService(
        IProjectReadRepository projectReadRepo,
        IEstimateReadRepository estimateReadRepo,
        IEstimateWriteRepository estimateWriteRepo,
        IServiceResponseHelper serviceResponseHelper,
        IMapper mapper) : IEstimateService
    {
        public async Task<ServiceResponse<EstimateInfoDto>> GetAllByProjectIdAsync(Guid projectId)
        {
            if(projectId == Guid.Empty)
               return serviceResponseHelper.SetError<EstimateInfoDto>("Request Failed");

            try
            {
                var projectName = await projectReadRepo.GetQueryable()
                    .Where(x => x.Id == projectId)
                    .Select(x => x.ProjectName)
                    .FirstOrDefaultAsync();

                var estimates = await estimateReadRepo.GetQueryable()
                    .Where(x => x.ProjectId == projectId)
                        .Include(x => x.Proje)
                        .Include(x => x.Sprint)
                            .ThenInclude(x => x.Tasks)
                    .Select(x => new EstimateDto
                    {
                        Title = x.Title,
                        SprintName = x.Sprint != null ? x.Sprint.SprintName : string.Empty, 
                        Type = x.Type,
                        CreatedDate = x.CreatedDatee,
                        Id = x.Id,
                        Status = x.Status,
                        TotalEstimate = (x.Sprint != null && x.Sprint.Tasks != null) ? 
                                         x.Sprint.Tasks.Sum(x => x.EffortScore) : 0,
                        TypeDescription = GetTypeDescription(x .Type)
                    })
                    .OrderByDescending(x => x.CreatedDate)
                    .ToListAsync();
                
                var estimateInfo = new EstimateInfoDto() 
                { 
                    ProjectId = projectId,
                    ProjectName = projectName,
                    Estimates = estimates,
                };

                return serviceResponseHelper.SetSuccess(estimateInfo);
            }
            catch (Exception)
            {
                return serviceResponseHelper.SetError<EstimateInfoDto>("Server Error");
            }
        }

        public async Task<ServiceResponse<bool>> CreateAsync(CreateEstimateDto createDto)
        {
            if (createDto is null)
                return serviceResponseHelper.SetError<bool>(false, "Request Failed");

            try
            {
                var mapResult = mapper.Map<CreateEstimateDto, Domain.Entities.Estimate>(createDto);

                var result = await estimateWriteRepo.AddAsync(mapResult);

                return serviceResponseHelper.SetSuccess(result);
            }
            catch (Exception)
            {

                return serviceResponseHelper.SetError<bool>(false, "Server Error");
            }
        }

        private static string GetTypeDescription(int type)
            => type switch
            {
                (int)EstimateType.ZeroToFive => "0-5",
                (int)EstimateType.ZeroToTen => "0-10",
                (int)EstimateType.Doubling => "Doubling",
                (int)EstimateType.Fibonacci => "Fibonacci",
                _ => string.Empty
            };

        public async Task<ServiceResponse<TaskEstimateInfoDto>> GetEstimateTasksInfoAsync(Guid estimateId)
        {
            try
            {
                var estimate = await estimateReadRepo.GetQueryable()
                    .Where(x => x.Id == estimateId)
                        .Include(x => x.Sprint)
                            .ThenInclude(x => x.Tasks)
                         .OrderByDescending(x => x.CreatedDatee)
                         .FirstOrDefaultAsync();

                if (estimate == null || estimate.Sprint == null || estimate.Sprint.Tasks?.Count == 0)
                    return serviceResponseHelper.SetSuccess(new TaskEstimateInfoDto());


                var estimateTasks = new TaskEstimateInfoDto
                {
                    ProjectId = estimate.ProjectId,
                    EstimateId = estimateId,
                    Title = estimate.Title ?? string.Empty,
                    Type = estimate.Type,
                    SprintName = estimate.Sprint.SprintName ?? string.Empty,
                    CreatedDate = estimate.CreatedDatee,
                    Task = estimate.Sprint.Tasks.Where(x => x.Status)
                    .Select(x => new TaskDto
                    {
                        Id = x.Id,
                        AppUser = x.AppUser,
                        EffortScore = x.EffortScore,
                        IsUrgent = x.IsUrgent,
                        TaskName = x.TaskName,
                        State = x.State
                    }).ToList()
                };

                return serviceResponseHelper.SetSuccess(estimateTasks);
            }
            catch (Exception)
            {
                return serviceResponseHelper.SetError<TaskEstimateInfoDto>("Server Error");
            }
        }
    }
}

﻿using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using ProjectManagementSystem.Application.Abstractions.Repositories.Project;
using ProjectManagementSystem.Application.Abstractions.Repositories.Sprint;
using ProjectManagementSystem.Application.Abstractions.Sprint;
using ProjectManagementSystem.Application.Abstractions.Sprint.Dto;
using ProjectManagementSystem.Application.Abstractions.Task;
using ProjectManagementSystem.Application.Abstractions.Task.Dto;
using ProjectManagementSystem.Common.Consts;
using ProjectManagementSystem.Common.ServiceResponse;

namespace ProjectManagementSystem.Application.Sprint
{
    public class SprintService(
        IServiceResponseHelper serviceResponseHelper,
        ISprintReadRepository sprintReadRepository,
        ISprintWriteRepository sprintWriteRepository,
        IProjectReadRepository projectReadRepository,
        ITaskService taskService,
        IMapper mapper) : ISprintService
    {
        public async Task<bool> CreateSprint(SprintDto sprint)
        {
            if (sprint is null)
                return false;

            try
            {
                var project = await projectReadRepository.GetFirstOrDefaultAsync(x => x.Id == sprint.ProjectId);
                var mappedResult = mapper.Map<SprintDto, Domain.Entities.Sprint>(sprint);
                mappedResult.Project = project;

                var response = await sprintWriteRepository.AddAsync(mappedResult);

                return response;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<List<SprintDto>> GetAllSprintsByProjectId(Guid id)
        {
            try
            {
                var sprints = await sprintReadRepository.GetQueryable(tracking: false)
                                                         .Where(x => x.Status)
                                                         .Where(y => y.ProjectId == id)
                                                         .Include(x => x.Tasks)
                                                         .ThenInclude(x => x.AppUser)
                                                         .OrderByDescending(x => x.CreatedDatee)
                                                         .ToListAsync();

                if (sprints is null)
                    return [];

                var mappedResult = mapper.Map<List<Domain.Entities.Sprint>, List<SprintDto>>(sprints);

                return mappedResult;
            }
            catch (Exception)
            {
                return [];
            }
        }

        public async Task<ServiceResponse<List<SprintDto>>> GetListAsync(Guid? projectId = null)
        {
            try
            {
                var sprintQuery = sprintReadRepository.GetQueryable();

                if (projectId != null)
                {
                    sprintQuery = sprintQuery.Where(x => x.ProjectId == projectId);
                }

                var sprints = await sprintQuery.ToListAsync();

                var mappedSprints = mapper.Map<List<SprintDto>>(sprints);

                return serviceResponseHelper.SetSuccess(mappedSprints);
            }
            catch (Exception ex)
            {
                return serviceResponseHelper.SetError<List<SprintDto>>("An unexpected error occured : " + ex.Message, true);
            }
        }

        public async Task<ServiceResponse<List<SprintDetailsDto>>> GetSprintDetailListAsync(Guid projectId)
        {
            if (projectId == Guid.Empty)
            {
                return serviceResponseHelper.SetError<List<SprintDetailsDto>>("Invalid Request");
            }

            try
            {
                var sprintDetails = await sprintReadRepository
                .GetQueryable(tracking: false)
                .Where(sprint => sprint.ProjectId == projectId)
                .Select(sprnt => new SprintDetailsDto
                {
                    Id = sprnt.Id,
                    Name = sprnt.SprintName,
                    StartDate = sprnt.StartDate,
                    FinishDate = sprnt.FinishDate,
                    ProjectId = sprnt.ProjectId,
                    Tasks = sprnt
                        .Tasks
                        .Select(task => new TaskSummaryDto()
                        {
                            Id = task.Id,
                            Description = task.TaskDesc,
                            Name = task.TaskName,
                            AssignedMember = task.AppUser == null ? Defaults.UNKNOWN_USER : task.AppUser.UserName,
                            EffortScore = task.EffortScore,
                            Type = task.Type,
                            CreatedAt = task.CreatedDatee,
                            CompletedAt = task.CompletedAt,
                            IsCompleted = task.State == Common.Enums.ProjectStatus.Done,
                            Number = task.TaskNumber,
                            Priority = task.Priority,
                            HasDocumentation = task.Documentation != null
                        }).ToList()
                })
                .ToListAsync();

                if (sprintDetails == null)
                {
                    return serviceResponseHelper.SetError<List<SprintDetailsDto>>("Sprint does not exist");
                }

                return serviceResponseHelper.SetSuccess(sprintDetails);
            }
            catch (MySqlException)
            {
                return serviceResponseHelper.SetError<List<SprintDetailsDto>>("Database error occurred", true);
            }
            catch (InvalidOperationException)
            {
                return serviceResponseHelper.SetError<List<SprintDetailsDto>>("Invalid operation error occured", true);
            }
            catch (Exception ex)
            {
                return serviceResponseHelper.SetError<List<SprintDetailsDto>>("An unexpected error occured : " + ex.Message, true);
            }
        }
    }
}

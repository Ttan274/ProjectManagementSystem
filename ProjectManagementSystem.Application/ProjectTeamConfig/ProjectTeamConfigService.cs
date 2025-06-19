using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using ProjectManagementSystem.Application.Abstractions.ProjectTeamConfig;
using ProjectManagementSystem.Application.Abstractions.ProjectTeamConfig.Dto;
using ProjectManagementSystem.Application.Abstractions.Repositories.ProjectTeamConfig;
using ProjectManagementSystem.Common.ServiceResponse;
using System.Text;
using System.Text.Json;

namespace ProjectManagementSystem.Application.ProjectTeamConfig
{
    public class ProjectTeamConfigService(
        IServiceResponseHelper serviceResponseHelper,
        IMapper mapper,
        IProjectTeamConfigReadRepository teamConfigReadRepository,
        IProjectTeamConfigWriteRepository teamConfigWriteRepository,
        HttpClient ollamaClient, IConfiguration configuration) : IProjectTeamConfigService
    {
        private readonly string projectConfigModel = configuration["OllamaOptions:SubTaskModel"];
        public async Task<ServiceResponse<ProjectTeamConfigDto>> CreateAsync(ProjectTeamConfigDto projectTeamConfig)
        {
            if (projectTeamConfig == null)
            {
                return serviceResponseHelper.SetError<ProjectTeamConfigDto>("Invalid request.");
            }

            projectTeamConfig.Validate(out string validationMessage);

            if (!string.IsNullOrWhiteSpace(validationMessage))
            {
                return serviceResponseHelper.SetError<ProjectTeamConfigDto>(validationMessage);
            }

            try
            {
                var mappedModel = mapper.Map<Domain.Entities.ProjectTeamConfig>(projectTeamConfig);

                await teamConfigWriteRepository.AddAsync(mappedModel);

                return serviceResponseHelper.SetSuccess(projectTeamConfig);
            }
            catch (Exception)
            {
                return serviceResponseHelper.SetError<ProjectTeamConfigDto>("Internal server error occured.");
            }
        }

        public async Task<ServiceResponse<ProjectTeamConfigDto?>> GetByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
            {
                return serviceResponseHelper.SetError<ProjectTeamConfigDto?>("Invalid request.");
            }

            try
            {
                var projectTeamConfig = teamConfigReadRepository.GetByIdAsync(id, false);

                if (projectTeamConfig == null)
                {
                    return serviceResponseHelper.SetError<ProjectTeamConfigDto?>("Not found.");
                }

                var mappedConfig = mapper.Map<ProjectTeamConfigDto>(projectTeamConfig);

                return serviceResponseHelper.SetSuccess(mappedConfig);
            }
            catch (Exception)
            {
                return serviceResponseHelper.SetError<ProjectTeamConfigDto?>("Internal server error occured.");
            }
        }

        public async Task<ServiceResponse<ProjectTeamConfigDto?>> GetByProjectIdAsync(Guid projectId)
        {
            if (projectId == Guid.Empty)
            {
                return serviceResponseHelper.SetError<ProjectTeamConfigDto?>("Invalid request.");
            }

            try
            {
                var projectTeamConfig = await teamConfigReadRepository
                    .GetQueryable()
                    .FirstOrDefaultAsync(x => x.ProjectId == projectId);

                if (projectTeamConfig == null)
                {
                    return serviceResponseHelper.SetError<ProjectTeamConfigDto?>("Not found.");
                }

                var mappedConfig = mapper.Map<ProjectTeamConfigDto>(projectTeamConfig);

                return serviceResponseHelper.SetSuccess(mappedConfig);
            }
            catch (Exception)
            {
                return serviceResponseHelper.SetError<ProjectTeamConfigDto?>("Internal server error occured.");
            }
        }
        public async Task<ServiceResponse<ProjectConfigSuggestionDto>> GetOllamaSuggestionsAsync(ProjectTeamProfileDto teamProfile)
        {
            if (string.IsNullOrWhiteSpace(teamProfile?.TeamIntroduction))
            {
                return serviceResponseHelper.SetError<ProjectConfigSuggestionDto>("Team profile is required.");
            }

            try
            {
                var requestPayload = new
                {
                    model = projectConfigModel,
                    prompt = @$"
Given the following software development team profile, analyze their strengths and weaknesses and redistribute the weight percentages among the following four performance criteria accordingly. The total must equal 100%.

Criteria:
- Task Completion
- On-Time Delivery
- Target Proximity
- Code Quality

Respond in STRICTLY valid JSON format ONLY. Do not include any explanation, markdown formatting, or extra characters — only return valid JSON. This is mandatory.
Be careful about json SHOULD BE VALID.

**Important**: Ensure that the total of all four criteria adds up to exactly 100%. The sum of these criteria values MUST be exactly 100, no more, no less.

Example format:
{{
  ""Task Completion"": {{
    ""value"": ...,
    ""reason"": ""Explanation for the value""
  }},
  ""On-Time Delivery"": {{
    ""value"": ...,
    ""reason"": ""Explanation for the value""
  }},
  ""Target Proximity"": {{
    ""value"": ...,
    ""reason"": ""Explanation for the value""
  }},
  ""Code Quality"": {{
    ""value"": ...,
    ""reason"": ""Explanation for the value""
  }}
}}

Team Profile:
{teamProfile.TeamIntroduction}

STRICTLY ensure the total of the four criteria sums up to exactly 100%.
",
                    stream = false
                };

                var content = new StringContent(JsonSerializer.Serialize(requestPayload), Encoding.UTF8, "application/json");

                var response = await ollamaClient.PostAsync("/api/generate", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();

                    if (string.IsNullOrWhiteSpace(responseContent))
                    {
                        return serviceResponseHelper.SetError<ProjectConfigSuggestionDto>("No valid response data received.");
                    }

                    try
                    {
                        var parsedResponse = JObject.Parse(responseContent);
                        var responseText = parsedResponse["response"]?.ToString();

                        if (string.IsNullOrWhiteSpace(responseText))
                        {
                            return serviceResponseHelper.SetError<ProjectConfigSuggestionDto>("No valid response data received.");
                        }

                        var cleanedJson = responseText.Trim();

                        if (!cleanedJson.StartsWith("{"))
                        {
                            cleanedJson = "{" + cleanedJson;
                        }

                        if (!cleanedJson.EndsWith("}}"))
                        {
                            cleanedJson = cleanedJson + "}";
                        }

                        cleanedJson = cleanedJson.Replace("“", "\"").Replace("”", "\"");

                        var suggestionDto = JsonSerializer.Deserialize<ProjectConfigSuggestionDto>(cleanedJson);

                        if (suggestionDto == null)
                        {
                            return serviceResponseHelper.SetError<ProjectConfigSuggestionDto>("Failed to parse response as valid JSON.");
                        }

                        return serviceResponseHelper.SetSuccess(suggestionDto);
                    }
                    catch (JsonException)
                    {
                        return serviceResponseHelper.SetError<ProjectConfigSuggestionDto>("Invalid JSON response received.");
                    }
                }

                return serviceResponseHelper.SetError<ProjectConfigSuggestionDto>("Failed to get a response from Ollama API.");
            }
            catch (Exception ex)
            {
                return new ServiceResponse<ProjectConfigSuggestionDto>
                {
                    Success = false,
                    Message = $"An error occurred: {ex.Message}"
                };
            }
        }

        public ServiceResponse<ProjectTeamConfigDto> Update(ProjectTeamConfigDto projectTeamConfig)
        {
            if (projectTeamConfig == null)
            {
                return serviceResponseHelper.SetError<ProjectTeamConfigDto>("Invalid request.");
            }

            projectTeamConfig.Validate(out string validationMessage);

            if (!string.IsNullOrWhiteSpace(validationMessage))
            {
                return serviceResponseHelper.SetError<ProjectTeamConfigDto>(validationMessage);
            }

            try
            {
                var mappedModel = mapper.Map<Domain.Entities.ProjectTeamConfig>(projectTeamConfig);

                teamConfigWriteRepository.Update(mappedModel);

                return serviceResponseHelper.SetSuccess(projectTeamConfig);
            }
            catch (Exception)
            {
                return serviceResponseHelper.SetError<ProjectTeamConfigDto>("Internal server error occured.");
            }
        }
    }
}

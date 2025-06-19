using System.Text.Json.Serialization;

namespace ProjectManagementSystem.Application.Abstractions.ProjectTeamConfig.Dto;
public class ProjectConfigSuggestionDto
{
    public ProjectConfigSuggestionDto()
    {
        TaskCompletion = new();
        OnTimeDelivery = new();
        TargetProximity = new();
        CodeQuality = new();
    }

    [JsonPropertyName("Task Completion")]
    public EvaluationCriterion TaskCompletion { get; set; }

    [JsonPropertyName("On-Time Delivery")]
    public EvaluationCriterion OnTimeDelivery { get; set; }

    [JsonPropertyName("Target Proximity")]
    public EvaluationCriterion TargetProximity { get; set; }

    [JsonPropertyName("Code Quality")]
    public EvaluationCriterion CodeQuality { get; set; }
}


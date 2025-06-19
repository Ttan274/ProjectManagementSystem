using System.Text.Json.Serialization;

namespace ProjectManagementSystem.Application.Abstractions.ProjectTeamConfig.Dto
{
    public class EvaluationCriterion
    {
        public EvaluationCriterion()
        {
        }

        [JsonPropertyName("value")]
        public int Value { get; set; }

        [JsonPropertyName("reason")]
        public string? Reason { get; set; }
    }
}

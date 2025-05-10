using System.Text.Json.Serialization;

namespace ProjectManagementSystem.Application.Abstractions.SubTaskProducer.Dto
{
    public class CreateSubTaskRequestDto
    {
        public CreateSubTaskRequestDto()
        {
            Model = "mistral";
            Prompt = "Return me something in this format :\n[\n  {\"title\": \"...\", \"description\": \"...\"}\n]";
        }
        [JsonPropertyName("model")]
        public string Model { get; set; }
        [JsonPropertyName("prompt")]
        public string Prompt { get; set; }
    }
}

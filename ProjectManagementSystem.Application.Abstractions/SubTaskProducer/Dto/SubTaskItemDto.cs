using Newtonsoft.Json;

namespace ProjectManagementSystem.Application.Abstractions.SubTaskProducer.Dto
{
    public class SubTaskItemDto
    {
        public SubTaskItemDto()
        {

        }
        [JsonProperty("title")]
        public string? Title { get; set; }
        [JsonProperty("description")]
        public string? Description { get; set; }
    }
}

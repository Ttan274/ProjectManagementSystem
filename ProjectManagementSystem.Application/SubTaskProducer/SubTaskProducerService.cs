using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ProjectManagementSystem.Application.Abstractions.SubTaskProducer;
using ProjectManagementSystem.Application.Abstractions.SubTaskProducer.Dto;
using System.Text;

namespace ProjectManagementSystem.Application.SubTaskProducer
{
    public class SubTaskProducerService(HttpClient ollamaClient, IConfiguration configuration) : ISubTaskProducerService
    {
        private readonly string subTaskModel = configuration["OllamaOptions:SubTaskModel"];
        public async Task<List<SubTaskItemDto>> GenerateSubTasksAsync(string taskDescription)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(subTaskModel))
                {
                    return [];
                }

                var payload = new
                {
                    model = subTaskModel,
                    prompt = $"Bir yazılım görevi için en fazla 3 adet alt görev listesi üret. Görev: \"{taskDescription}\". Sadece şu JSON formatında dön: [{{\"title\": \"...\", \"description\": \"...\"}}]",
                    stream = false
                };

                var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");

                var response = await ollamaClient.PostAsync("/api/generate", content);

                if (!response.IsSuccessStatusCode)
                    return [];

                var ollamaResponse = await response.Content.ReadAsStringAsync();

                if (string.IsNullOrWhiteSpace(ollamaResponse))
                    return [];

                var responseText = ParseOllamaResponse(ollamaResponse);

                if (string.IsNullOrWhiteSpace(responseText))
                    return [];

                var cleanedJson = responseText.Trim();

                var subTasks = JsonConvert.DeserializeObject<List<SubTaskItemDto>>(cleanedJson);

                return subTasks ?? [];
            }
            catch (Exception)
            {
                return [];
            }
        }

        private static string? ParseOllamaResponse(string ollamaResponse)
        {
            var parsedResponse = JObject.Parse(ollamaResponse);

            var responseText = parsedResponse["response"]?.ToString();

            if (string.IsNullOrWhiteSpace(responseText))
                return null;

            // response 1. [ ... ] şeklinde geldiği için ilk kapalı parantez ile son kapalı parantez arasına bakıyoruz.
            int jsonStart = responseText.IndexOf('[');

            int jsonEnd = responseText.LastIndexOf(']');

            if (jsonStart == -1 || jsonEnd == -1 || jsonEnd <= jsonStart)
                return null;

            var jsonSubstring = responseText.Substring(jsonStart, jsonEnd - jsonStart + 1);

            return jsonSubstring;
        }
    }
}

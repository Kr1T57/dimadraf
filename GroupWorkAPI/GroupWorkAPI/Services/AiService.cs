using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace GroupWorkAPI.Services
{
    public class AiService
    {
        private readonly HttpClient _httpClient;

        public AiService(IConfiguration configuration)
        {
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            };

            _httpClient = new HttpClient(handler)
            {
                Timeout = TimeSpan.FromSeconds(45)
            };

            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64)");
        }

        public async Task<string?> AskAiAsync(string prompt, string systemRole = "You are an expert ASICS running coach.")
        {
            try
            {
                // Международный открытый эндпоинт формата OpenAI (НЕ блокирует РФ и Docker)
                string endpoint = "https://text.pollinations.ai/openai/chat/completions";

                var payload = new
                {
                    model = "mistral", // Качественная быстрая модель с отличным русским языком
                    messages = new[]
                    {
                        new { role = "system", content = systemRole },
                        new { role = "user", content = prompt }
                    },
                    temperature = 0.5
                };

                using var request = new HttpRequestMessage(HttpMethod.Post, endpoint);
                request.Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

                var response = await _httpClient.SendAsync(request);
                var responseBody = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"[AI GATEWAY ERROR]: {response.StatusCode} - {responseBody}");
                    return null;
                }

                using var doc = JsonDocument.Parse(responseBody);
                var root = doc.RootElement;

                if (root.TryGetProperty("choices", out var choices) && choices.GetArrayLength() > 0)
                {
                    return choices[0]
                        .GetProperty("message")
                        .GetProperty("content")
                        .GetString()?.Trim();
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AI CONNECTION ERROR]: {ex.Message}");
                return null;
            }
        }
    }
}
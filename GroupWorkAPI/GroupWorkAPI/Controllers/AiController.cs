using System.Text.Json;
using System.Text.RegularExpressions;
using GroupWorkAPI.DataClasses;
using GroupWorkAPI.Model;
using GroupWorkAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GroupWorkAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AiController : ControllerBase
    {
        private readonly PostgresContext _context;
        private readonly AiService _aiService;

        public AiController(PostgresContext context, AiService aiService)
        {
            _context = context;
            _aiService = aiService;
        }

        [HttpPost("Coach")]
        public async Task<IActionResult> Coach([FromBody] AiCoachRequestDTO request)
        {
            if (string.IsNullOrWhiteSpace(request?.Message))
            {
                return Ok(new AiCoachResponseDTO { Reply = "Пожалуйста, задайте вопрос тренеру." });
            }

            try
            {
                // Загружаем список товаров для передачи в ИИ
                var availableProducts = await _context.Products
                    .Select(p => new { p.Id, p.Name, p.BasePrice })
                    .ToListAsync();

                string productsList = string.Join("\n", availableProducts.Select(p => $"ID {p.Id}: {p.Name} (Цена: {p.BasePrice} руб)"));

                string systemPrompt = @"Ты — профессиональный спортивный тренер и эксперт по подбору экипировки ASICS ('AI Running Coach').
Твоя задача — дать профессиональную консультацию клиенту на русском языке.
ОБЯЗАТЕЛЬНО выбери из списка доступных товаров 1-3 подходящие модели и укажи их точные ID.

Формат ответа СТРОГО следующий валидный JSON (без markdown-тегов и без ```json):
{
  ""reply"": ""Твой развернутый ответ, почему именно эти модели подходят под параметры человека"",
  ""productIds"": [id1, id2]
}";

                string userPrompt = $"Список доступных товаров в магазине:\n{productsList}\n\nВопрос покупателя:\n{request.Message}";

                string? aiResponse = await _aiService.AskAiAsync(userPrompt, systemPrompt);

                if (string.IsNullOrWhiteSpace(aiResponse))
                {
                    return Ok(new AiCoachResponseDTO
                    {
                        Reply = "К сожалению, сервис ИИ не ответил на запрос. Пожалуйста, попробуйте еще раз."
                    });
                }

                // Очистка ответа от возможных markdown-оберток
                string cleanJson = Regex.Replace(aiResponse, @"```json\s*", "");
                cleanJson = Regex.Replace(cleanJson, @"```\s*", "").Trim();

                using var doc = JsonDocument.Parse(cleanJson);
                var root = doc.RootElement;

                string reply = root.TryGetProperty("reply", out var r) ? r.GetString() ?? "" : aiResponse;
                List<int> ids = new();

                if (root.TryGetProperty("productIds", out var idsElement))
                {
                    foreach (var item in idsElement.EnumerateArray())
                    {
                        if (item.TryGetInt32(out int id)) ids.Add(id);
                    }
                }

                // Прикрепляем карточки тех товаров, которые выбрал ИИ
                var selectedByAi = await _context.Products
                    .Where(p => ids.Contains(p.Id))
                    .Select(p => new AiProductRecommendationDTO
                    {
                        ProductId = p.Id,
                        ProductName = p.Name,
                        Price = p.BasePrice,
                        Reason = "Выбор тренера ASICS"
                    })
                    .ToListAsync();

                return Ok(new AiCoachResponseDTO
                {
                    Reply = reply,
                    Recommendations = selectedByAi
                });
            }
            catch (Exception ex)
            {
                return Ok(new AiCoachResponseDTO
                {
                    Reply = $"Ошибка обработки ответа ИИ: {ex.Message}",
                    Recommendations = new()
                });
            }
        }

        [HttpPost("GenerateDescription")]
        public async Task<IActionResult> GenerateDescription([FromBody] AiGenerateDescriptionDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                return BadRequest("Укажите модель.");
            }

            string systemPrompt = "Ты — спортивный маркетолог ASICS. Напиши профессиональное продающее описание товара на русском языке (3 содержательных предложения) без кавычек и вступительных фраз.";
            string userPrompt = $"Модель: {dto.Name}, Категория: {dto.Category}. Технологии: {dto.Technologies ?? "PureGEL, FF BLAST"}";

            string? description = await _aiService.AskAiAsync(userPrompt, systemPrompt);

            return Ok(new { description = (description ?? "").Trim('"', ' ') });
        }
    }
}
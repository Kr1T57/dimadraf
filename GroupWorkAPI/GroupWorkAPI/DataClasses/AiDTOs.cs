namespace GroupWorkAPI.DataClasses
{
    public class AiCoachRequestDTO
    {
        public string Message { get; set; } = string.Empty;
    }

    public class AiProductRecommendationDTO
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Reason { get; set; } = string.Empty;
    }

    public class AiCoachResponseDTO
    {
        public string Reply { get; set; } = string.Empty;
        public List<AiProductRecommendationDTO> Recommendations { get; set; } = new();
    }

    public class AiGenerateDescriptionDTO
    {
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public string? Technologies { get; set; }
    }
}
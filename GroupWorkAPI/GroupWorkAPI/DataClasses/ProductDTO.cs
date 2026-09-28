    namespace GroupWorkAPI.DataClasses
{
    public class ProductDTO
    {
        public int Id { get; set; }

        public string Brand { get; set; } = null!;

        public string Category { get; set; } = null!;

        public string Name { get; set; } = null!;

        public string Description { get; set; } = null!;

        public decimal BasePrice { get; set; }

        public List<ProductVariantDTO> Variants { get; set; } = new();
    }
}

namespace GroupWorkAPI.DataClasses
{
    public class ProductVariantDTO
    {
        public int Id { get; set; }

        public int ProductId { get; set; }

        public string Color { get; set; } = null!;

        public decimal Size { get; set; }

        public int StockQuantity { get; set; }
    }
}

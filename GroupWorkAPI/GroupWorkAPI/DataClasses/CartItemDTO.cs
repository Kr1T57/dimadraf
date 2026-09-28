namespace GroupWorkAPI.DataClasses
{
    public class CartItemDTO
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public int ProductVariantId { get; set; }

        public int Quantity { get; set; }

        //поля для фронтента
        public string ProductName { get; set; } = null!;
        public string Color { get; set; } = null!;
        public decimal Size { get; set; }
        public decimal Price { get; set; }
        //
    }
}

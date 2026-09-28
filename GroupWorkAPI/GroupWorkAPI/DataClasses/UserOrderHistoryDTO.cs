namespace GroupWorkAPI.DataClasses
{
    public class UserOrderHistoryDTO
    {
        public int OrderId { get; set; }
        public string OrderDate { get; set; } = null!;
        public string Status { get; set; } = null!;
        public decimal TotalAmount { get; set; }
        public List<UserOrderItemDTO> Items { get; set; } = new();
    }
    public class UserOrderItemDTO
    {
        public int ProductVariantId { get; set; }
        public string ProductName { get; set; } = null!;
        public string Color { get; set; } = null!;
        public decimal Size { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}

using GroupWorkAPI.Model;

namespace GroupWorkAPI.DataClasses
{
    public class OrderItemDTO
    {
        public int Id { get; set; }

        public Order Order { get; set; } = null!;

        public ProductVariant ProductVariant { get; set; } = null!;

        public int Quantity { get; set; }

        public decimal PriceAtPurchase { get; set; }
    }
}

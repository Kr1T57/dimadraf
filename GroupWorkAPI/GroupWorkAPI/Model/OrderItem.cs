using GroupWorkAPI.DataClasses;

namespace GroupWorkAPI;

public partial class OrderItem
{
    public int Id { get; set; }

    public int OrderId { get; set; }

    public int ProductVariantId { get; set; }

    public int Quantity { get; set; }

    public decimal PriceAtPurchase { get; set; }

    public virtual Order Order { get; set; } = null!;

    public virtual ProductVariant ProductVariant { get; set; } = null!;
    public OrderItemDTO OrderItemsdto()
    {
        return new()
        {
            Id = Id,
            Order = Order,
            ProductVariant = ProductVariant,
            Quantity = Quantity,
            PriceAtPurchase = PriceAtPurchase,
        };
    }
}

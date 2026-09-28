using GroupWorkAPI.DataClasses;

namespace GroupWorkAPI;

public partial class ProductVariant
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public int ColorId { get; set; }

    public decimal Size { get; set; }

    public int StockQuantity { get; set; }

    public virtual Color Color { get; set; } = null!;

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    public virtual ICollection<VariantImage> VariantImages { get; set; } = new List<VariantImage>();
    public virtual ICollection<VariantFavorite> VariantFavorites { get; set; } = new List<VariantFavorite>();


    public virtual Product Product { get; set; } = null!;
    public ProductVariantDTO ToDto()
    {
        return new()
        {
            Id = Id,
            ProductId = ProductId,
            Color = Color.Name,
            Size = Size,
            StockQuantity = StockQuantity,
        };
    }
}

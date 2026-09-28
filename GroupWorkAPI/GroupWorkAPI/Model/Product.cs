using GroupWorkAPI.DataClasses;
using GroupWorkAPI.Model;

namespace GroupWorkAPI;

public partial class Product
{
    public int Id { get; set; }

    public int BrandId { get; set; }

    public int CategoryId { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public decimal BasePrice { get; set; }

    public virtual Brand Brand { get; set; } = null!;

    public virtual Category Category { get; set; } = null!;

    public virtual ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();

    public virtual ICollection<ProductVariant> ProductVariants { get; set; } = new List<ProductVariant>();
    public virtual ICollection<ProductFavorite> ProductFavorites { get; set; } = new List<ProductFavorite>();
    public ProductDTO ToDto()
    {
        return new()
        {
            Id = Id,
            Brand = Brand.Name,
            Category = Category.Name,
            Name = Name,
            Description = Description,
            BasePrice = BasePrice,

            Variants = [.. ProductVariants.Select(v => new ProductVariantDTO
            {
                Id = v.Id,
                ProductId = v.ProductId,
                Color = v.Color?.Name ?? "Unknown",
                Size = v.Size,
                StockQuantity = v.StockQuantity
            })]
        };
    }
}

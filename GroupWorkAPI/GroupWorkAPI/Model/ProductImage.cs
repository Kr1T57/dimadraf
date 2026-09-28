using GroupWorkAPI.DataClasses;

namespace GroupWorkAPI;

public partial class ProductImage
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public string ImageUrl { get; set; } = null!;

    public bool IsMain { get; set; }

    public virtual Product Product { get; set; } = null!;
    public ProductImageDTO ToDto()
    {
        return new()
        {
            Id = Id,
            ProductId = ProductId,
            ImageUrl = ImageUrl,
            IsMain = IsMain,
        };
    }
}

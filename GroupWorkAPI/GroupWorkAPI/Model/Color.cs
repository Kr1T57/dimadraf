using GroupWorkAPI.DataClasses;

namespace GroupWorkAPI;

public partial class Color
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<ProductVariant> ProductVariants { get; set; } = new List<ProductVariant>();
    public ColorDTO ToDto()
    {
        return new()
        {
            Id = Id,
            Name = Name,
        };
    }
}

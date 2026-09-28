using GroupWorkAPI.DataClasses;

namespace GroupWorkAPI.Model;

public partial class Brand
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    public BrandDTO ToDto()
    {
        return new()
        {
            Id = Id,
            Name = Name,
        };
    }
}

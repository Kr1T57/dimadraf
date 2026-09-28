using GroupWorkAPI.DataClasses;

namespace GroupWorkAPI;

public partial class OrderStatus
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    public OrderStatusDTO ToDto()
    {
        return new()
        {
            Id = Id,
            Name = Name,
        };
    }
}

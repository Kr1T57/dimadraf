using GroupWorkAPI.DataClasses;

namespace GroupWorkAPI;

public partial class Order
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public DateTime OrderDate { get; set; }

    public int StatusId { get; set; }

    public decimal TotalAmount { get; set; }

    public string City { get; set; } = null!;

    public string Street { get; set; } = null!;

    public string House { get; set; } = null!;

    public string PostalCode { get; set; } = null!;

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual OrderStatus Status { get; set; } = null!;

    public virtual User User { get; set; } = null!;
    public OrderDTO ToDto()
    {
        return new()
        {
            Id = Id,
            User = User.Email,
            OrderDate = OrderDate.ToLongDateString(),
            Status = Status.Name,
            TotalAmount = TotalAmount,
            City = City,
            Street = Street,
            House = House,
            PostalCode = PostalCode,
        };
    }
}

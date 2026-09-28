using GroupWorkAPI.DataClasses;

namespace GroupWorkAPI;

public partial class User
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Surname { get; set; } = null!;

    public string? Patronymic { get; set; }

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public int RoleId { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    public virtual ICollection<ProductFavorite> ProductFavorites { get; set; } = new List<ProductFavorite>();
    public virtual ICollection<VariantFavorite> VariantFavorites { get; set; } = new List<VariantFavorite>();


    public virtual Role Role { get; set; } = null!;

    public UserDTO ToDto()
    {
        return new()
        {
            Id = Id,
            Name = Name,
            Surname = Surname,
            Patronymic = Patronymic,
            Email = Email,
            Password = Password,
            Phone = Phone,
            Role = Role.Name,
        };
    }
}


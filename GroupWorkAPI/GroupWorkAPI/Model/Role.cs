using GroupWorkAPI.DataClasses;

namespace GroupWorkAPI;

public partial class Role
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<User> Users { get; set; } = new List<User>();
    public RoleDTO ToDto()
    {
        return new()
        {
            Id = Id,
            Name = Name,
        };
    }
}

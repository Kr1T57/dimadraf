using GroupWorkAPI.DataClasses;
using Microsoft.EntityFrameworkCore;
using System.Net.NetworkInformation;

namespace GroupWorkAPI
{
    public class ProductFavorite
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public int ProductId { get; set; }

        public virtual Product Product { get; set; } = null!;

        public virtual User User { get; set; } = null!;
        public ProductFavorite ToDto()
        {
            return new()
            {
                Id = Id,
                UserId=UserId,
                ProductId=ProductId,
            };
        }
    }
}

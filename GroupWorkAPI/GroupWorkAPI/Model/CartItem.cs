using GroupWorkAPI.DataClasses;
using System.Xml.Linq;

namespace GroupWorkAPI
{
    public class CartItem
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public int ProductVariantId { get; set; }

        public int Quantity { get; set; }

        public virtual ProductVariant ProductVariant { get; set; } = null!;

        public virtual User User { get; set; } = null!;
        public CartItemDTO ToDto()
        {
            return new()
            {
                Id = Id,
                UserId = UserId,
                ProductVariantId = ProductVariantId,
                Quantity = Quantity,

                ProductName = ProductVariant.Product.Name,
                Color = ProductVariant.Color.Name,
                Size = ProductVariant.Size,
                Price = ProductVariant.Product.BasePrice
            };
        }
    }
}

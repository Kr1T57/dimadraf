namespace GroupWorkAPI
{
    public class VariantFavorite
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public int ProductVariantId { get; set; }

        public virtual ProductVariant ProductVariant { get; set; } = null!;

        public virtual User User { get; set; } = null!;

        public VariantFavorite ToDto()
        {
            return new()
            {
                Id = Id,
                UserId = UserId,
                ProductVariantId = ProductVariantId,
            };
        }
    }
}

namespace GroupWorkAPI.DataClasses
{
    public class VariantImageDTO
    {
        public int Id { get; set; }

        public int ProductVariantId { get; set; }

        public string ImagePath { get; set; } = null!;
    }
}

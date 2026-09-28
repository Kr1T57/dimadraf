namespace GroupWorkAPI.DataClasses
{
    public class ProductImageDTO
    {
        public int Id { get; set; }

        public int ProductId { get; set; }

        public string ImageUrl { get; set; } = null!;

        public bool IsMain { get; set; }
    }
}

namespace GroupWorkAPI.DataClasses
{
    public class OrderDTO
    {
        public int Id { get; set; }

        public string User { get; set; } = null!;

        public string OrderDate { get; set; } = null!;

        public string Status { get; set; } = null!;

        public decimal TotalAmount { get; set; }

        public string City { get; set; } = null!;

        public string Street { get; set; } = null!;

        public string House { get; set; } = null!;

        public string PostalCode { get; set; } = null!;
    }
}

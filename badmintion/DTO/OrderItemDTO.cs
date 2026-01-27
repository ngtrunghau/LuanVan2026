namespace badmintion.DTO
{
    public class OrderItemDTO
    {
        public int Id { get; set; }

        public int? Quantity { get; set; }

        public decimal? Price { get; set; }

        public int? ProductsId { get; set; }

        public int? Orders { get; set; }

        public bool? IsDeleted { get; set; }
    }
}

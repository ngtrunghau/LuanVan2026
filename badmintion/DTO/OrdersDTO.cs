namespace badmintion.DTO
{
    public class OrdersDTO
    {
        public int Id { get; set; }

        public DateTime? OrderDate { get; set; }

        public decimal? TotalAmount { get; set; }

        public int? Status { get; set; }

        public int? CustomerId { get; set; }
        public int? AddressId { get; set; }

        public bool? IsDeleted { get; set; }

        public List<OrderItemDTO> ListOrderItems { get; set; }
    }
}

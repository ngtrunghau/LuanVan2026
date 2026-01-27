namespace badmintion.DTO
{
    public class ShippingDetailDTO
    {
        public int Id { get; set; }

        public int? Status { get; set; }

        public DateTime? DateShip { get; set; }

        public int? OrdersId { get; set; }

        public bool? IsDeleted { get; set; }

    }
}

namespace badmintion.DTO
{
    public class AddressCustomerDTO
    {
        public int Id { get; set; }

        public string Address { get; set; } = null!;

        public int? ProvinceId { get; set; }

        public int? DistrictId { get; set; }

        public int? TownId { get; set; }

        public int? CustomerId { get; set; }

        public bool? IsDeleted { get; set; }

    }

    public class AddressDetail
    {
        public int Id { get; set; }
        public string Province { get; set; }
        public string Address { get; set; }
        public string District { get; set; }
        public string Town { get; set; }
        public int? CustomerId { get; set; }
    }
}

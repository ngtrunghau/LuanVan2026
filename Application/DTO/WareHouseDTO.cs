namespace badmintion.DTO
{
    public class WareHouseDTO
    {

        public int Id { get; set; }

        public string? Name { get; set; }

        public int? ProductId { get; set; }

        public int QuantityImport { get; set; } = 0;

        public int? RemainQuantity { get; set; }

        public bool? IsDeleted { get; set; } = false;
    }
}

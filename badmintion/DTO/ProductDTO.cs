namespace badmintion.DTO
{
    public class ProductDTO
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string? Descriptions { get; set; }

        public decimal? Price { get; set; }

        public int? StockQuantity { get; set; } 

        public string? ImageUrl { get; set; }

        public string? Color { get; set; }

        public int? CategoriesId { get; set; }

        public bool? IsDeleted { get; set; }
    }
}

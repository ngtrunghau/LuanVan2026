namespace badmintion.DTO
{
    public class CategoriesDTO
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public bool? IsDeleted { get; set; }
        public int? Sort { get; set; } = 0;
    }
}

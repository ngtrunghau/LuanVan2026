namespace badmintion.DTO
{
    public class ProductReviewDTO
    {
        public int Id { get; set; }

        public int? ProductId { get; set; }

        public int? TotalStar { get; set; }

        public string? Comment { get; set; }

        public string? UrlImg { get; set; }
        public bool? IsDeleted { get; set; }

        public DateTime? Date { get; set; } = DateTime.Now;

    }
}

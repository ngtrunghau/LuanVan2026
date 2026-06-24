namespace badmintion.DTO
{
    public class ProductReviewDTO
    {
        public int Id { get; set; }

        public int? ProductId { get; set; }

        public int? OrderId { get; set; }

        public int? TotalStar { get; set; }

        public string? Comment { get; set; }

        public string? UrlImg { get; set; }
        public bool? IsDeleted { get; set; }

        public DateTime? Date { get; set; } = DateTime.Now;

    }

    public class ProductReviewFilterDTO
    {
        public int Start { get; set; } = 1;
        public int Limit { get; set; } = 10;
        public int? Status { get; set; }
        public string? Keyword { get; set; }
    }

    public class ProductReviewModerationDTO
    {
        public int Id { get; set; }
        public int Status { get; set; }
        public string? Reason { get; set; }
    }
}

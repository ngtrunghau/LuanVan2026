namespace badmintion.DTO
{
    public class PromotionPagingRequest
    {
        public int Start { get; set; } = 1;
        public int Limit { get; set; } = 10;
        public string? Search { get; set; }
        public string? State { get; set; }
        public int Skip => (Start > 0 ? Start - 1 : 0) * Math.Max(1, Limit);
    }

    public class PromotionStatusRequest
    {
        public int Id { get; set; }
        public bool IsActive { get; set; }
    }

    public class PromotionValidationRequest
    {
        public string Code { get; set; } = string.Empty;
        public decimal OrderValue { get; set; }
    }
}

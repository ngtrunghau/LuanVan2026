using System;
using System.Collections.Generic;

namespace badmintion.Models;

public partial class ProductReview
{
    public int Id { get; set; }

    public int? ProductId { get; set; }

    public int? CustomerId { get; set; }

    public int? OrderId { get; set; }

    public int? TotalStar { get; set; }

    public string? Comment { get; set; }

    public string? UrlImg { get; set; }

    public bool? IsDeleted { get; set; }

    public DateTime? Date { get; set; }

    public int ModerationStatus { get; set; }

    public string? ModerationReason { get; set; }

    public DateTime? ModeratedAt { get; set; }

    public string? ModeratedBy { get; set; }

    public virtual Product? Product { get; set; }
}

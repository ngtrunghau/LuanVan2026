using System;
using System.Collections.Generic;

namespace badmintion.Models;

public partial class Promotion
{
    public int Id { get; set; }

    public string? Descriptions { get; set; }

    public string? DiscountType { get; set; }

    public decimal? DiscountValue { get; set; }

    public decimal? MinOrderValue { get; set; }

    public string? Code { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public int? MaxUsage { get; set; }

    public int? UsedCount { get; set; }

    public bool? IsDeleted { get; set; }
}

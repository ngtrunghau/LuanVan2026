using System;
using System.Collections.Generic;

namespace badmintion.Models;

public partial class SalesReport
{
    public int Id { get; set; }

    public DateTime? ReportDate { get; set; }

    public decimal? TotalSales { get; set; }

    public string? Period { get; set; }

    public bool? IsDeleted { get; set; }
}

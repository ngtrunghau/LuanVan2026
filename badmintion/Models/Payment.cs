using System;
using System.Collections.Generic;

namespace badmintion.Models;

public partial class Payment
{
    public int Id { get; set; }

    public string? PaymentMethod { get; set; }

    public string? PaymentStatus { get; set; }

    public string? BillId { get; set; }

    public int? OrdersId { get; set; }

    public bool? IsDeleted { get; set; }

    public virtual Order? Orders { get; set; }
}

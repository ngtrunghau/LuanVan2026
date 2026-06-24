using System;
using System.Collections.Generic;

namespace badmintion.Models;

public partial class ShippingDetail
{
    public int Id { get; set; }

    public int? Status { get; set; }

    public DateTime? DateShip { get; set; }

    public int? OrdersId { get; set; }

    public bool? IsDeleted { get; set; }

    public string? Note { get; set; }

    public string? ChangedBy { get; set; }

    public string? ChangedByType { get; set; }

    public virtual Order? Orders { get; set; }
}

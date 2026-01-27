using System;
using System.Collections.Generic;

namespace badmintion.Models;

public partial class InventoryAlert
{
    public int Id { get; set; }

    public int? AlertThreshold { get; set; }

    public int? ProductsId { get; set; }

    public bool? IsDeleted { get; set; }

    public virtual Product? Products { get; set; }
}

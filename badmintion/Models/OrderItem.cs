using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace badmintion.Models;

public partial class OrderItem
{
    public int Id { get; set; }

    public int? Quantity { get; set; }

    public decimal? Price { get; set; }

    public int? ProductsId { get; set; }

    public int? Orders { get; set; }

    public bool? IsDeleted { get; set; }
    [JsonIgnore]
    public virtual Order? OrdersNavigation { get; set; }
    //[JsonIgnore]
    public virtual Product? Products { get; set; }
}

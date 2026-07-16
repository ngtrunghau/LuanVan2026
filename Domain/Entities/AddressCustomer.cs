using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace badmintion.Models;

public partial class AddressCustomer
{
    public int Id { get; set; }

    public string Address { get; set; } = null!;

    public int? ProvinceId { get; set; }

    public int? DistrictId { get; set; }

    public int? TownId { get; set; }

    public int? CustomerId { get; set; }

    public bool? IsDeleted { get; set; }
    [JsonIgnore]
    public virtual Customer? Customer { get; set; }

    public virtual District? District { get; set; }
    [JsonIgnore]
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual Province? Province { get; set; }

    public virtual Town? Town { get; set; }
}

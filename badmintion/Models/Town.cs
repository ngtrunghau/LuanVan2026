using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace badmintion.Models;

public partial class Town
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int? DistrictId { get; set; }
    [JsonIgnore]
    public virtual ICollection<AddressCustomer> AddressCustomers { get; set; } = new List<AddressCustomer>();
    [JsonIgnore]
    public virtual District? District { get; set; }
}

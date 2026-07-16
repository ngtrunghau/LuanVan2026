using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace badmintion.Models;

public partial class District
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int? ProvinceId { get; set; }
    [JsonIgnore]
    public virtual ICollection<AddressCustomer> AddressCustomers { get; set; } = new List<AddressCustomer>();
    [JsonIgnore]
    public virtual Province? Province { get; set; }
    [JsonIgnore]
    public virtual ICollection<Town> Towns { get; set; } = new List<Town>();
}

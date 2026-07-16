using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace badmintion.Models;

public partial class Province
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;
    [JsonIgnore]
    public virtual ICollection<AddressCustomer> AddressCustomers { get; set; } = new List<AddressCustomer>();

    public virtual ICollection<District> Districts { get; set; } = new List<District>();
}

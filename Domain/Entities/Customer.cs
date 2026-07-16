using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace badmintion.Models;

public partial class Customer
{
    public int Id { get; set; }

    public string FullName { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string UserName { get; set; } = null!;

    [System.Text.Json.Serialization.JsonIgnore]
    public string Password { get; set; } = null!;

    public bool? IsDeleted { get; set; }

    public DateTime? PasswordChangedAt { get; set; }

    public virtual ICollection<AddressCustomer> AddressCustomers { get; set; } = new List<AddressCustomer>();
    [JsonIgnore]
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}

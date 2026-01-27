using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace badmintion.Models;

public partial class Order
{
    public int Id { get; set; }

    public DateTime? OrderDate { get; set; }

    public decimal? TotalAmount { get; set; }

    public int? Status { get; set; }

    public int? CustomerId { get; set; }

    public bool? IsDeleted { get; set; }

    public int? AddressId { get; set; }

    public string? TxnRef { get; set; }
   
    public virtual AddressCustomer? Address { get; set; }

    public virtual Customer? Customer { get; set; }

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
    [JsonIgnore]
    public virtual ICollection<ShippingDetail> ShippingDetails { get; set; } = new List<ShippingDetail>();
}

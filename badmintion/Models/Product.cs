using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace badmintion.Models;

public partial class Product
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Descriptions { get; set; }

    public decimal? Price { get; set; }

    public int? StockQuantity { get; set; }

    public string? ImageUrl { get; set; }

    public string? Color { get; set; }

    public int? CategoriesId { get; set; }

    public bool? IsDeleted { get; set; }

    public virtual Category? Categories { get; set; }

    public virtual ICollection<HistoryImport> HistoryImports { get; set; } = new List<HistoryImport>();

    public virtual ICollection<InventoryAlert> InventoryAlerts { get; set; } = new List<InventoryAlert>();
    [JsonIgnore]
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual ICollection<ProductReview> ProductReviews { get; set; } = new List<ProductReview>();

    public virtual ICollection<WareHouse> WareHouses { get; set; } = new List<WareHouse>();
}

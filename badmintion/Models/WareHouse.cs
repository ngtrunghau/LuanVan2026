using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace badmintion.Models;

public partial class WareHouse
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public int? ProductId { get; set; }

    public int QuantityImport { get; set; }

    public int? RemainQuantity { get; set; }

    public bool? IsDeleted { get; set; }
    [JsonIgnore]
    public virtual ICollection<HistoryImport> HistoryImports { get; set; } = new List<HistoryImport>();
    [JsonIgnore]
    public virtual Product? Product { get; set; }
}

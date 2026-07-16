using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace badmintion.Models;

public partial class HistoryImport
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public DateTime? DateImport { get; set; }

    public int? ProductId { get; set; }

    public int? QuantityImport { get; set; }

    public int? WareHouseId { get; set; }

    public bool? IsDeleted { get; set; }
    [JsonIgnore]
    public virtual Product? Product { get; set; }

    [JsonIgnore]
    public virtual WareHouse? WareHouse { get; set; }
}

using System;
using System.Collections.Generic;

namespace badmintion.Models;

public partial class ControllerManage
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public bool? IsDeleted { get; set; }

    public string? Key { get; set; }

    public virtual ICollection<FunctionManage> FunctionManages { get; set; } = new List<FunctionManage>();
}

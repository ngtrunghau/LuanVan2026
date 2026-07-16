using System;
using System.Collections.Generic;

namespace badmintion.Models;

public partial class FunctionManage
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Router { get; set; } = null!;

    public int ControllerId { get; set; }

    public bool? IsDeleted { get; set; }

    public int UnitRoleId { get; set; }

    public string? Key { get; set; }

    public virtual ControllerManage Controller { get; set; } = null!;

    public virtual UnitRole UnitRole { get; set; } = null!;
}

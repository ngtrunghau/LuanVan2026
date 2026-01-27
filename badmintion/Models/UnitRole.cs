using System;
using System.Collections.Generic;

namespace badmintion.Models;

public partial class UnitRole
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public bool? IsDeleted { get; set; }

    public virtual ICollection<FunctionManage> FunctionManages { get; set; } = new List<FunctionManage>();

    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}

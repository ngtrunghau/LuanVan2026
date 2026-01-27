using System;
using System.Collections.Generic;

namespace badmintion.Models;

public partial class User
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? UserName { get; set; }

    public string? Password { get; set; }

    public bool? IsDeleted { get; set; }

    public int? UnitRoleId { get; set; }

    public virtual ICollection<Login> Logins { get; set; } = new List<Login>();

    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

    public virtual UnitRole? UnitRole { get; set; }
}

using System;
using System.Collections.Generic;

namespace badmintion.Models;

public partial class RefreshToken
{
    public int Id { get; set; }

    public bool? IsDeleted { get; set; }

    public string? AccessToken { get; set; }

    public string? RefreshToken1 { get; set; }

    public string? JwtId { get; set; }

    public DateTime? CreationDatetoken { get; set; }

    public DateTime ExpiryDatetoken { get; set; }

    public double? Expirydaterefreshtoken { get; set; }

    public bool? Invalidated { get; set; }

    public int? UnitRoleId { get; set; }

    public int? UserId { get; set; }

    public virtual UnitRole? UnitRole { get; set; }

    public virtual User? User { get; set; }
}

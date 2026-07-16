using System;
using System.Collections.Generic;

namespace badmintion.Models;

public partial class Login
{
    public int Id { get; set; }

    public bool? IsDeleted { get; set; }

    public string? AccessToken { get; set; }

    public string? RefreshToken { get; set; }

    public DateTime? Date { get; set; }

    public int? UserId { get; set; }

    public virtual User? User { get; set; }
}

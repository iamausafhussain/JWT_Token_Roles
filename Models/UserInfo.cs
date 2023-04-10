using System;
using System.Collections.Generic;

namespace JWT_Role.Models;

public partial class UserInfo
{
    public int Userid { get; set; }

    public string? Username { get; set; }

    public string? Password { get; set; }

    public string? Role { get; set; }
}

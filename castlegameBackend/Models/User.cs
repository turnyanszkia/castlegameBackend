using System;
using System.Collections.Generic;

namespace castlegameBackend.Models;

public partial class User
{
    public int Id { get; set; }

    public string LoginNev { get; set; } = null!;

    public string Hash { get; set; } = null!;

    public string Salt { get; set; } = null!;

    public string Name { get; set; } = null!;

    public int PermissionId { get; set; }

    public int Active { get; set; }

    public string Email { get; set; } = null!;

    public string ProfilePicturePath { get; set; } = null!;
}

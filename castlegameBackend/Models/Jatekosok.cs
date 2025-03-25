using System;
using System.Collections.Generic;

namespace castlegameBackend.Models;

public partial class Jatekosok
{
    public int Id { get; set; }

    public string Nev { get; set; } = null!;

    public int Helyezes { get; set; }

    public int Pont { get; set; }
}

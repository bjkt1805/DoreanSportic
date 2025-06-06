using System;
using System.Collections.Generic;

namespace DoreanSportic.Infrastructure.Models;

public partial class Sexo
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public virtual ICollection<Cliente> Cliente { get; set; } = new List<Cliente>();
}

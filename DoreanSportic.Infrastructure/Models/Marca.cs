using System;
using System.Collections.Generic;

namespace DoreanSportic.Infrastructure.Models;

public partial class Marca
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public byte[]? Foto { get; set; }

    public bool Estado { get; set; }

    public virtual ICollection<Producto> Producto { get; set; } = new List<Producto>();
}

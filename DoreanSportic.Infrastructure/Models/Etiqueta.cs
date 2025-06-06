using System;
using System.Collections.Generic;

namespace DoreanSportic.Infrastructure.Models;

public partial class Etiqueta
{
    public int Id { get; set; }

    public string? Nombre { get; set; }

    public bool Estado { get; set; }

    public virtual ICollection<Producto> IdProducto { get; set; } = new List<Producto>();
}

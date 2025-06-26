using System;
using System.Collections.Generic;

namespace DoreanSportic.Infrastructure.Models;

public partial class Categoria
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public bool Estado { get; set; }

    public virtual ICollection<Producto> Producto { get; set; } = new List<Producto>();

    public virtual ICollection<Promocion> IdPromocion { get; set; } = new List<Promocion>();
}

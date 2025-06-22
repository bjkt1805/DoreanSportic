using System;
using System.Collections.Generic;

namespace DoreanSportic.Infrastructure.Models;

public partial class MetodoPago
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public bool Estado { get; set; }

    public virtual ICollection<Pedido> Pedido { get; set; } = new List<Pedido>();
}

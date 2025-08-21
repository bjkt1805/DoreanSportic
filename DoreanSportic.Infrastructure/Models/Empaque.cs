using System;
using System.Collections.Generic;

namespace DoreanSportic.Infrastructure.Models;

public partial class Empaque
{
    public int Id { get; set; }

    public string TipoEmpaque { get; set; } = null!;

    public decimal? PrecioBase { get; set; }

    public byte[]? Imagen { get; set; }

    public bool Estado { get; set; }

    public virtual ICollection<PedidoDetalle> PedidoDetalle { get; set; } = new List<PedidoDetalle>();
}

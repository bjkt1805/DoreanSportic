using System;
using System.Collections.Generic;

namespace DoreanSportic.Infrastructure.Models;

public partial class Carrito
{
    public int Id { get; set; }

    public int IdCliente { get; set; }

    public DateTime FechaCreacion { get; set; }

    public string EstadoPago { get; set; } = null!;

    public bool Estado { get; set; }

    public virtual ICollection<CarritoDetalle> CarritoDetalle { get; set; } = new List<CarritoDetalle>();

    public virtual Cliente IdClienteNavigation { get; set; } = null!;
}

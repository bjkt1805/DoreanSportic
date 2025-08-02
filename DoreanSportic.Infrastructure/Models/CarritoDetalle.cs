using System;
using System.Collections.Generic;

namespace DoreanSportic.Infrastructure.Models;

public partial class CarritoDetalle
{
    public int Id { get; set; }

    public int IdCarrito { get; set; }

    public int IdProducto { get; set; }

    public int IdEmpaque { get; set; }

    public int Cantidad { get; set; }

    public byte[]? Foto { get; set; }

    public string? MensajePersonalizado { get; set; }

    public bool Estado { get; set; }

    public virtual Carrito IdCarritoNavigation { get; set; } = null!;

    public virtual Empaque IdEmpaqueNavigation { get; set; } = null!;

    public virtual Producto IdProductoNavigation { get; set; } = null!;
}

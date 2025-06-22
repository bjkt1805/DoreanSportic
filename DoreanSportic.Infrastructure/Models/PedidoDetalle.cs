using System;
using System.Collections.Generic;

namespace DoreanSportic.Infrastructure.Models;

public partial class PedidoDetalle
{
    public int Id { get; set; }

    public string IdPedido { get; set; } = null!;

    public int IdProducto { get; set; }

    public int IdEmpaque { get; set; }

    public int Cantidad { get; set; }

    public bool Estado { get; set; }

    public virtual Empaque IdEmpaqueNavigation { get; set; } = null!;

    public virtual Pedido IdPedidoNavigation { get; set; } = null!;

    public virtual Producto IdProductoNavigation { get; set; } = null!;
}

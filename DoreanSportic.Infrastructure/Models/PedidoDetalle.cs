using System;
using System.Collections.Generic;

namespace DoreanSportic.Infrastructure.Models;

public partial class PedidoDetalle
{
    public int Id { get; set; }

    public int IdPedido { get; set; }

    public int IdProducto { get; set; }

    public int? IdEmpaque { get; set; }

    public int Cantidad { get; set; }

    public byte[]? Foto { get; set; }

    public string? MensajePersonalizado { get; set; }

    public decimal? SubTotal { get; set; }

    public bool Estado { get; set; }

    public virtual Empaque? IdEmpaqueNavigation { get; set; }

    public virtual Pedido IdPedidoNavigation { get; set; } = null!;

    public virtual Producto IdProductoNavigation { get; set; } = null!;
}

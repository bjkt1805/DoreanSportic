using System;
using System.Collections.Generic;

namespace DoreanSportic.Infrastructure.Models;

public partial class Pedido
{
    public int Id { get; set; }

    public string? NumFactura { get; set; }

    public int IdCliente { get; set; }

    public DateTime FechaPedido { get; set; }

    public string? EstadoPedido { get; set; }

    public decimal? SubTotal { get; set; }

    public decimal? Impuesto { get; set; }

    public decimal? Total { get; set; }

    public int? IdMetodoPago { get; set; }

    public bool Estado { get; set; }

    public virtual Cliente IdClienteNavigation { get; set; } = null!;

    public virtual MetodoPago? IdMetodoPagoNavigation { get; set; }

    public virtual ICollection<PedidoDetalle> PedidoDetalle { get; set; } = new List<PedidoDetalle>();
}

using DoreanSportic.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoreanSportic.Application.DTOs;

public record PedidoDTO
{
    public int Id { get; set; }

    public int IdCliente { get; set; }

    public DateTime FechaPedido { get; set; }

    public string EstadoPedido { get; set; } = null!;

    public decimal SubTotal { get; set; }

    public decimal Impuesto { get; set; }

    public decimal Total { get; set; }

    public int? IdMetodoPago { get; set; }

    public bool Estado { get; set; }

    public virtual Cliente IdClienteNavigation { get; set; } = null!;

    public virtual MetodoPago? IdMetodoPagoNavigation { get; set; }

    public virtual ICollection<PedidoDetalle> PedidoDetalle { get; set; } = new List<PedidoDetalle>();
}

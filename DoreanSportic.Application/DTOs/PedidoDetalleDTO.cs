using DoreanSportic.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoreanSportic.Application.DTOs;

public record PedidoDetalleDTO
{
    public int Id { get; set; }

    public int IdPedido { get; set; }

    public int IdProducto { get; set; }

    public int IdEmpaque { get; set; }

    public int Cantidad { get; set; }

    public bool Estado { get; set; }

    public virtual Empaque IdEmpaqueNavigation { get; set; } = null!;

    public virtual Pedido IdPedidoNavigation { get; set; } = null!;

    public virtual Producto IdProductoNavigation { get; set; } = null!;
}

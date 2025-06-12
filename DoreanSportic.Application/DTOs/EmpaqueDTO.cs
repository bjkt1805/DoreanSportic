using DoreanSportic.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoreanSportic.Application.DTOs;

public record EmpaqueDTO
{
    public int Id { get; set; }

    public string TipoEmpaque { get; set; } = null!;

    public string? Observaciones { get; set; }

    public byte[]? Foto { get; set; }

    public decimal? PrecioBase { get; set; }

    public bool Estado { get; set; }

    public virtual ICollection<CarritoDetalle> CarritoDetalle { get; set; } = new List<CarritoDetalle>();

    public virtual ICollection<PedidoDetalle> PedidoDetalle { get; set; } = new List<PedidoDetalle>();
}

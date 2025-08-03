using DoreanSportic.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoreanSportic.Application.DTOs;

public record CarritoDTO
{
    public int Id { get; set; }

    public int? IdCliente { get; set; }

    public DateTime FechaCreacion { get; set; }

    public string EstadoPago { get; set; } = null!;

    public bool Estado { get; set; }

    public virtual ICollection<CarritoDetalle> CarritoDetalle { get; set; } = new List<CarritoDetalle>();

    public virtual Cliente IdClienteNavigation { get; set; } = null!;
}
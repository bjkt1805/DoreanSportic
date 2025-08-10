using DoreanSportic.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoreanSportic.Application.DTOs;

public record TarjetaDTO
{
    public int Id { get; set; }

    public string IdCliente { get; set; } = null!;

    public string NumeroEnmascarado { get; set; } = null!;

    public string NombreTitular { get; set; } = null!;

    public DateOnly FechaExpiracion { get; set; }

    public string? TipoTarjeta { get; set; }

    public virtual Cliente IdClienteNavigation { get; set; } = null!;
}

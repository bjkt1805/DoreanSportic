using System;
using System.Collections.Generic;

namespace DoreanSportic.Infrastructure.Models;

public partial class Tarjeta
{
    public int Id { get; set; }

    public int IdCliente { get; set; }

    public string NumeroEnmascarado { get; set; } = null!;

    public string NombreTitular { get; set; } = null!;

    public DateOnly FechaExpiracion { get; set; }

    public string? TipoTarjeta { get; set; }

    public virtual Cliente IdClienteNavigation { get; set; } = null!;
}

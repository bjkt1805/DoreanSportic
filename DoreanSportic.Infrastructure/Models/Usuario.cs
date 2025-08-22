using System;
using System.Collections.Generic;

namespace DoreanSportic.Infrastructure.Models;

public partial class Usuario
{
    public int Id { get; set; }

    public int IdCliente { get; set; }

    public string UserName { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public DateTime FechaRegistro { get; set; }

    public int IdRol { get; set; }

    public DateTime? UltimoInicioSesionUtc { get; set; }

    public bool Estado { get; set; }

    public virtual Cliente IdClienteNavigation { get; set; } = null!;

    public virtual Rol IdRolNavigation { get; set; } = null!;

    public virtual ICollection<ResennaValoracion> ResennaValoracion { get; set; } = new List<ResennaValoracion>();
}

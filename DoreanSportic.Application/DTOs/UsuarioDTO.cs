using DoreanSportic.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoreanSportic.Application.DTOs;

public record UsuarioDTO
{
    public int Id { get; set; }

    public int IdCliente { get; set; }

    public string UserName { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public DateTime FechaRegistro { get; set; }

    public bool EsActivo { get; set; }

    public DateTime? UltimoInicioSesionUtc { get; set; }

    public int IdRol { get; set; }

    public bool Estado { get; set; }

    public virtual Cliente IdClienteNavigation { get; set; } = null!;

    public virtual Rol IdRolNavigation { get; set; } = null!;

    public virtual ICollection<ResennaValoracion> ResennaValoracion { get; set; } = new List<ResennaValoracion>();
}

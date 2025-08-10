using DoreanSportic.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoreanSportic.Application.DTOs;

public record ClienteDTO
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string Apellido { get; set; } = null!;

    public string Email { get; set; } = null!;

    public int? IdSexo { get; set; }

    public string? Telefono { get; set; }

    public string? DireccionEnvio { get; set; }

    public bool Estado { get; set; }

    public virtual Sexo? IdSexoNavigation { get; set; }

    public virtual ICollection<Pedido> Pedido { get; set; } = new List<Pedido>();

    public virtual ICollection<Tarjeta> Tarjeta { get; set; } = new List<Tarjeta>();

    public virtual Usuario? Usuario { get; set; }
}

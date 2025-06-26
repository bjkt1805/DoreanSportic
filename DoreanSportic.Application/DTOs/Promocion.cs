using DoreanSportic.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoreanSportic.Application.DTOs;

public record PromocionDTO
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }

    public decimal? PorcentajeDescuento { get; set; }

    public decimal? DescuentoFijo { get; set; }

    public DateTime FechaInicio { get; set; }

    public DateTime FechaFin { get; set; }

    public bool Estado { get; set; }

    public virtual ICollection<Categoria> IdCategoria { get; set; } = new List<Categoria>();

    public virtual ICollection<Producto> IdProducto { get; set; } = new List<Producto>();
}

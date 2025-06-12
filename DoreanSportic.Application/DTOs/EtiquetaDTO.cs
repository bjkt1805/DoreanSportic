using DoreanSportic.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoreanSportic.Application.DTOs;

public record EtiquetaDTO
{
    public int Id { get; set; }

    public string? Nombre { get; set; }

    public bool Estado { get; set; }

    public virtual ICollection<Producto> IdProducto { get; set; } = new List<Producto>();
}

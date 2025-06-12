using System;
using System.Collections.Generic;

namespace DoreanSportic.Infrastructure.Models;

public partial class ResennaValoracionDTO
{
    public int Id { get; set; }

    public int IdCliente { get; set; }

    public int IdProducto { get; set; }

    public int Calificacion { get; set; }

    public string? Comentario { get; set; }

    public DateTime FechaResenna { get; set; }

    public bool Estado { get; set; }

    public virtual Cliente IdClienteNavigation { get; set; } = null!;

    public virtual Producto IdProductoNavigation { get; set; } = null!;
}

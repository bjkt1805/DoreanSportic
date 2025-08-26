using System;
using System.Collections.Generic;

namespace DoreanSportic.Infrastructure.Models;

public partial class ResennaValoracion
{
    public int Id { get; set; }

    public int IdUsuario { get; set; }

    public int IdProducto { get; set; }

    public int Calificacion { get; set; }

    public string? Comentario { get; set; }

    public DateTime FechaResenna { get; set; }

    public bool? Reportada { get; set; }

    public string? ObservacionReporte { get; set; }

    public bool Estado { get; set; }

    public virtual Producto IdProductoNavigation { get; set; } = null!;

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;
}

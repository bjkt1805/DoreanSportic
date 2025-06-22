using System;
using System.Collections.Generic;

namespace DoreanSportic.Infrastructure.Models;

public partial class ImagenProducto
{
    public int Id { get; set; }

    public int IdProducto { get; set; }

    public byte[]? Imagen { get; set; }

    public string? Descripcion { get; set; }

    public bool Estado { get; set; }

    public virtual Producto IdProductoNavigation { get; set; } = null!;
}

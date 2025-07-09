using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoreanSportic.Infrastructure.Models;

public partial class Producto
{

    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }

    public decimal PrecioBase { get; set; }

    public int Stock { get; set; }

    public int IdMarca { get; set; }

    public int IdCategoria { get; set; }

    public bool Estado { get; set; }

    public virtual ICollection<CarritoDetalle> CarritoDetalle { get; set; } = new List<CarritoDetalle>();

    public virtual Categoria IdCategoriaNavigation { get; set; } = null!;

    public virtual Marca IdMarcaNavigation { get; set; } = null!;

    public virtual ICollection<PedidoDetalle> PedidoDetalle { get; set; } = new List<PedidoDetalle>();

    public virtual ICollection<ResennaValoracion> ResennaValoracion { get; set; } = new List<ResennaValoracion>();

    public virtual ICollection<Etiqueta> IdEtiqueta { get; set; } = new List<Etiqueta>();

    public virtual ICollection<Promocion> IdPromocion { get; set; } = new List<Promocion>();

    // Aplicar data notation NotMapped para hacerle entender a EF Core que esta propiedad no es mapeada de la base de datos.
    //[NotMapped]
    //public byte[]? PrimeraImagen { get; set; }

    [NotMapped]
    public byte[]? PrimeraImagen => ImagenesProducto?.FirstOrDefault()?.Imagen;

    public virtual ICollection<ImagenProducto> ImagenesProducto { get; set; } = new List<ImagenProducto>();
}

using DoreanSportic.Infrastructure.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using DoreanSportic.Application.DTOs;

namespace DoreanSportic.Application.DTOs;

public record ProductoDTO
{
    public int Id { get; set; }

    [Required(
        AllowEmptyStrings = false,
        ErrorMessageResourceType = typeof(Resources.ProductoDTO),
        ErrorMessageResourceName = "NombreRequerido")]
    [StringLength(200,
        ErrorMessageResourceType = typeof(Resources.ProductoDTO),
        ErrorMessageResourceName = "NombreLongitud")]
    public string Nombre { get; set; } = null!;

    [Required(
        AllowEmptyStrings = false,
        ErrorMessageResourceType = typeof(Resources.ProductoDTO),
        ErrorMessageResourceName = "DescripcionRequerida")]
    [StringLength(500,
        ErrorMessageResourceType = typeof(Resources.ProductoDTO),
        ErrorMessageResourceName = "DescripcionLongitud")]
    public string? Descripcion { get; set; }

    [DisplayFormat(DataFormatString = "{0:C0}")]
    [Required(
        ErrorMessageResourceType = typeof(Resources.ProductoDTO),
        ErrorMessageResourceName = "PrecioBaseRequerido")]
    [Range(5000, 100000.00,
        ErrorMessageResourceType = typeof(Resources.ProductoDTO),
        ErrorMessageResourceName = "PrecioBaseRango")]
    [RegularExpression(@"^\d+(\.\d{1,2})?$",
        ErrorMessageResourceType = typeof(Resources.ProductoDTO),
        ErrorMessageResourceName = "PrecioBaseDecimales")]
    public decimal PrecioBase { get; set; }

    [Required(
         ErrorMessageResourceType = typeof(Resources.ProductoDTO),
         ErrorMessageResourceName = "StockRequerido")]
    [RegularExpression(@"^\d+$",
         ErrorMessageResourceType = typeof(Resources.ProductoDTO),
         ErrorMessageResourceName = "StockNumerico")]
    [Range(0, 100,
         ErrorMessageResourceType = typeof(Resources.ProductoDTO),
         ErrorMessageResourceName = "StockRango")]
    public int Stock { get; set; }

    [Required(ErrorMessage = " {0} es requerida ")]
    public int IdMarca { get; set; }

    [Required(ErrorMessage = " {0} es requerida ")]
    public int IdCategoria { get; set; }

    public bool Estado { get; set; }

    [ValidateNever]
    public virtual ICollection<CarritoDetalle> CarritoDetalle { get; set; } = new List<CarritoDetalle>();

    [ValidateNever]
    public virtual Categoria IdCategoriaNavigation { get; set; } = null!;

    [ValidateNever]
    public virtual Marca IdMarcaNavigation { get; set; } = null!;

    [ValidateNever]
    public virtual ICollection<PedidoDetalle> PedidoDetalle { get; set; } = new List<PedidoDetalle>();

    public virtual ICollection<ResennaValoracion> ResennaValoracion { get; set; } = new List<ResennaValoracion>();

    public virtual ICollection<Etiqueta> IdEtiqueta { get; set; } = new List<Etiqueta>();

    public virtual ICollection<Promocion> IdPromocion { get; set; } = new List<Promocion>();

    public byte[]? PrimeraImagen { get; set; }

    [Required(
            ErrorMessageResourceType = typeof(Resources.ProductoDTO),
            ErrorMessageResourceName = "ImagenesProductoRequerido")]
    public virtual ICollection<ImagenProducto> ImagenesProducto { get; set; } = new List<ImagenProducto>();
}


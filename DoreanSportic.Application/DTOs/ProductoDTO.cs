using DoreanSportic.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace DoreanSportic.Application.DTOs;

public record ProductoDTO
{
    public int Id { get; set; }

    [Display(Name = "Nombre del producto")]
    [Required(AllowEmptyStrings = false, ErrorMessage = " {0} es requerido ")]
    [StringLength(200, ErrorMessage = "El nombre no puede exceder los 200 caracteres.")]
    public string Nombre { get; set; } = null!;

    [Display(Name = "Descripción del producto")]
    [Required(AllowEmptyStrings = false, ErrorMessage = " {0} es requerida ")]
    [StringLength(500, ErrorMessage = "La descripción no puede exceder los 500 caracteres.")]
    public string? Descripcion { get; set; }

    [Display(Name = "Precio Base")]
    [DisplayFormat(DataFormatString = "{0:C0}")]
    [Required(ErrorMessage = " {0} es requerido ")]
    //[RegularExpression(@"^\d+$", ErrorMessage = "* {0} debe ser númerico *")]
    [Range(5000, 100000.00, ErrorMessage = "  {0} debe estar en el rango de {1} y {2} (hasta dos decimales) ")]
    [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "Máximo dos decimales")]
    public decimal PrecioBase { get; set; }

    [Display(Name = "Cantidad")]
    [Required(ErrorMessage = " {0} es requerida ")]
    [RegularExpression(@"^\d+$", ErrorMessage = "* {0} debe ser númerico ")]
    [Range(0, 100, ErrorMessage = "  {0} debe ser menor a {2} ")]
    public int Stock { get; set; }

    [Display(Name = "Marca")]
    [Required(ErrorMessage = " {0} es requerida ")]
    public int IdMarca { get; set; }

    [Display(Name = "Categoría")]
    [Required(ErrorMessage = " {0} es requerida ")]
    public int IdCategoria { get; set; }

    [Display(Name = "Estado")]
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

    [Required(ErrorMessage = "* Debe insertar al menos una imagen *")]
    public virtual ICollection<ImagenProducto> ImagenesProducto { get; set; } = new List<ImagenProducto>();

}

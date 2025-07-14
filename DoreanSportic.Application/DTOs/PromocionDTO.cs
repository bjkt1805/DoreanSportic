using DoreanSportic.Infrastructure.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoreanSportic.Application.DTOs;

public record PromocionDTO
{
    public int Id { get; set; }

    [Display(Name = "Nombre de la promoción")]
    [Required(AllowEmptyStrings = false, ErrorMessage = " {0} es requerido ")]
    public string Nombre { get; set; } = null!;

    [Display(Name = "Descripción de la promoción")]
    [Required(AllowEmptyStrings = false, ErrorMessage = " {0} es requerida ")]
    public string? Descripcion { get; set; }

    [Display(Name = "Porcentaje de descuento")]
    [Required(ErrorMessage = " {0} es requerido")]
    [Range(1, 100, ErrorMessage = "  {0} debe estar en el rango de {1} y {2} ")]
    public decimal? PorcentajeDescuento { get; set; }

    public decimal? DescuentoFijo { get; set; }

    [Display(Name = "Fecha de inicio")]
    [Required(ErrorMessage = " {0} es requerida ")]
    public DateTime FechaInicio { get; set; }

    [Display(Name = "Fecha final")]
    [Required(ErrorMessage = " {0} es requerida ")]
    public DateTime FechaFin { get; set; }

    public bool Estado { get; set; }

    //[Display(Name = "Categoría")]
    //[Required(ErrorMessage = " {0} es requerida ")]
    public virtual ICollection<Categoria> IdCategoria { get; set; } = new List<Categoria>();

    [Display(Name = "Producto")]
    [Required(ErrorMessage = " {0} es requerido")]
    public virtual ICollection<Producto> IdProducto { get; set; } = new List<Producto>();

    [Display(Name = "Categoría")]
    [Required(ErrorMessage = " {0} es requerida ")]
    // Propiedades auxiliar para idCategoria para poder hacer uso de Select simple en las vistas de Promocion (_CreatePromocion y _EditPromocion)

    [ValidateNever]
    public int? IdCategoriaSeleccionada { get; set; }
}

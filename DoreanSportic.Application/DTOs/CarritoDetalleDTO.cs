using DoreanSportic.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoreanSportic.Application.DTOs;

public record CarritoDetalleDTO
{
    public int Id { get; set; }

    public int IdCarrito { get; set; }

    public int IdProducto { get; set; }
    
    [Display(Name = "Empaque")]
    [Required(ErrorMessage = " {0} es requerido ")]
    public int IdEmpaque { get; set; }

    [Display(Name = "Cantidad")]
    [Required(ErrorMessage = " {0} es requerida ")]
    [RegularExpression(@"^\d+$", ErrorMessage = "* {0} debe ser númerica ")]
    public int Cantidad { get; set; }
    
    [Required(ErrorMessage = "Debe insertar al menos una imagen")]
    public byte[]? Foto { get; set; }

    [Display(Name = "Mensaje personalizado")]
    [Required(AllowEmptyStrings = false, ErrorMessage = " {0} es requerido ")]
    [StringLength(500, ErrorMessage = "El mensaje personalizado no puede exceder los 500 caracteres.")]
    public string? MensajePersonalizado { get; set; }

    public bool Estado { get; set; }

    public virtual Carrito IdCarritoNavigation { get; set; } = null!;

    public virtual Empaque IdEmpaqueNavigation { get; set; } = null!;

    public virtual Producto IdProductoNavigation { get; set; } = null!;
}

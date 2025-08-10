using DoreanSportic.Infrastructure.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoreanSportic.Application.DTOs;

public record PedidoDetalleDTO
{
    public int Id { get; set; }

    public int IdPedido { get; set; }

    public int IdProducto { get; set; }

    //A pesar de tener DataAnnotations, 
    // aquí el empaque, la foto y mensaje personalizado
    // van a tener el data annotation de ValidateNever
    // no va a usarse ya que en el cliente existe Alpine.js
    // y su validación afecta la validación del modelo

    [Display(Name = "Empaque")]
    [Required(ErrorMessage = " {0} es requerido ")]
    [ValidateNever]
    public int? IdEmpaque { get; set; }

    [Display(Name = "Cantidad")]
    [Required(ErrorMessage = " {0} es requerida ")]
    [RegularExpression(@"^\d+$", ErrorMessage = "* {0} debe ser númerica ")]
    public int Cantidad { get; set; }

    // Como el controlador no acepta archivos byte directamente, 
    // Hay que incluir un atributo de tipo IFormFile para recibir la imagen
    // Este atributo se convierte a byte[] en el controlador 
    // y no se mapea a la base de datos

    [NotMapped]
    public IFormFile? FotoArchivo { get; set; }

    [Required(ErrorMessage = "Debe insertar al menos una imagen")]
    [ValidateNever]
    public byte[]? Foto { get; set; }

    [Display(Name = "Mensaje personalizado")]
    [Required(AllowEmptyStrings = false, ErrorMessage = " {0} es requerido ")]
    [StringLength(500, ErrorMessage = "El mensaje personalizado no puede exceder los 500 caracteres.")]
    [ValidateNever]
    public string? MensajePersonalizado { get; set; }
    public decimal SubTotal { get; set; }

    public bool Estado { get; set; }

    [ValidateNever]
    public virtual Pedido IdPedidoNavigation { get; set; } = null!;

    [ValidateNever]
    public virtual Empaque IdEmpaqueNavigation { get; set; } = null!;

    [ValidateNever]
    public virtual Producto IdProductoNavigation { get; set; } = null!;
}

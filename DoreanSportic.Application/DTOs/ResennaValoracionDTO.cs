using DoreanSportic.Infrastructure.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoreanSportic.Application.DTOs;

public partial class ResennaValoracionDTO
{
    public int Id { get; set; }

    public int IdUsuario { get; set; }

    public int IdProducto { get; set; }

    [Required(
    AllowEmptyStrings = false,
    ErrorMessageResourceType = typeof(Resources.ResennaValoracionDTO),
    ErrorMessageResourceName = "CalificacionRequired")]
    [Range(1, 5,
        ErrorMessageResourceType = typeof(Resources.ResennaValoracionDTO),
        ErrorMessageResourceName = "CalificacionRango")]
    public int Calificacion { get; set; }

    [StringLength(500,
        ErrorMessageResourceType = typeof(Resources.ResennaValoracionDTO),
        ErrorMessageResourceName = "ComentarioRango")]
    public string? Comentario { get; set; }

    public DateTime FechaResenna { get; set; }

    public bool? Reportada { get; set; }

    public string? ObservacionReporte { get; set; }

    public bool Estado { get; set; }

    [ValidateNever]
    public virtual Producto IdProductoNavigation { get; set; } = null!;

    [ValidateNever]
    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;
}

using DoreanSportic.Infrastructure.Models;
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

    [Required(ErrorMessage = "La calificación es obligatoria.")]
    [Range(1, 5, ErrorMessage = "La calificación debe estar entre 1 y 5.")]
    public int Calificacion { get; set; }

    [StringLength(500, ErrorMessage = "El comentario no puede exceder los 500 caracteres.")]
    public string? Comentario { get; set; }

    public DateTime FechaResenna { get; set; }

    public bool Estado { get; set; }

    public virtual Producto IdProductoNavigation { get; set; } = null!;

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;
}

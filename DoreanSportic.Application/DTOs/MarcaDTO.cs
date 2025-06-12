using DoreanSportic.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoreanSportic.Application.DTOs;

public record MarcaDTO
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public byte[]? Foto { get; set; }

    public bool Estado { get; set; }

    public virtual ICollection<Producto> Producto { get; set; } = new List<Producto>();
}

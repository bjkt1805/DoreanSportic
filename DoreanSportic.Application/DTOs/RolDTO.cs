using DoreanSportic.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoreanSportic.Application.DTOs;

public record RolDTO
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public bool Estado { get; set; }

    public virtual ICollection<Usuario> Usuario { get; set; } = new List<Usuario>();
}

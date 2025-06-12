using DoreanSportic.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoreanSportic.Application.DTOs;

public record SexoDTO
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public virtual ICollection<Cliente> Cliente { get; set; } = new List<Cliente>();
}

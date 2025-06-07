using DoreanSportic.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Libreria.Application.DTOs
{
    public record CarritoDTO
    {
        public int IdAutor { get; set; }
        public string Nombre { get; set; } = null!;
        public virtual List<Carrito> Libro { get; set; } = new List<Carrito>();
    }
}
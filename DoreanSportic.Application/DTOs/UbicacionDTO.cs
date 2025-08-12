using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoreanSportic.Application.DTOs
{
    public record UbicacionDTO
    {
        public record ProvinciaDTO(int Id, string Nombre);
        public record CantonDTO(int Id, string Nombre, int ProvinciaId);
        public record DistritoDTO(int Id, string Nombre, int ProvinciaId, int CantonId);
    }
}

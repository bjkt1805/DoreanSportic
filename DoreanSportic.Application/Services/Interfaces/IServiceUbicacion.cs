using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DoreanSportic.Application.DTOs.UbicacionDTO;

namespace DoreanSportic.Application.Services.Interfaces
{
    public interface IServiceUbicacion
    {
        Task<IReadOnlyList<ProvinciaDTO>> GetProvinciasAsync();
        Task<IReadOnlyList<CantonDTO>> GetCantonesAsync(int provinciaId);
        Task<IReadOnlyList<DistritoDTO>> GetDistritosAsync(int provinciaId, int cantonId);
    }
}

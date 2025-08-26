using DoreanSportic.Application.DTOs;
using DoreanSportic.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace DoreanSportic.Application.Services.Interfaces
{
    public interface IServiceResennaValoracion
    {
        Task<ICollection<ResennaValoracionDTO>> ListAsync();

        Task<ICollection<ResennaValoracionDTO>> GetResennasPorProducto(int idProducto);

        Task<ResennaValoracionDTO> FindByIdAsync(int id);

        Task<int> AddAsync(ResennaValoracionDTO dto);

        // Método para obtener estadísticas de valoraciones
        Task<ResennaValoracionStatsDTO> GetStatsAsync();

        // Método para reportar una reseña
        Task ReportarAsync(int idResenna, int idUsuarioReporta, string nombreUsuarioReporta, string? observacion);

        // Método para actualizar el estado (activo/inactivo) de una reseña
        Task UpdateEstadoAsync(int id, bool estado);


    }
}
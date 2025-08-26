using DoreanSportic.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoreanSportic.Infrastructure.Repository.Interfaces
{
    public interface IRepositoryResennaValoracion
    {
        Task<ICollection<ResennaValoracion>> ListAsync();
        Task<ICollection<ResennaValoracion>> GetResennasPorProducto(int idProducto, int? calificacion = null, int? take = null);

        Task<ResennaValoracion> FindByIdAsync(int id);

        Task<int> AddAsync(ResennaValoracion entity);

        // Obtener estadísticas de valoraciones
        Task<(int Star5, int Star4, int Star3, int Star2, int Star1, int Total, double Average)> GetStatsAsync();

        // Método para reportar una reseña
        Task ReportarAsync(int idResenna, int idUsuarioReporta, string nombreUsuarioReporta, string? observacion);

        // Método para actualizar el estado (activo/inactivo) de una reseña
        Task UpdateEstadoAsync(int id, bool estado);

        // Método para verificar si un usuario ya valoró un producto
        Task<bool> ExistsByUserProductAsync(int userId, int productId);
    }
}

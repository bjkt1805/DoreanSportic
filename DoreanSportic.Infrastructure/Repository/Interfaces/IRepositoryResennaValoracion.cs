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
        Task<ICollection<ResennaValoracion>> GetResennasPorProducto(int idProducto);

        Task<ResennaValoracion> FindByIdAsync(int id);

        Task<int> AddAsync(ResennaValoracion entity);

        // Nuevo método para obtener estadísticas de valoraciones
        Task<(int Star5, int Star4, int Star3, int Star2, int Star1, int Total, double Average)> GetStatsAsync();
    }
}

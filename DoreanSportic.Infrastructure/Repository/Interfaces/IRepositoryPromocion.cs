using DoreanSportic.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoreanSportic.Infrastructure.Repository.Interfaces
{
    public interface IRepositoryPromocion
    {
        Task<ICollection<Promocion>> ListAsync();
        Task<Promocion> FindByIdAsync(int id);

        Task<int> AddAsync(Promocion entity, List<int> listaProductosSeleccionados);

        Task UpdateAsync(Promocion entity, List<Categoria> nuevasCategorias, List<Producto> nuevosProductos);
        Task<ICollection<Producto>> ObtenerProductosPorIdsAsync(List<int> ids);
        Task<Categoria?> ObtenerCategoriaPorIdAsync(int id);
    }
}

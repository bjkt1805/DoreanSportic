using DoreanSportic.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoreanSportic.Infrastructure.Repository.Interfaces
{
    public interface IRepositoryPedidoDetalle
    {
        Task<ICollection<PedidoDetalle>> ListAsync();
        Task<PedidoDetalle> FindByIdAsync(int id);

        Task<int> AddAsync(PedidoDetalle entity);
        Task<ICollection<PedidoDetalle>> GetDetallesPorPedido(int idProducto);

        Task<List<PedidoDetalle>> GetByPedidoIdAsync(int idPedido);

    }
}

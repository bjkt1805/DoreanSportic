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

        // Méodo para obtener el ID del pedido asociado a un detalle
        Task<int?> GetPedidoIdByDetalleAsync(int detalleId);

        // Método para actualizar la cantidad, subtotal, impuesto y total de un detalle de pedido
        Task UpdateCantidadAsync(int detalleId, int nuevaCantidad, decimal nuevoSub, decimal nuevoImp, decimal nuevoTotal);

        // Método para eliminar un detalle de pedido
        Task RemoveAsync(int detalleId);

    }
}

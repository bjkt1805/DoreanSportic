using DoreanSportic.Application.DTOs;
using DoreanSportic.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace DoreanSportic.Application.Services.Interfaces
{
    public interface IServicePedidoDetalle
    {
        Task<ICollection<PedidoDetalleDTO>> ListAsync();
        Task<PedidoDetalleDTO> FindByIdAsync(int id);

        Task<int> AddAsync(PedidoDetalleDTO dto);
        Task<ICollection<PedidoDetalleDTO>> GetDetallesPorPedido(int idPedido);

        Task<List<PedidoDetalleDTO>> GetByPedidoIdAsync(int idCarrito);

        // Método para actualizar la cantidad de productos en un detalle del pedido
        Task<(PedidoDetalleDTO? det, bool eliminado)> ActualizarCantidadAsync(int detalleId, int nuevaCantidad);

        // Método para eliminar un detalle de pedido
        Task EliminarAsync(int detalleId);

    }
}
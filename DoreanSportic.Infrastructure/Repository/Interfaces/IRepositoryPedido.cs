using DoreanSportic.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoreanSportic.Infrastructure.Repository.Interfaces
{
    public interface IRepositoryPedido
    {
        Task<ICollection<Pedido>> ListAsync();
        Task<Pedido> FindByIdAsync(int id);

        Task<IReadOnlyList<Pedido>> ListByUserAsync(int userId);
        Task<int> AddAsync(Pedido entity);

        // Método para actualizar el encabezado del pedido
        Task UpdateHeaderAsync(int pedidoId, int? idCliente, string? direccionEnvio, int metodoPago);

        // Método para actualizar los totales y el estado de pago del pedido
        Task UpdateTotalsAndStateAsync(int pedidoId, decimal sub, decimal imp, decimal total, string? estadoPago = null);

        // Método para agregar un detalle al pedido
        Task<bool> AnyDetalleAsync(int pedidoId);

        // Método para verificar que el cliente/usuario çompró un producto en el pedido
        Task<bool> UsuarioComproProductoAsync(int userId, int productId);
    }
}

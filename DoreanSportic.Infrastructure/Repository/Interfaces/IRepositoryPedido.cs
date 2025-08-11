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

        Task<int> AddAsync(Pedido entity);

        // Método para actualizar el encabezado del pedido
        Task UpdateHeaderAsync(int pedidoId, int? idCliente, string? direccionEnvio);

        // Método para actualizar los totales y el estado de pago del pedido
        Task UpdateTotalsAndStateAsync(int pedidoId, decimal sub, decimal imp, decimal total, string? estadoPago = null);

        // Método para agregar un detalle al pedido
        Task<bool> AnyDetalleAsync(int pedidoId);
    }
}

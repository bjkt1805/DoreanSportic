using DoreanSportic.Application.DTOs;
using DoreanSportic.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace DoreanSportic.Application.Services.Interfaces
{
    public interface IServicePedido
    {
        Task<ICollection<PedidoDTO>> ListAsync();
        Task<IReadOnlyList<PedidoDTO>> ListByUserAsync(int userId);
        Task<PedidoDTO> FindByIdAsync(int id);

        Task<int> AddAsync(PedidoDTO dto);

        // Método para actualizar el encabezado del pedido
        Task<bool> ActualizarEncabezadoAsync(int pedidoId, int userId, string? direccionEnvio,int metodoPago);

        // Método para actualizar los totales y el estado de pago del pedido
        Task<(decimal sub, decimal imp, decimal total)> RecalcularTotalesAsync(int pedidoId);

        // Método para verificar si un pedido tiene detalles
        Task<(bool ok, List<(int detalleId, string nombre, int stockDisp, int cant)> errores)> ValidarStockAsync(int pedidoId);

        // Confirmar el pedido: descuenta stock + cambia estado
        Task<bool> ConfirmarAsync(int pedidoId); // descuenta stock + cambia estado
    }
}
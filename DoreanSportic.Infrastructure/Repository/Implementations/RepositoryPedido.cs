using DoreanSportic.Infrastructure.Data;
using DoreanSportic.Infrastructure.Models;
using DoreanSportic.Infrastructure.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoreanSportic.Infrastructure.Repository.Implementations
{
    public class RepositoryPedido : IRepositoryPedido
    {
        private readonly DoreanSporticContext _context;
        public RepositoryPedido(DoreanSporticContext context)
        {
            _context = context;
        }
        public async Task<Pedido> FindByIdAsync(int id)
        {
            //Obtener el pedido  (Eager loading con Detalles del pedido)
            var @object = await _context.Pedido
                                .Where(x => x.Id == id)
                                .Include(c => c.PedidoDetalle)
                                    .ThenInclude(d => d.IdProductoNavigation)
                                 .Include(p => p.IdClienteNavigation)
                                    .ThenInclude(c => c.Usuario)
                                 .Include(p => p.IdMetodoPagoNavigation)
                                .FirstAsync();
            return @object!;
        }
        public async Task<ICollection<Pedido>> ListAsync()
        {
            //Select * from Pedido
            var collection = await _context.Set<Pedido>()
                .Where(p => p.IdCliente != null && p.Estado == true)
                .ToListAsync();
            return collection;
        }

        public async Task<IReadOnlyList<Pedido>> ListByUserAsync(int userId)
        {
            return await _context.Pedido
                .Where(p => p.IdCliente == userId)
                .OrderByDescending(p => p.FechaPedido)
                .ToListAsync();
        }
        public async Task<int> AddAsync(Pedido entity)
        {

            // Añadir el pedido a la base de datos
            await _context.Set<Pedido>().AddAsync(entity);

            // Para debuggear los cambios que va a realizar EF
            // antes de salvar los cambios (Ej: borrar entidedes, agregar campos, etc)

            var entries = _context.ChangeTracker.Entries();

            foreach (var entry in entries)
            {
                Console.WriteLine($"Entidad: {entry.Entity.GetType().Name}, Estado: {entry.State}");
            }

            try
            {
                await _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                // Para loggear la excepció al enviar
                // datos a la base de datos
                var inner = ex.InnerException;
                var innerMessage = inner?.Message ?? ex.Message;

                // Imprimir en consola el error
                Console.WriteLine("Error al guardar en base de datos: " + innerMessage);
            }
            return entity.Id;
        }

        // Metodo para actualizar el encabezado del pedido
        public async Task UpdateHeaderAsync(int pedidoId, int? idCliente, string? direccionEnvio, int metodoPago)
        {
            // Obtener el pedido por id
            var pedido = await _context.Pedido.FirstAsync(p => p.Id == pedidoId);

            // Si el idCliente tiene valor, asignarlo al pedido
            if (idCliente.HasValue) pedido.IdCliente = idCliente.Value;

            // Asignar la dirección de envío al pedido
            pedido.DireccionEnvio = direccionEnvio;

            // Asignar el Id de metodo de pago
            pedido.IdMetodoPago = metodoPago;

            // Para debuggear los cambios que va a realizar EF
            // antes de salvar los cambios (Ej: borrar entidedes, agregar campos, etc)

            var entries = _context.ChangeTracker.Entries();

            foreach (var entry in entries)
            {
                Console.WriteLine($"Entidad: {entry.Entity.GetType().Name}, Estado: {entry.State}");
            }

            try
            {
                await _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                // Para loggear la excepció al enviar
                // datos a la base de datos
                var inner = ex.InnerException;
                var innerMessage = inner?.Message ?? ex.Message;

                // Imprimir en consola el error
                Console.WriteLine("Error al guardar en base de datos: " + innerMessage);
            }
        }

        // Metodo para actualizar los totales y el estado de pago del pedido
        public async Task UpdateTotalsAndStateAsync(int pedidoId, decimal sub, decimal imp, decimal total, string? estadoPago = null)
        {
            // Obtener el pedido por id 
            var pedido = await _context.Pedido.FirstAsync(p => p.Id == pedidoId);

            // Actualizar el subtotal del pedido
            pedido.SubTotal = sub; 

            // Si el estado de pago no es nulo o vacío, actualizarlo
            if (!string.IsNullOrWhiteSpace(estadoPago)) pedido.EstadoPago = estadoPago;
            var entries = _context.ChangeTracker.Entries();

            foreach (var entry in entries)
            {
                Console.WriteLine($"Entidad: {entry.Entity.GetType().Name}, Estado: {entry.State}");
            }

            try
            {
                await _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                // Para loggear la excepció al enviar
                // datos a la base de datos
                var inner = ex.InnerException;
                var innerMessage = inner?.Message ?? ex.Message;

                // Imprimir en consola el error
                Console.WriteLine("Error al guardar en base de datos: " + innerMessage);
            }
        }

        // Método para verificar si un pedido tiene detalles
        public async Task<bool> AnyDetalleAsync(int pedidoId)
        {
            // Verificar si existe algún detalle de pedido asociado al pedidoId
            return await _context.PedidoDetalle.AnyAsync(d => d.IdPedido == pedidoId);
        }

        // Método para verificar que el cliente/usuario compró un producto en el pedido
        public async Task<bool> UsuarioComproProductoAsync(int userId, int productId)
        {
            // Considerar “Pagado” como compra realizada;
            return await _context.Pedido
                .Where(p => p.IdCliente == userId
                            && p.Estado == true
                            && p.EstadoPago == "Pagado")
                .AnyAsync(p => p.PedidoDetalle.Any(d => d.IdProducto == productId));
        }

    }


}

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
    public class RepositoryPedidoDetalle : IRepositoryPedidoDetalle
    {
        private readonly DoreanSporticContext _context;
        public RepositoryPedidoDetalle(DoreanSporticContext context)
        {
            _context = context;
        }
        public Task<PedidoDetalle> FindByIdAsync(int id)
        {
            throw new NotImplementedException();
        }
        public async Task<ICollection<PedidoDetalle>> ListAsync()
        {
            //Select * from Carrito
            var collection = await _context.Set<PedidoDetalle>().ToListAsync();
            return collection;
        }

        public async Task<int> AddAsync(PedidoDetalle entity)
        {

            // Añadir el detalle del carrito a la base de datos
            await _context.Set<PedidoDetalle>().AddAsync(entity);

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

        public async Task<List<PedidoDetalle>> GetByPedidoIdAsync(int idPedido)
        {
            return await _context.Set<PedidoDetalle>()
                .Include(cd => cd.IdProductoNavigation)
                    .ThenInclude(p => p.ImagenesProducto)
                .Where(cd => cd.IdPedido == idPedido)
                .ToListAsync();
        }
        public async Task<ICollection<PedidoDetalle>> GetDetallesPorPedido(int idPedido)
        {

            var collection = await _context.PedidoDetalle
                .Include(r => r.IdProductoNavigation)
                    .ThenInclude(s => s.IdPromocion)
                .Include(r => r.IdProductoNavigation)
                    .ThenInclude(p => p.IdCategoriaNavigation)
                    .ThenInclude(c => c.IdPromocion)
                .Include(r => r.IdEmpaqueNavigation)
                .Include(r => r.IdPedidoNavigation)
                    .ThenInclude(p => p.IdMetodoPagoNavigation)
                .Where(r => r.IdPedido == idPedido)
                .ToListAsync();
            return collection;
        }
    }
}

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
                                .FirstAsync();
            return @object!;
        }
        public async Task<ICollection<Pedido>> ListAsync()
        {
            //Select * from Pedido
            var collection = await _context.Set<Pedido>().ToListAsync();
            return collection;
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
    }


}

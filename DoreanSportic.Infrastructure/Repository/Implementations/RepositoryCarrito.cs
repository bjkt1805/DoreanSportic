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
    public class RepositoryCarrito : IRepositoryCarrito
    {
        private readonly DoreanSporticContext _context;
        public RepositoryCarrito(DoreanSporticContext context)
        {
            _context = context;
        }
        public async Task<Carrito> FindByIdAsync(int id)
        {
            //Obtener el carrito (Eager loading con Detalles del carrito)
            var @object = await _context.Carrito
                                .Where(x => x.Id == id)
                                .Include(c => c.CarritoDetalle)
                                    .ThenInclude(d => d.IdProductoNavigation)
                                .FirstAsync();
            return @object!;
        }
        public async Task<ICollection<Carrito>> ListAsync()
        {
            //Select * from Carrito
            var collection = await _context.Set<Carrito>().ToListAsync();
            return collection;
        }
        public async Task<int> AddAsync(Carrito entity)
        {

            // Añadir el producto a la base de datos
            await _context.Set<Carrito>().AddAsync(entity);

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

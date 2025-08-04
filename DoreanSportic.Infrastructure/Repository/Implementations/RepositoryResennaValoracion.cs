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
    public class RepositoryResennaValoracion : IRepositoryResennaValoracion
    {
        private readonly DoreanSporticContext _context;
        public RepositoryResennaValoracion(DoreanSporticContext context)
        {
            _context = context;
        }
        public async Task<ResennaValoracion> FindByIdAsync(int id)
        {
            //Obtener una Resenna (Eager loading con el nombre del producto)
            var @object = await _context.ResennaValoracion
                                .Where(x => x.Id == id)
                                .Include(r => r.IdUsuarioNavigation)
                                .Include(r => r.IdProductoNavigation)
                                    .ThenInclude(p => p.ImagenesProducto)
                                .FirstAsync();
            return @object!;
        }
        public async Task<ICollection<ResennaValoracion>> ListAsync()
        {
            //Select * from ResennaValoracion
            var collection = await _context.ResennaValoracion
                    .Include(r => r.IdUsuarioNavigation)
                    .Include(r => r.IdProductoNavigation)
                    .OrderByDescending(r => r.FechaResenna) // Ordenar por fecha descendente
                    .ToListAsync();
            return collection;

        }

        public async Task<ICollection<ResennaValoracion>> GetResennasPorProducto(int idProducto)
        {
            //Select * from ResennaValoracion where idProducto = idProducto
            var collection = await _context.ResennaValoracion
                    .Include(r => r.IdUsuarioNavigation)
                    .Include(r => r.IdProductoNavigation)
                    .Where(r => r.IdProducto == idProducto)
                    .OrderByDescending(r => r.Id)
                    .ToListAsync();

            return collection;
        }

        public async Task<int> AddAsync(ResennaValoracion entity)
        {

            // Añadir el producto a la base de datos
            await _context.Set<ResennaValoracion>().AddAsync(entity);

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

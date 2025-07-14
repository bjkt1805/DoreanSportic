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
    public class RepositoryPromocion: IRepositoryPromocion
    {
        private readonly DoreanSporticContext _context;
        public RepositoryPromocion(DoreanSporticContext context)
        {
            _context = context;
        }
        public async Task<Promocion> FindByIdAsync(int id)
        {
            //Obtener una Promocion (Eager loading con los productos que tienen la promoción)
            var @object = await _context.Promocion
                                .Where(x => x.Id == id)
                                .Include(p => p.IdCategoria)
                                    .ThenInclude(c => c.Producto)
                                        .ThenInclude(p => p.ImagenesProducto)
                                .Include(p => p.IdProducto)
                                    .ThenInclude(p => p.ImagenesProducto)
                                .FirstAsync();
            return @object!;
        }
        public async Task<ICollection<Promocion>> ListAsync()
        {
            //Select * from Promocion
            var collection = await _context.Set<Promocion>().ToListAsync();
            return collection;
        }

        public async Task<int> AddAsync(Promocion entity,List<int> listaProductosSeleccionados)
        {
            // Adjuntar manualmente las categorías para evitar que EF intente insertarlas
            foreach (var categoria in entity.IdCategoria)
            {
                _context.Attach(categoria); // Evita error de identidad en inserción
            }

            // Asociar productos (relación muchos a muchos)
            entity.IdProducto = await _context.Producto
                .Where(p => listaProductosSeleccionados.Contains(p.Id))
                .ToListAsync();

            // Añadir el producto a la base de datos
            await _context.Set<Promocion>().AddAsync(entity);

            // Para debuggear los cambios que va a realizar EF
            // antes de salvar los cambios (Ej: borrar entidedes, agregar campos, etc)

            var entries = _context.ChangeTracker.Entries();

            foreach (var entry in entries)
            {
                Console.WriteLine($"Entidad: {entry.Entity.GetType().Name}, Estado: {entry.State}");
            }

            await _context.SaveChangesAsync();
            return entity.Id;
        }
    }
}

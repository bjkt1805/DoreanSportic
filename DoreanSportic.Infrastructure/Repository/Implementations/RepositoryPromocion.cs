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

            try
            {
                await _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                // Para loggear la excepción al enviar
                // datos a la base de datos
                var inner = ex.InnerException;
                var innerMessage = inner?.Message ?? ex.Message;

                // Imprimir en consola el error
                Console.WriteLine("Error al guardar en base de datos: " + innerMessage);
            }
            return entity.Id;
        }

        public async Task UpdateAsync(Promocion entity, List<Categoria> nuevasCategorias, List<Producto> nuevosProductos)
        {
            //Obtener la promoción actual desde la base de datos con sus relaciones
            var promocionExistente = await _context.Promocion
                .Include(p => p.IdCategoria)
                .Include(p => p.IdProducto)
                .FirstOrDefaultAsync(p => p.Id == entity.Id);

            if (promocionExistente == null)
                throw new Exception("Promoción no encontrada.");

            // Actualizar propiedades simples
            promocionExistente.Nombre = entity.Nombre;
            promocionExistente.Descripcion = entity.Descripcion;
            promocionExistente.PorcentajeDescuento = entity.PorcentajeDescuento;
            promocionExistente.DescuentoFijo = entity.DescuentoFijo;
            promocionExistente.FechaInicio = entity.FechaInicio;
            promocionExistente.FechaFin = entity.FechaFin;
            promocionExistente.Estado = entity.Estado;

            // Actualizar categorías
            promocionExistente.IdCategoria.Clear();
            foreach (var categoria in nuevasCategorias)
            {
                _context.Attach(categoria);
                promocionExistente.IdCategoria.Add(categoria);
            }

            // Actualizar productos
            promocionExistente.IdProducto.Clear();
            foreach (var producto in nuevosProductos)
            {
                _context.Attach(producto);
                promocionExistente.IdProducto.Add(producto);
            }

            // Para debuggear los cambios que va a realizar EF
            // antes de salvar los cambios (Ej: borrar entidedes, agregar campos, etc)

            var entries = _context.ChangeTracker.Entries();

            foreach (var entry in entries)
            {
                Console.WriteLine($"Entidad: {entry.Entity.GetType().Name}, Estado: {entry.State}");
            }

            await _context.SaveChangesAsync();
        }
        public async Task<ICollection<Producto>> ObtenerProductosPorIdsAsync(List<int> ids)
        {
            return await _context.Producto.Where(p => ids.Contains(p.Id)).ToListAsync();
        }

        public async Task<Categoria?> ObtenerCategoriaPorIdAsync(int id)
        {
            return await _context.Categoria.FindAsync(id);
        }
    }
}

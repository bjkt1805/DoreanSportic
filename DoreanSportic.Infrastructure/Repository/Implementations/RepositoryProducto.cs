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
    public class RepositoryProducto : IRepositoryProducto
    {
        private readonly DoreanSporticContext _context;
        public RepositoryProducto(DoreanSporticContext context)
        {
            _context = context;
        }
        
        // Listar todos los productos
        public async Task<ICollection<Producto>> ListAsync()
        {
            //Select * from Producto where Estado == true
            //Consulta LINQ
            var collection = await _context
                .Set<Producto>()
                .Include(p => p.IdPromocion)
                .Where(p=> p.Estado == true)
                .ToListAsync(); 
            return collection;
        }

        // Listar los productos por categoria
        public async Task<ICollection<Producto>> GetProductoByCategoriaAdmin(int idCategoria)
        {
            //Select * from Producto where idCategoria = @idCategoria
            //Consulta LINQ
            var collection = await _context.Producto
                    //.Include(p => p.ImagenesProducto)
                    .Include(p => p.ImagenesProducto)
                    .Include(p => p.IdMarcaNavigation)
                    .Include(p => p.IdPromocion)
                    .Include(p => p.IdEtiqueta)
                    .Include(p => p.IdCategoriaNavigation)
                    .ThenInclude(c => c.IdPromocion)
                    .Where(p => p.IdCategoria == idCategoria)
                    // Orden descendente por ID para ayuda visual al crear en el dashboard de admin
                    .OrderByDescending(p => p.Id) 
                    .ToListAsync();

            return collection;
        }

        public async Task<ICollection<Producto>> GetProductoByCategoria(int idCategoria)
        {
            //Select * from Producto where idCategoria = @idCategoria
            //Consulta LINQ
            var collection = await _context.Producto
                    //.Include(p => p.ImagenesProducto)
                    .Include(p => p.ImagenesProducto)
                    .Include(p => p.IdMarcaNavigation)
                    .Include(p => p.IdPromocion)
                    .Include(p => p.IdEtiqueta)
                    .Include(p => p.IdCategoriaNavigation)
                    .ThenInclude(c => c.IdPromocion)
                    .Where(p => p.IdCategoria == idCategoria && p.Estado == true)
                    // Orden descendente por ID para ayuda visual al crear en el dashboard de admin
                    .OrderByDescending(p => p.Id)
                    .ToListAsync();

            return collection;
        }

        public async Task<Producto> FindByIdAsync(int id)
        {
            //Obtener un Producto (Eager loading con Imagenes Producto, Categoria y Marca)
            var @object = await _context.Producto
                                .Where(x => x.Id == id)
                                .Include(p => p.ImagenesProducto)
                                .Include(p => p.IdMarcaNavigation)
                                .Include(p => p.IdPromocion)
                                .Include(p => p.IdCategoriaNavigation)
                                    .ThenInclude(c => c.IdPromocion)
                                .Include(p => p.IdEtiqueta)
                                .FirstAsync();
            return @object!;
        }

        public async Task<int> AddAsync(Producto entity, string[] selectedEtiquetas)
        {
            // Relación de muchos a muchos solo con llave primaria compuesta
            var etiquetas = await getEtiquetas(selectedEtiquetas);
            entity.IdEtiqueta = etiquetas;

            // Añadir el producto a la base de datos
            await _context.Set<Producto>().AddAsync(entity);

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
        public async Task UpdateAsync(Producto entity, string[] selectedEtiquetas, List<ImagenProducto> listaImagenes)
        {
            // Obtener el producto actual desde la base de datos
            var productoExistente = await _context.Producto
                .Include(p => p.IdEtiqueta)           // Cargar relación etiquetas
                .Include(p => p.ImagenesProducto)     // Cargar imágenes actuales
                .FirstOrDefaultAsync(p => p.Id == entity.Id);

            if (productoExistente == null)
                throw new Exception("Producto no encontrado.");

            // Actualizar propiedades simples
            productoExistente.Nombre = entity.Nombre;
            productoExistente.Descripcion = entity.Descripcion;
            productoExistente.PrecioBase = entity.PrecioBase;
            productoExistente.Stock = entity.Stock;
            productoExistente.Estado = entity.Estado;
            productoExistente.IdCategoria = entity.IdCategoria;
            productoExistente.IdMarca = entity.IdMarca;

            //Relación de muchos a muchos solo con llave primaria compuesta
            var nuevasEtiquetas = await getEtiquetas(selectedEtiquetas);

            // Actualizar etiquetas (relación muchos a muchos)
            productoExistente.IdEtiqueta.Clear(); // Eliminar todas las etiquetas actuales

            //Asignar las etiquetas actualizadas
            productoExistente.IdEtiqueta = nuevasEtiquetas;

            // Eliminar TODAS las imágenes actuales
            _context.ImagenProducto.RemoveRange(productoExistente.ImagenesProducto);

            // Agregar las nuevas (incluye las que ya existían y las nuevas)
            productoExistente.ImagenesProducto = listaImagenes;

            // Para debuggear los cambios que va a realizar EF
            // antes de salvar los cambios (Ej: borrar entidedes, agregar campos, etc)

            var entries = _context.ChangeTracker.Entries();

            foreach (var entry in entries)
            {
                Console.WriteLine($"Entidad: {entry.Entity.GetType().Name}, Estado: {entry.State}");
            }

            await _context.SaveChangesAsync();
        }

        private async Task<ICollection<Etiqueta>> getEtiquetas(string[] selectedEtiquetas)
        {
            // Buscar o crear etiquetas
            var ids = selectedEtiquetas.Select(id => int.Parse(id)).ToList();
            return await _context.Etiqueta
                                 .Where(e => ids.Contains(e.Id))
                                 .ToListAsync();

        }

    }
}

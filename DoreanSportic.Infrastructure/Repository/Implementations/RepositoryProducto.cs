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
            //Select * from Producto
            //Consulta LINQ
            var collection = await _context
                .Set<Producto>()
                .Include(p => p.IdPromocion)
                .ToListAsync(); 
            return collection;
        }

        // Listar los productos por categoria
        public async Task<ICollection<Producto>> GetProductoByCategoria(int idCategoria)
        {
            //Select * from Producto where idCategoria = @idCategoria
            //Consulta LINQ
            var collection = await _context.Producto
                    //.Include(p => p.ImagenesProducto)
                    .Include(p => p.ImagenesProducto)
                    .Include(p => p.IdMarcaNavigation)
                    .Include(p => p.IdPromocion)
                    .Include(p => p.IdCategoriaNavigation)
                    .ThenInclude(c => c.IdPromocion)
                    .Where(p => p.IdCategoria == idCategoria)
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
                                .FirstAsync();
            return @object!;
        }

        public async Task<int> AddAsync(Producto entity)
        {
            //Relación de muchos a muchos solo con llave primaria compuesta
            //var categorias = await getCategorias(selectedCategorias);
            //entity.IdCategoria = categorias;

            // Añadir el producto a la base de datos
            await _context.Set<Producto>().AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity.Id;
        }
        public async Task UpdateAsync(Producto entity)
        {
            //Las relaciones a actualizar depende de la consulta utilizada en el servicio

            // Asegurar que la relación con Autor se mantenga
            //var autor = await _context.Set<Autor>().FindAsync(entity.IdAutor);
            //entity.IdAutorNavigation = autor!;

            ////Relación de muchos a muchos solo con llave primaria compuesta
            //var nuevasCategorias = await getCategorias(selectedCategorias);
            //entity.IdCategoria.Clear();// Eliminar todas las categorías actuales
            ////Asignar las categorias actualizadas
            //entity.IdCategoria = nuevasCategorias;

            await _context.SaveChangesAsync();
        }

    }
}

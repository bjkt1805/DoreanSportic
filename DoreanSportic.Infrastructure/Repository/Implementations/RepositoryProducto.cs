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
                .ToListAsync(); 
            return collection;
        }

        // Listar los productos por categoria
        public async Task<ICollection<Producto>> GetProductoByCategoria(int idCategoria)
        {
            //Select * from Producto where idCategoria = @idCategoria
            //Consulta LINQ
            var collection = await _context.Producto
                    .Include(p => p.ImagenesProducto)
                    .Include(p => p.IdMarcaNavigation)
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
                                .Include(p => p.IdCategoriaNavigation)
                                .Include(p=> p.IdMarcaNavigation)
                                .FirstAsync();
            return @object!;
        }

    }
}

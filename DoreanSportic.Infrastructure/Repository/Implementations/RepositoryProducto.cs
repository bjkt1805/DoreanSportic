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
            var collection = await _context
                .Set<Producto>()
                .Where(p => p.IdCategoria == idCategoria)
                .ToListAsync();
            return collection;
        }

        public Task<Producto> FindByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

    }
}

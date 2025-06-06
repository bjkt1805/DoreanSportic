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
        public Task<Producto> FindByIdAsync(int id)
        {
            throw new NotImplementedException();
        }
        public async Task<ICollection<Producto>> ListAsync()
        {
            //Select * from Producto
            var collection = await _context.Set<Producto>().ToListAsync();
            return collection;
        }
    }
}

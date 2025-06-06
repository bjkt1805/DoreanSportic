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
        public Task<Carrito> FindByIdAsync(int id)
        {
            throw new NotImplementedException();
        }
        public async Task<ICollection<Carrito>> ListAsync()
        {
            //Select * from Carrito
            var collection = await _context.Set<Carrito>().ToListAsync();
            return collection;
        }
    }
}

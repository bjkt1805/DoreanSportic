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
    public class RepositoryTarjeta : IRepositoryTarjeta
    {
        private readonly DoreanSporticContext _context;
        public RepositoryTarjeta(DoreanSporticContext context)
        {
            _context = context;
        }
        public Task<Tarjeta> FindByIdAsync(int id)
        {
            throw new NotImplementedException();
        }
        public async Task<ICollection<Tarjeta>> ListAsync()
        {
            //Select * from Tarjeta
            var collection = await _context.Set<Tarjeta>().ToListAsync();
            return collection;
        }
    }
}

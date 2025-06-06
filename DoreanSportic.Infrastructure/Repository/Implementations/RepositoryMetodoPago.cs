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
    public class RepositoryMetodoPago : IRepositoryMetodoPago
    {
        private readonly DoreanSporticContext _context;
        public RepositoryMetodoPago(DoreanSporticContext context)
        {
            _context = context;
        }
        public Task<MetodoPago> FindByIdAsync(int id)
        {
            throw new NotImplementedException();
        }
        public async Task<ICollection<MetodoPago>> ListAsync()
        {
            //Select * from MetodoPago
            var collection = await _context.Set<MetodoPago>().ToListAsync();
            return collection;
        }
    }
}

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
    public class RepositoryMarca : IRepositoryMarca
    {
        private readonly DoreanSporticContext _context;
        public RepositoryMarca(DoreanSporticContext context)
        {
            _context = context;
        }
        public Task<Marca> FindByIdAsync(int id)
        {
            throw new NotImplementedException();
        }
        public async Task<ICollection<Marca>> ListAsync()
        {
            //Select * from Marca
            var collection = await _context.Set<Marca>().ToListAsync();
            return collection;
        }
    }
}

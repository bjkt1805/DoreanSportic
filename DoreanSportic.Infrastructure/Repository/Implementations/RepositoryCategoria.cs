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
    public class RepositoryCategoria : IRepositoryCategoria
    {
        private readonly DoreanSporticContext _context;
        public RepositoryCategoria(DoreanSporticContext context)
        {
            _context = context;
        }
        public Task<Categoria> FindByIdAsync(int id)
        {
            throw new NotImplementedException();
        }
        public async Task<ICollection<Categoria>> ListAsync()
        {
            //Select * from Categoria
            var collection = await _context.Set<Categoria>().ToListAsync();
            return collection;
        }
    }
}

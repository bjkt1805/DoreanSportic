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
    public class RepositoryEtiqueta : IRepositoryEtiqueta
    {
        private readonly DoreanSporticContext _context;
        public RepositoryEtiqueta(DoreanSporticContext context)
        {
            _context = context;
        }
        public Task<Etiqueta> FindByIdAsync(int id)
        {
            throw new NotImplementedException();
        }
        public async Task<ICollection<Etiqueta>> ListAsync()
        {
            //Select * from Etiqueta
            var collection = await _context.Set<Etiqueta>().ToListAsync();
            return collection;
        }
    }
}

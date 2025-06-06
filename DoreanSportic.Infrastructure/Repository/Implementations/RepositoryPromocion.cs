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
    public class RepositoryPromocion: IRepositoryPromocion
    {
        private readonly DoreanSporticContext _context;
        public RepositoryPromocion(DoreanSporticContext context)
        {
            _context = context;
        }
        public Task<Promocion> FindByIdAsync(int id)
        {
            throw new NotImplementedException();
        }
        public async Task<ICollection<Promocion>> ListAsync()
        {
            //Select * from Promocion
            var collection = await _context.Set<Promocion>().ToListAsync();
            return collection;
        }
    }
}

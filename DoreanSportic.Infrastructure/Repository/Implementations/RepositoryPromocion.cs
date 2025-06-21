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
        public async Task<Promocion> FindByIdAsync(int id)
        {
            //Obtener una Promocion (Eager loading con los productos que tienen la promoción)
            var @object = await _context.Promocion
                                .Where(x => x.Id == id)
                                .Include(p => p.IdProducto)
                                    .ThenInclude(p => p.ImagenesProducto)
                                .FirstAsync();
            return @object!;
        }
        public async Task<ICollection<Promocion>> ListAsync()
        {
            //Select * from Promocion
            var collection = await _context.Set<Promocion>().ToListAsync();
            return collection;
        }
    }
}

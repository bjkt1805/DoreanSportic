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
    public class RepositoryResennaValoracion : IRepositoryResennaValoracion
    {
        private readonly DoreanSporticContext _context;
        public RepositoryResennaValoracion(DoreanSporticContext context)
        {
            _context = context;
        }
        public Task<ResennaValoracion> FindByIdAsync(int id)
        {
            throw new NotImplementedException();
        }
        public async Task<ICollection<ResennaValoracion>> ListAsync()
        {
            //Select * from ResennaValoracion
            var collection = await _context.Set<ResennaValoracion>().ToListAsync();
            return collection;
        }

        public async Task<ICollection<ResennaValoracion>> GetResennasPorProducto(int idProducto)
        {
            //Select * from ResennaValoracion
            var collection = await _context.ResennaValoracion
                    .Include(r => r.IdUsuarioNavigation)
                    .Include(r => r.IdProductoNavigation)
                    .Where(r => r.IdProducto == idProducto)
                    .ToListAsync();

            return collection;
        }
    }
}

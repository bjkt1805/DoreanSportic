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
    public class RepositoryImagenProducto : IRepositoryImagenProducto
    {
        private readonly DoreanSporticContext _context;
        public RepositoryImagenProducto(DoreanSporticContext context)
        {
            _context = context;
        }
        public Task<ImagenProducto> FindByIdAsync(int id)
        {
            throw new NotImplementedException();
        }
        public async Task<ICollection<ImagenProducto>> ListAsync()
        {
            //Select * from ImagenProducto
            var collection = await _context.Set<ImagenProducto>().ToListAsync();
            return collection;
        }
    }
}

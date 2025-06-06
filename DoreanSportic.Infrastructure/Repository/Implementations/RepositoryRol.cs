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
    public class RepositoryRol : IRepositoryRol
    {
        private readonly DoreanSporticContext _context;
        public RepositoryRol(DoreanSporticContext context)
        {
            _context = context;
        }
        public Task<Rol> FindByIdAsync(int id)
        {
            throw new NotImplementedException();
        }
        public async Task<ICollection<Rol>> ListAsync()
        {
            //Select * from Rol
            var collection = await _context.Set<Rol>().ToListAsync();
            return collection;
        }
    }
}

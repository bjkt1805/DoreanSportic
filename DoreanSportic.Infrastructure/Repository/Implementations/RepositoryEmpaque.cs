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
    public class RepositoryEmpaque : IRepositoryEmpaque
    {
        private readonly DoreanSporticContext _context;
        public RepositoryEmpaque(DoreanSporticContext context)
        {
            _context = context;
        }
        public Task<Empaque> FindByIdAsync(int id)
        {
            throw new NotImplementedException();
        }
        public async Task<ICollection<Empaque>> ListAsync()
        {
            //Select * from Empaque where Estado = true;
            var collection = await _context
                .Set<Empaque>()
                .Where(e => e.Estado == true)
                .ToListAsync();
            return collection;
        }
    }
}

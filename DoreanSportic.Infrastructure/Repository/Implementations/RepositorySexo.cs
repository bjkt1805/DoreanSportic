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
    public class RepositorySexo : IRepositorySexo
    {
        private readonly DoreanSporticContext _context;
        public RepositorySexo(DoreanSporticContext context)
        {
            _context = context;
        }
        public Task<Sexo> FindByIdAsync(int id)
        {
            throw new NotImplementedException();
        }
        public async Task<ICollection<Sexo>> ListAsync()
        {
            //Select * from Sexo
            var collection = await _context.Set<Sexo>().ToListAsync();
            return collection;
        }
    }
}

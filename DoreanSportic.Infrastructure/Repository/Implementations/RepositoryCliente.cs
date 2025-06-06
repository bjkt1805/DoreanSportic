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
    public class RepositoryCliente : IRepositoryCliente
    {
        private readonly DoreanSporticContext _context;
        public RepositoryCliente(DoreanSporticContext context)
        {
            _context = context;
        }
        public Task<Cliente> FindByIdAsync(int id)
        {
            throw new NotImplementedException();
        }
        public async Task<ICollection<Cliente>> ListAsync()
        {
            //Select * from Cliente
            var collection = await _context.Set<Cliente>().ToListAsync();
            return collection;
        }
    }
}

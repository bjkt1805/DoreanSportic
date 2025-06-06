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
    public class RepositoryPedido : IRepositoryPedido
    {
        private readonly DoreanSporticContext _context;
        public RepositoryPedido(DoreanSporticContext context)
        {
            _context = context;
        }
        public Task<Pedido> FindByIdAsync(int id)
        {
            throw new NotImplementedException();
        }
        public async Task<ICollection<Pedido>> ListAsync()
        {
            //Select * from Pedido
            var collection = await _context.Set<Pedido>().ToListAsync();
            return collection;
        }
    }
}

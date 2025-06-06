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
    public class RepositoryPedidoDetalle : IRepositoryPedidoDetalle
    {
        private readonly DoreanSporticContext _context;
        public RepositoryPedidoDetalle(DoreanSporticContext context)
        {
            _context = context;
        }
        public Task<PedidoDetalle> FindByIdAsync(int id)
        {
            throw new NotImplementedException();
        }
        public async Task<ICollection<PedidoDetalle>> ListAsync()
        {
            //Select * from Carrito
            var collection = await _context.Set<PedidoDetalle>().ToListAsync();
            return collection;
        }
    }
}

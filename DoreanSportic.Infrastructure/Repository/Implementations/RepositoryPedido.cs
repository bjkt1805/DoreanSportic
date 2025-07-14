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
        public async Task<Pedido> FindByIdAsync(int id)
        {
            //Obtener una Promocion (Eager loading con los productos que tienen la promoción)
            var @object = await _context.Pedido
                                .Where(x => x.Id == id)
                                .Include(p => p.PedidoDetalle)
                                .Include(p => p.IdClienteNavigation)
                                .Include(p => p.PedidoDetalle)
                                .Include(p => p.IdMetodoPagoNavigation)
                                .FirstAsync();
            return @object!;
        }
        public async Task<ICollection<Pedido>> ListAsync()
        {
            //Select * from Pedido
            var collection = await _context.Set<Pedido>().ToListAsync();
            return collection;
        }
    }
}

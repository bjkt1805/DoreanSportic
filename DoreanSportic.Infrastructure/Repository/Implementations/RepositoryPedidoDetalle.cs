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
        public async Task<ICollection<PedidoDetalle>> GetDetallesPorPedido(int idPedido)
        {

            var collection = await _context.PedidoDetalle
                .Include(r => r.IdProductoNavigation)
                    .ThenInclude(s => s.IdPromocion)
                .Include(r => r.IdProductoNavigation)
                    .ThenInclude(p => p.IdCategoriaNavigation)
                    .ThenInclude(c => c.IdPromocion)
                .Include(r => r.IdEmpaqueNavigation)
                .Include(r => r.IdPedidoNavigation)
                    .ThenInclude(p => p.IdMetodoPagoNavigation)
                .Where(r => r.IdPedido == idPedido)
                .ToListAsync();
            return collection;
        }
    }
}

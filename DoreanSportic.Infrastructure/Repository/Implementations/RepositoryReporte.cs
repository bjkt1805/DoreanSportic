using DoreanSportic.Infrastructure.Data;
using DoreanSportic.Infrastructure.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DoreanSportic.Infrastructure.Repository.Implementations
{
    public class RepositoryReporte : IRepositoryReporte
    {
        private readonly DoreanSporticContext _context;
        public RepositoryReporte(DoreanSporticContext context)
        {
            _context = context;
        }

        // Cantidad de ventas (pedidos) por día
        public async Task<object> VentasPorDiaAsync(DateTime from, DateTime to)
        {
            // Consulta LINQ para agrupar por día y contar pedidos
            var query = await _context.Pedido
                .Where(p => p.FechaPedido.Date >= from.Date && p.FechaPedido.Date <= to.Date && p.Estado)
                .GroupBy(p => p.FechaPedido.Date)
                .Select(g => new
                {
                    dia = g.Key,
                    cantidad = g.Count()
                })
                .OrderBy(x => x.dia)
                .ToListAsync();

            // Etiquetas con formato dd/MM/yyyy
            var labels = query.Select(x => x.dia.ToString("dd/MM/yyyy")).ToArray();

            // Datos (cantidad de ventas por día)
            var data = query.Select(x => x.cantidad).ToArray();

            // Retornar objeto con labels y data
            return new { labels, data };
        }

        // Cantidad de ventas por mes (por año)
        public async Task<object> VentasPorMesAsync(int year)
        {
            // Consulta LINQ para agrupar por mes y contar pedidos
            var query = await _context.Pedido
                .Where(p => p.FechaPedido.Year == year && p.Estado)
                .GroupBy(p => p.FechaPedido.Month)
                .Select(g => new
                {
                    mes = g.Key,
                    cantidad = g.Count()
                })
                .OrderBy(x => x.mes)
                .ToListAsync();

            // Etiquetas con nombres de meses abreviados (Ene, Feb, Mar, etc.)
            var labels = Enumerable.Range(1, 12).Select(m => new DateTime(year, m, 1).ToString("MMM")).ToArray();

            // Datos (cantidad de ventas por mes, 0 si no hay ventas en ese mes)
            var data = Enumerable.Range(1, 12)
                .Select(m => query.FirstOrDefault(x => x.mes == m)?.cantidad ?? 0)
                .ToArray();

            // Retornar objeto con labels y data
            return new { labels, data };
        }

        // Pedidos por estado
        public async Task<object> PedidosPorEstadoAsync()
        {
            // Consulta LINQ para agrupar por estado y contar pedidos
            var query = await _context.Pedido
                .GroupBy(p => p.EstadoPago) // <-- CAMBIA a tu propiedad real
                .Select(g => new
                {
                    estado = g.Key,
                    cantidad = g.Count()
                })
                .OrderByDescending(x => x.cantidad)
                .ToListAsync();

            // Etiquetas con nombres de estados (o "N/D" si es null)
            var labels = query.Select(x => x.estado ?? "N/D").ToArray();

            // Datos (cantidad de pedidos por estado)
            var data = query.Select(x => x.cantidad).ToArray();

            // Retornar objeto con labels y data
            return new { labels, data };
        }

        // Top 3 productos por número de ventas (sumatoria de cantidades vendidas)
        public async Task<object> TopProductosAsync(int n)
        {
            // Consulta LINQ para agrupar por producto y sumar cantidades vendidas
            var query = await _context.PedidoDetalle
                .Where(d => d.IdPedidoNavigation.Estado) // pedidos válidos
                .GroupBy(d => new { d.IdProducto, d.IdProductoNavigation.Nombre })
                .Select(g => new
                {
                    producto = g.Key.Nombre,
                    cantidadVendida = g.Sum(x => x.Cantidad)
                })
                .OrderByDescending(x => x.cantidadVendida)
                .Take(n)
                .ToListAsync();

            // Etiquetas con nombres de productos
            var labels = query.Select(x => x.producto).ToArray();

            // Datos (cantidades vendidas)
            var data = query.Select(x => x.cantidadVendida).ToArray();

            // Retornar objeto con labels y data
            return new { labels, data };
        }

        // Últimas 3 reseñas
        public async Task<object> ResennasRecientesAsync(int n)
        {
            // Consulta LINQ para obtener las últimas 3 reseñas
            var query = await _context.ResennaValoracion
                .Include(r => r.IdProductoNavigation)
                .Include(r => r.IdUsuarioNavigation)
                .OrderByDescending(r => r.FechaResenna)
                .Take(n)
                .Select(r => new
                {
                    producto = r.IdProductoNavigation.Nombre,
                    usuario = r.IdUsuarioNavigation.UserName,
                    calificacion = r.Calificacion,
                    comentario = r.Comentario,
                    fecha = r.FechaResenna
                })
                .ToListAsync();

            // Enlistrar las reseñas con fecha formateada
            var items = query.Select(r => new
            {
                r.producto,
                r.usuario,
                r.calificacion,
                r.comentario,
                fecha = r.fecha.ToString("dd/MM/yyyy")
            });

            // Retornar las reseñas (lista)
            return items;
        }
    }
}

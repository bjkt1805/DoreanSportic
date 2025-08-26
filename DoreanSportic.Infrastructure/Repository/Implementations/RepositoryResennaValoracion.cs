using DoreanSportic.Infrastructure.Data;
using DoreanSportic.Infrastructure.Models;
using DoreanSportic.Infrastructure.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoreanSportic.Infrastructure.Repository.Implementations
{
    public class RepositoryResennaValoracion : IRepositoryResennaValoracion
    {
        private readonly DoreanSporticContext _context;
        public RepositoryResennaValoracion(DoreanSporticContext context)
        {
            _context = context;
        }
        public async Task<ResennaValoracion> FindByIdAsync(int id)
        {
            //Obtener una Resenna (Eager loading con el nombre del producto)
            var @object = await _context.ResennaValoracion
                                .Where(x => x.Id == id)
                                .Include(r => r.IdUsuarioNavigation)
                                .Include(r => r.IdProductoNavigation)
                                    .ThenInclude(p => p.ImagenesProducto)
                                .FirstAsync();
            return @object!;
        }
        public async Task<ICollection<ResennaValoracion>> ListAsync()
        {
            //Select * from ResennaValoracion
            var collection = await _context.ResennaValoracion
                    .Include(r => r.IdUsuarioNavigation)
                    .Include(r => r.IdProductoNavigation)
                    .OrderByDescending(r => r.FechaResenna) // Ordenar por fecha descendente
                    .ToListAsync();
            return collection;

        }

        public async Task<ICollection<ResennaValoracion>> GetResennasPorProducto(int idProducto, int? calificacion = null, int? take = null)
        {

            // Construir la consulta base (IQueryable)
            IQueryable<ResennaValoracion> q = _context.ResennaValoracion
                .Include(r => r.IdUsuarioNavigation)
                .Include(r => r.IdProductoNavigation)
                .Where(r => r.IdProducto == idProducto && r.Estado == true);

            // Si se especifica una calificación, filtrar por ella
            if (calificacion.HasValue)
                q = q.Where(r => r.Calificacion == calificacion.Value);


            // Ordenar la lista por fecha descendente
            q = q.OrderByDescending(r => r.FechaResenna);

            // Por defecto: últimas dos reseñas si NO hay filtro
            if (!calificacion.HasValue)
                q = q.Take(take ?? 2);

            // Ejecutar la consulta y retornar la lista
            return await q.ToListAsync();
        }

        public async Task<int> AddAsync(ResennaValoracion entity)
        {

            // Añadir el producto a la base de datos
            await _context.Set<ResennaValoracion>().AddAsync(entity);

            // Para debuggear los cambios que va a realizar EF
            // antes de salvar los cambios (Ej: borrar entidedes, agregar campos, etc)

            var entries = _context.ChangeTracker.Entries();

            foreach (var entry in entries)
            {
                Console.WriteLine($"Entidad: {entry.Entity.GetType().Name}, Estado: {entry.State}");
            }

            try
            {
                await _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                // Para loggear la excepción al enviar
                // datos a la base de datos
                var inner = ex.InnerException;
                var innerMessage = inner?.Message ?? ex.Message;

                // Imprimir en consola el error
                Console.WriteLine("Error al guardar en base de datos: " + innerMessage);
            }
            return entity.Id;
        }

        // Método para obtener estadísticas de valoraciones
        public async Task<(int Star5, int Star4, int Star3, int Star2, int Star1, int Total, double Average)> GetStatsAsync()
        {
            // Agrupar por calificación
            var grupos = await _context.ResennaValoracion
                .GroupBy(r => r.Calificacion)
                .Select(g => new { Star = g.Key, Count = g.Count() })
                .ToListAsync();

            // Obtener el conteo para cada calificación, si no existe, asignar 0
            int s5 = grupos.FirstOrDefault(x => x.Star == 5)?.Count ?? 0;
            int s4 = grupos.FirstOrDefault(x => x.Star == 4)?.Count ?? 0;
            int s3 = grupos.FirstOrDefault(x => x.Star == 3)?.Count ?? 0;
            int s2 = grupos.FirstOrDefault(x => x.Star == 2)?.Count ?? 0;
            int s1 = grupos.FirstOrDefault(x => x.Star == 1)?.Count ?? 0;

            // Calcular total y promedio
            int total = s1 + s2 + s3 + s4 + s5;
            double avg = 0;

            // Si el total es mayor que 0, calcular el promedio
            if (total > 0)
            {
                var suma = 5 * s5 + 4 * s4 + 3 * s3 + 2 * s2 + 1 * s1;
                avg = (double)suma / total;
            }

            // Retornar los resultados
            return (s5, s4, s3, s2, s1, total, avg);
        }

        // Método para reportar una reseña
        public async Task ReportarAsync(int idResenna, int idUsuarioReporta, string nombreUsuarioReporta, string? observacion)
        {
            // Buscar la reseña por su Id
            var res = await _context.ResennaValoracion.FirstOrDefaultAsync(x => x.Id == idResenna);
            // Si no se encuentra, salir
            if (res == null) return;

            // Marcar la reseña como reportada
            res.Reportada = true;

            // Añadir una línea al campo ObservacionReporte (bitácora para el admin)
            var stamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss 'UTC'", CultureInfo.InvariantCulture);

            // Formatear la línea a añadir
            var linea = $"[{stamp}] Reportado por Id={idUsuarioReporta}, User=\"{nombreUsuarioReporta}\""
                      + (string.IsNullOrWhiteSpace(observacion) ? "" : $": {observacion.Trim()}");

            // Si no hay observaciones previas, asignar la nueva línea
            if (string.IsNullOrWhiteSpace(res.ObservacionReporte))
                res.ObservacionReporte = linea;

            // Si ya hay observaciones, añadir la nueva línea al final
            else
                res.ObservacionReporte += Environment.NewLine + linea;

            // Guardar los cambios en la base de datos
            await _context.SaveChangesAsync();
        }

        // Método para actualizar el estado (activo/inactivo) de una reseña
        public async Task UpdateEstadoAsync(int id, bool estado)
        {
            // Buscar la reseña por su Id
            var res = await _context.ResennaValoracion.FirstOrDefaultAsync(x => x.Id == id);
            // Si no se encuentra, salir
            if (res == null) return;
            // Actualizar el estado
            res.Estado = estado;
            // Guardar los cambios en la base de datos
            await _context.SaveChangesAsync();
        }

        // Método para verificar si un usuario ya valoró un producto
        public async Task<bool> ExistsByUserProductAsync(int userId, int productId)
        {
            return await _context.ResennaValoracion
                .AnyAsync(r => r.IdUsuario == userId && r.IdProducto == productId);
        }

    }
}

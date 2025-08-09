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
    public class RepositoryUsuario : IRepositoryUsuario
    {
        private readonly DoreanSporticContext _context;
        public RepositoryUsuario(DoreanSporticContext context)
        {
            _context = context;
        }
        public Task<Usuario> FindByIdAsync(int id)
        {
            throw new NotImplementedException();
        }
        public async Task<ICollection<Usuario>> ListAsync()
        {
            //Select * from Usuario
            var collection = await _context.Set<Usuario>().ToListAsync();
            return collection;
        }

        // PARA EFECTOS DEL AVANCE 4
        public async Task<Usuario> PrimerUsuario()
        {
            //Select top 1 from Usuario
            var usuario = await _context.Set<Usuario>().FirstOrDefaultAsync();
            return usuario;
        }

        public async Task<int> CrearUsuarioAsync(Usuario entity)
        {
            // Añadir el cliente a la base de datos
            await _context.Set<Usuario>().AddAsync(entity);

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

        public async Task<bool> ExisteUserNameAsync(string userName)
        {
            // Verificar si existe un usuario con el mismo UserName
            return await _context.Set<Usuario>()
                .AnyAsync(u => u.UserName == userName && u.Estado);
        }

        // Buscar un usuario por UserName y Estado activo
        public async Task<Usuario?> FindByUserNameWithClienteAsync(string userName)
        {
            return await _context.Set<Usuario>()
                .Include(u => u.IdClienteNavigation)
                .FirstOrDefaultAsync(u =>
                    u.UserName == userName &&
                    u.Estado && u.EsActivo);
        }
    }
}

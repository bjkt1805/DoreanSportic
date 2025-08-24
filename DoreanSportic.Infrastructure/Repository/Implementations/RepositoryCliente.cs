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
        public async Task<Cliente> FindByIdAsync(int id)
        {
            var @object = await _context.Cliente
                                .Where(c => c.Id == id)
                                .FirstAsync();
            return @object!;
        }

        public async Task<Cliente> FindByUserIdAsync(int userId)
        {
            return await _context.Cliente
                .FirstOrDefaultAsync(c => c.Usuario.Id == userId);
        }
        public async Task<bool> ExisteEmailAsync(string email)
        {
            // Verificar si ya existe este correo en la base datos
            return await _context.Set<Cliente>()
                .AnyAsync(c => c.Email == email && c.Estado);
        }

        public async Task<bool> ExisteEmailEditUsuarioAsync(string email, int? idCliente = null)
        {
            // Verificar si existe un usuario con el mismo correo electrónico
            return await _context.Set<Cliente>()
                .AnyAsync(c =>
                    c.Email == email &&
                    c.Estado &&
                    c.Id != idCliente); // Se excluye de la consulta el id del cliente actual
        }

        public async Task<ICollection<Cliente>> ListAsync()
        {
            //Select * from Cliente
            var collection = await _context.Set<Cliente>().ToListAsync();
            return collection;
        }
        public async Task<int> CrearClienteAsync(Cliente entity)
        {
            // Añadir el cliente a la base de datos
            await _context.Set<Cliente>().AddAsync(entity);

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

        public async Task <int>ActualizarClienteAsync(Cliente entity)
        {

            // Actualizar el cliente en la base de datos
            _context.Set<Cliente>().Update(entity);
            return await _context.SaveChangesAsync();
        }
    }
    
}

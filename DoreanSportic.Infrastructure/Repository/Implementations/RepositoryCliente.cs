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

        public async Task<Cliente> FindByUserIdAsync(int userId)
        {
            return await _context.Cliente
                .FirstOrDefaultAsync(c => c.Usuario.Id == userId);
        }
        public async Task<bool> ExisteEmailAsync(string email)
        {
            // Verificar si existe un usuario con el mismo correo electrónico
            return await _context.Set<Cliente>()
                .AnyAsync(c => c.Email == email && c.Estado);
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

        public async Task ActualizarClienteAsync(Cliente cliente)
        {
            // Asegurarse de que el cliente existe en la base de datos
            _context.Attach(cliente);

            // Marcar las propiedades que se van a actualizar
            _context.Entry(cliente).Property(c => c.Nombre).IsModified = true;
            _context.Entry(cliente).Property(c => c.Apellido).IsModified = true;
            _context.Entry(cliente).Property(c => c.Email).IsModified = true;
            _context.Entry(cliente).Property(c => c.Telefono).IsModified = true;
            _context.Entry(cliente).Property(c => c.IdSexo).IsModified = true;
            _context.Entry(cliente).Property(c => c.Estado).IsModified = true;

            // Para debuggear los cambios que va a realizar EF
            // antes de salvar los cambios (Ej: borrar entidedes, agregar campos, etc)

            var entries = _context.ChangeTracker.Entries();

            foreach (var entry in entries)
            {
                Console.WriteLine($"Entidad: {entry.Entity.GetType().Name}, Estado: {entry.State}");
            }

            // Guardar los cambios en la base de datos
            await _context.SaveChangesAsync();
        }
    }
}

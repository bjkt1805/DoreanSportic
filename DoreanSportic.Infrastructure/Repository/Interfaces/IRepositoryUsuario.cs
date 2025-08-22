using DoreanSportic.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoreanSportic.Infrastructure.Repository.Interfaces
{
    public interface IRepositoryUsuario
    {
        Task<ICollection<Usuario>> ListAsync();
        Task<Usuario> FindByIdAsync(int id);
        Task<Usuario> FindByUserNameAsync(string userName);

        // PARA EFECTOS DEL AVANCE 4
        Task<Usuario> PrimerUsuario();

        Task<bool> ExisteUserNameAsync(String userName);
        Task<int> CrearUsuarioAsync(Usuario entity);

        Task<int> ActualizarUsuarioAsync(Usuario entity);

        // Buscar usuario por cliente
        Task<Usuario?> FindByUserNameWithClienteAsync(string userName);

        // Cambiar solo campos necesarios de Usuario
        Task<int> SaveChangesAsync();

        // Obtener fecha/hora de inicio de sesión
        Task<DateTime?> GetLastLoginUtcAsync (int usuarioId);

        // Actualizar fecha/hora de inicio de sesión
        Task ActualizarFechaHoraUltimoLogin(int id, DateTime utcNow);


    }
}

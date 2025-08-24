using DoreanSportic.Application.DTOs;
using DoreanSportic.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace DoreanSportic.Application.Services.Interfaces
{
    public interface IServiceUsuario
    {
        Task<ICollection<UsuarioDTO>> ListAsync();
        Task<UsuarioDTO> FindByIdAsync(int id);

        // PARA EFECTOS DEL AVANCE 4
        Task<UsuarioDTO> PrimerUsuario();
        Task<bool> ExisteUserNameAsync(string userName);

        // Crear usuario
        Task<int> CrearUsuarioAsync(UsuarioDTO dto);

        // Actualizar usuario
        Task<bool> ActualizarUsuarioAsync(UsuarioDTO dto);

        // Actualizar usuario (vista EditUsuario)

        Task<bool> ActualizarUsuarioEditAsync(UsuarioDTO dto);

        // Login de usuario 
        Task<UsuarioDTO?> LoginAsync(string id, string password);

        // Cambiar contraseña
        Task<bool> RecuperarContrasennaAsync(string nombreUsuario, string nuevaContrasenna);

        // Registrar inicio de sesión
        Task<DateTime?> RegistrarInicioSesionAsync(int usuarioId);

    }
}
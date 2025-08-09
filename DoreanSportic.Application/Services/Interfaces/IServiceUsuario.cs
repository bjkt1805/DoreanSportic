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
        Task<int> CrearUsuarioAsync(Usuario u);
        Task<UsuarioDTO> LoginAsync(string id, string password);
    }
}
using DoreanSportic.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoreanSportic.Infrastructure.Repository.Interfaces
{
    public interface IRepositoryCliente 
    {
        Task<ICollection<Cliente>> ListAsync();
        Task<Cliente> FindByIdAsync(int id);

        Task<bool> ExisteEmailAsync(String email);

        Task<bool> ExisteEmailEditUsuarioAsync(String email, int? idCliente = null);

        Task<int> CrearClienteAsync(Cliente entity);

        Task<int> ActualizarClienteAsync(Cliente entity);

        Task<Cliente> FindByUserIdAsync(int userId);
    }
}

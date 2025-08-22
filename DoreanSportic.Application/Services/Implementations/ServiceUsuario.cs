using AutoMapper;
using DoreanSportic.Application.DTOs;
using DoreanSportic.Application.Services.Interfaces;
using DoreanSportic.Infrastructure.Models;
using DoreanSportic.Infrastructure.Repository.Implementations;
using DoreanSportic.Infrastructure.Repository.Interfaces;
using DoreanSportic.Abstractions.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace DoreanSportic.Application.Services.Implementations
{
    public class ServiceUsuario : IServiceUsuario
    {
        private readonly IRepositoryUsuario _repository;
        private readonly IMapper _mapper;
        private readonly IPasswordHasher _passwordHasher;
        public ServiceUsuario(IRepositoryUsuario repository, IMapper mapper, IPasswordHasher passwordHasher)
        {
            _repository = repository;
            _mapper = mapper;
            _passwordHasher = passwordHasher;
        }
        public async Task<UsuarioDTO> FindByIdAsync(int id)
        {
            var @object = await _repository.FindByIdAsync(id);
            var objectMapped = _mapper.Map<UsuarioDTO>(@object);
            return objectMapped;
        }
        public async Task<ICollection<UsuarioDTO>> ListAsync()
        {
            //Obtener datos del repositorio
            var list = await _repository.ListAsync();
            // Map List<Usuario> a ICollection<UsuarioDTO>
            var collection = _mapper.Map<ICollection<UsuarioDTO>>(list);
            // Return lista
            return collection;
        }

        // PARA EFECTOS DEL AVANCE 4
        public async Task<UsuarioDTO> PrimerUsuario()
        {
            var @object = await _repository.PrimerUsuario();
            var objectMapped = _mapper.Map<UsuarioDTO>(@object);
            return objectMapped;
        }

        public async Task<int> CrearUsuarioAsync(UsuarioDTO dto)
        {
            var objectMapped = _mapper.Map<Usuario>(dto);
            return await _repository.CrearUsuarioAsync(objectMapped);
        }

        public async Task<bool> ActualizarUsuarioAsync(UsuarioDTO dto)
        {
            // Mapeo manual para evitar sobreescritura de PasswordHash si no se proporciona una nueva contraseña
            var entity = new Usuario
            {
                Id = dto.Id,
                IdCliente = dto.IdCliente,
                IdRol = dto.IdRol,
                Estado = dto.Estado,
                EsActivo = dto.EsActivo,
                UserName = dto.UserName,
                PasswordHash = dto.PasswordHash
            };

            var rows = await _repository.ActualizarUsuarioAsync(entity);
            return rows > 0;
        }

        public async Task<bool> ExisteUserNameAsync(string userName)
        {
            // Verificar si el nombre de usuario ya existe en la base de datos
            return await _repository.ExisteUserNameAsync(userName);
        }

        public async Task<UsuarioDTO?> LoginAsync(string userName, string password)
        {
            var user = await _repository.FindByUserNameWithClienteAsync(userName);
            if (user == null) return null;

            // Verificar hash
            var ok = _passwordHasher.Verify(password, user.PasswordHash);
            if (!ok) return null;

            // Mapear a DTO con Cliente (idClienteNavigation) incluido
            var dto = _mapper.Map<UsuarioDTO>(user);
            return dto;
        }

        public async Task<bool> RecuperarContrasennaAsync(string nombreUsuario, string nuevaContrasenna)
        {
            // Obtener el usuario mediante su nombre de Usuario (UserName)
            var usuario = await _repository.FindByUserNameAsync(nombreUsuario);

            // Si el usuario no existe, retornar false
            if (usuario == null) return false;

            // Hashear la nueva contraseña para enviarla encriptada a la base de datos
            usuario.PasswordHash = _passwordHasher.Hash(nuevaContrasenna);

            // Actualizar la contraseña en el repositorio
            var cambioContrasenna = await _repository.SaveChangesAsync();

            // Retornar true si se actualizó correctamente (1), de lo contrario false
            return cambioContrasenna > 0;
        }

        public async Task<DateTime?> RegistrarInicioSesionAsync(int usuarioId)
        {
            // Obtener la fecha y hora del último inicio de sesión
            var anterior = await _repository.GetLastLoginUtcAsync(usuarioId);

            // Actualizar la fecha y hora del último inicio de sesión
            await _repository.ActualizarFechaHoraUltimoLogin(usuarioId, DateTime.UtcNow);
            return anterior;
        }
    }
}
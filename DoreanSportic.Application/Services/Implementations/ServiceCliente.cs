using AutoMapper;
using DoreanSportic.Application.DTOs;
using DoreanSportic.Application.Services.Interfaces;
using DoreanSportic.Infrastructure.Models;
using DoreanSportic.Infrastructure.Repository.Implementations;
using DoreanSportic.Infrastructure.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace DoreanSportic.Application.Services.Implementations
{
    public class ServiceCliente : IServiceCliente
    {
        private readonly IRepositoryCliente _repository;
        private readonly IMapper _mapper;
        public ServiceCliente(IRepositoryCliente repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public async Task<ClienteDTO> FindByIdAsync(int id)
        {
            var @object = await _repository.FindByIdAsync(id);
            var objectMapped = _mapper.Map<ClienteDTO>(@object);
            return objectMapped;
        }

        public async Task<ClienteDTO> GetByUserIdAsync(int userId)
        {
            var @object = await _repository.FindByUserIdAsync(userId);
            var objectMapped = _mapper.Map<ClienteDTO>(@object);
            return objectMapped;
        }
        public async Task<bool> ExisteEmailAsync(string email)
        {
            // Verificar si el correoElectrónico ya existe en la base de datos a la hora
            // de registrar el usuario
            return await _repository.ExisteEmailAsync(email);
        }

        public async Task<bool> ExisteEmailEditUsuarioAsync(string email, int? idCliente = null)
        {
            // Verificar si el correo electrónico ya está asignado a otro usuario
            // a la hora de revisar su asignación
            return await _repository.ExisteEmailEditUsuarioAsync(email, idCliente);
        }
        public async Task<ICollection<ClienteDTO>> ListAsync()
        {
            //Obtener datos del repositorio
            var list = await _repository.ListAsync();
            // Map List<Cliente> a ICollection<ClienteDTO>
            var collection = _mapper.Map<ICollection<ClienteDTO>>(list);
            // Return lista
            return collection;
        }

        public async Task<int> CrearClienteAsync(ClienteDTO dto)
        {
            var objectMapped = _mapper.Map<Cliente>(dto);
            return await _repository.CrearClienteAsync(objectMapped);
        }

        public async Task<bool> ActualizarClienteAsync(ClienteDTO dto)
        {
            var entity = new Cliente
            {
                Id = dto.Id,
                Nombre = dto.Nombre,
                Apellido = dto.Apellido,
                Email = dto.Email,
                Telefono = dto.Telefono,
                IdSexo = dto.IdSexo,
                Estado = dto.Estado
            };
            var rows = await _repository.ActualizarClienteAsync(entity);
            return rows > 0;
        }
    }
}
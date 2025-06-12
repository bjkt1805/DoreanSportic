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
        public async Task<ICollection<ClienteDTO>> ListAsync()
        {
            //Obtener datos del repositorio
            var list = await _repository.ListAsync();
            // Map List<Cliente> a ICollection<ClienteDTO>
            var collection = _mapper.Map<ICollection<ClienteDTO>>(list);
            // Return lista
            return collection;
        }
    }
}
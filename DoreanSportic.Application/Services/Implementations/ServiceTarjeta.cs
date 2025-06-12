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
    public class ServiceTarjeta : IServiceTarjeta
    {
        private readonly IRepositoryTarjeta _repository;
        private readonly IMapper _mapper;
        public ServiceTarjeta(IRepositoryTarjeta repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public async Task<TarjetaDTO> FindByIdAsync(int id)
        {
            var @object = await _repository.FindByIdAsync(id);
            var objectMapped = _mapper.Map<TarjetaDTO>(@object);
            return objectMapped;
        }
        public async Task<ICollection<TarjetaDTO>> ListAsync()
        {
            //Obtener datos del repositorio
            var list = await _repository.ListAsync();
            // Map List<Tarjeta> a ICollection<TarjetaDTO>
            var collection = _mapper.Map<ICollection<TarjetaDTO>>(list);
            // Return lista
            return collection;
        }
    }
}
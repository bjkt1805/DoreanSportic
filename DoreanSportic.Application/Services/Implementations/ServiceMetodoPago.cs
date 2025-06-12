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
    public class ServiceMetodoPago : IServiceMetodoPago
    {
        private readonly IRepositoryMetodoPago _repository;
        private readonly IMapper _mapper;
        public ServiceMetodoPago(IRepositoryMetodoPago repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public async Task<MetodoPagoDTO> FindByIdAsync(int id)
        {
            var @object = await _repository.FindByIdAsync(id);
            var objectMapped = _mapper.Map<MetodoPagoDTO>(@object);
            return objectMapped;
        }
        public async Task<ICollection<MetodoPagoDTO>> ListAsync()
        {
            //Obtener datos del repositorio
            var list = await _repository.ListAsync();
            // Map List<MetodoPago> a ICollection<MetodoPagoDTO>
            var collection = _mapper.Map<ICollection<MetodoPagoDTO>>(list);
            // Return lista
            return collection;
        }
    }
}
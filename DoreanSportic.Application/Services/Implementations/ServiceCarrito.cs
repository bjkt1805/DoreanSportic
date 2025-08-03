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
    public class ServiceCarrito : IServiceCarrito
    {
        private readonly IRepositoryCarrito _repository;
        private readonly IMapper _mapper;
        public ServiceCarrito(IRepositoryCarrito repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public async Task<CarritoDTO> FindByIdAsync(int id)
        {
            var @object = await _repository.FindByIdAsync(id);
            var objectMapped = _mapper.Map<CarritoDTO>(@object);
            return objectMapped;
        }
        public async Task<ICollection<CarritoDTO>> ListAsync()
        {
            //Obtener datos del repositorio
            var list = await _repository.ListAsync();
            // Map List<Carrito> a ICollection<CarritoDTO>
            var collection = _mapper.Map<ICollection<CarritoDTO>>(list);
            // Return lista
            return collection;
        }
        public async Task<int> AddAsync(CarritoDTO dto)
        {
            var objectMapped = _mapper.Map<Carrito>(dto);
            return await _repository.AddAsync(objectMapped);
        }
    }
}
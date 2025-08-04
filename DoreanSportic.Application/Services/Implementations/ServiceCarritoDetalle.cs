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
    public class ServiceCarritoDetalle : IServiceCarritoDetalle
    {
        private readonly IRepositoryCarritoDetalle _repository;
        private readonly IMapper _mapper;
        public ServiceCarritoDetalle(IRepositoryCarritoDetalle repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public async Task<CarritoDetalleDTO> FindByIdAsync(int id)
        {
            var @object = await _repository.FindByIdAsync(id);
            var objectMapped = _mapper.Map<CarritoDetalleDTO>(@object);
            return objectMapped;
        }
        public async Task<ICollection<CarritoDetalleDTO>> ListAsync()
        {
            //Obtener datos del repositorio
            var list = await _repository.ListAsync();
            // Map List<CarritoDetalle> a ICollection<CarritoDetalleDTO>
            var collection = _mapper.Map<ICollection<CarritoDetalleDTO>>(list);
            // Return lista
            return collection;
        }
        public async Task<int> AddAsync(CarritoDetalleDTO dto)
        {
            var objectMapped = _mapper.Map<CarritoDetalle>(dto);
            return await _repository.AddAsync(objectMapped);
        }

        public async Task<List<CarritoDetalleDTO>> GetByCarritoIdAsync(int idCarrito)
        {
            var detalles = await _repository.GetByCarritoIdAsync(idCarrito);
            return _mapper.Map<List<CarritoDetalleDTO>>(detalles);
        }

    }
}
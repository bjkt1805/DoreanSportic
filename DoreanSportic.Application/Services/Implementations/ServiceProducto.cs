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
    public class ServiceProducto : IServiceProducto
    {
        private readonly IRepositoryProducto _repository;
        private readonly IMapper _mapper;
        public ServiceProducto(IRepositoryProducto repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public async Task<ProductoDTO> FindByIdAsync(int id)
        {
            var @object = await _repository.FindByIdAsync(id);
            var objectMapped = _mapper.Map<ProductoDTO>(@object);
            return objectMapped;
        }
        public async Task<ICollection<ProductoDTO>> ListAsync()
        {
            //Obtener datos del repositorio
            var list = await _repository.ListAsync();
            // Map List<Producto> a ICollection<ProductoDTO>
            var collection = _mapper.Map<ICollection<ProductoDTO>>(list);
            // Return lista
            return collection;
        }
    }
}
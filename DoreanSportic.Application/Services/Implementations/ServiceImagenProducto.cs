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
    public class ServiceImagenProducto : IServiceImagenProducto
    {
        private readonly IRepositoryImagenProducto _repository;
        private readonly IMapper _mapper;
        public ServiceImagenProducto(IRepositoryImagenProducto repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public async Task<ImagenProductoDTO> FindByIdAsync(int id)
        {
            var @object = await _repository.FindByIdAsync(id);
            var objectMapped = _mapper.Map<ImagenProductoDTO>(@object);
            return objectMapped;
        }
        public async Task<ICollection<ImagenProductoDTO>> ListAsync()
        {
            //Obtener datos del repositorio
            var list = await _repository.ListAsync();
            // Map List<Categoria> a ICollection<CategoriaDTO>
            var collection = _mapper.Map<ICollection<ImagenProductoDTO>>(list);
            // Return lista
            return collection;
        }
    }
}
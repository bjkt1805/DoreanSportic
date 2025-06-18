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
    public class ServiceResennaValoracion : IServiceResennaValoracion
    {
        private readonly IRepositoryResennaValoracion _repository;
        private readonly IMapper _mapper;
        public ServiceResennaValoracion(IRepositoryResennaValoracion repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public async Task<ResennaValoracionDTO> FindByIdAsync(int id)
        {
            var @object = await _repository.FindByIdAsync(id);
            var objectMapped = _mapper.Map<ResennaValoracionDTO>(@object);
            return objectMapped;
        }
        public async Task<ICollection<ResennaValoracionDTO>> ListAsync()
        {
            //Obtener datos del repositorio
            var list = await _repository.ListAsync();
            // Map List<ResennaValoracion> a ICollection<ResennaValoracionDTO>
            var collection = _mapper.Map<ICollection<ResennaValoracionDTO>>(list);
            // Return lista
            return collection;
        }

        public async Task<ICollection<ResennaValoracionDTO>> GetResennasPorProducto(int idProducto)
        {
            //Obtener datos del repositorio
            var list = await _repository.GetResennasPorProducto(idProducto);
            // Map List<ResennaValoracion> a ICollection<ResennaValoracionDTO>
            var collection = _mapper.Map<ICollection<ResennaValoracionDTO>>(list);
            // Return lista
            return collection;
        }
    }
}
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
    public class ServiceEtiqueta : IServiceEtiqueta
    {
        private readonly IRepositoryEtiqueta _repository;
        private readonly IMapper _mapper;
        public ServiceEtiqueta(IRepositoryEtiqueta repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public async Task<EtiquetaDTO> FindByIdAsync(int id)
        {
            var @object = await _repository.FindByIdAsync(id);
            var objectMapped = _mapper.Map<EtiquetaDTO>(@object);
            return objectMapped;
        }
        public async Task<ICollection<EtiquetaDTO>> ListAsync()
        {
            //Obtener datos del repositorio
            var list = await _repository.ListAsync();
            // Map List<Etiqueta> a ICollection<EtiquetaDTO>
            var collection = _mapper.Map<ICollection<EtiquetaDTO>>(list);
            // Return lista
            return collection;
        }
    }
}
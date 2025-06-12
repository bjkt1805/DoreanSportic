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
    public class ServiceEmpaque : IServiceEmpaque
    {
        private readonly IRepositoryEmpaque _repository;
        private readonly IMapper _mapper;
        public ServiceEmpaque(IRepositoryEmpaque repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public async Task<EmpaqueDTO> FindByIdAsync(int id)
        {
            var @object = await _repository.FindByIdAsync(id);
            var objectMapped = _mapper.Map<EmpaqueDTO>(@object);
            return objectMapped;
        }
        public async Task<ICollection<EmpaqueDTO>> ListAsync()
        {
            //Obtener datos del repositorio
            var list = await _repository.ListAsync();
            // Map List<Empaque> a ICollection<EmpaqueDTO>
            var collection = _mapper.Map<ICollection<EmpaqueDTO>>(list);
            // Return lista
            return collection;
        }
    }
}
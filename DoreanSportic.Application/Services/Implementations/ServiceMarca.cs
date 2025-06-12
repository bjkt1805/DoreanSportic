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
    public class ServiceMarca : IServiceMarca
    {
        private readonly IRepositoryMarca _repository;
        private readonly IMapper _mapper;
        public ServiceMarca(IRepositoryMarca repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public async Task<MarcaDTO> FindByIdAsync(int id)
        {
            var @object = await _repository.FindByIdAsync(id);
            var objectMapped = _mapper.Map<MarcaDTO>(@object);
            return objectMapped;
        }
        public async Task<ICollection<MarcaDTO>> ListAsync()
        {
            //Obtener datos del repositorio
            var list = await _repository.ListAsync();
            // Map List<Marca> a ICollection<MarcaDTO>
            var collection = _mapper.Map<ICollection<MarcaDTO>>(list);
            // Return lista
            return collection;
        }
    }
}
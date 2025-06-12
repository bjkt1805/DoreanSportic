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
    public class ServiceSexo : IServiceSexo
    {
        private readonly IRepositorySexo _repository;
        private readonly IMapper _mapper;
        public ServiceSexo(IRepositorySexo repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public async Task<SexoDTO> FindByIdAsync(int id)
        {
            var @object = await _repository.FindByIdAsync(id);
            var objectMapped = _mapper.Map<SexoDTO>(@object);
            return objectMapped;
        }
        public async Task<ICollection<SexoDTO>> ListAsync()
        {
            //Obtener datos del repositorio
            var list = await _repository.ListAsync();
            // Map List<Sexo> a ICollection<SexoDTO>
            var collection = _mapper.Map<ICollection<SexoDTO>>(list);
            // Return lista
            return collection;
        }
    }
}
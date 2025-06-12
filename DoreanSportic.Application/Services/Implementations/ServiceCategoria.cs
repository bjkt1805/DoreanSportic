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
    public class ServiceCategoria : IServiceCategoria
    {
        private readonly IRepositoryCategoria _repository;
        private readonly IMapper _mapper;
        public ServiceCategoria(IRepositoryCategoria repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public async Task<CategoriaDTO> FindByIdAsync(int id)
        {
            var @object = await _repository.FindByIdAsync(id);
            var objectMapped = _mapper.Map<CategoriaDTO>(@object);
            return objectMapped;
        }
        public async Task<ICollection<CategoriaDTO>> ListAsync()
        {
            //Obtener datos del repositorio
            var list = await _repository.ListAsync();
            // Map List<Categoria> a ICollection<CategoriaDTO>
            var collection = _mapper.Map<ICollection<CategoriaDTO>>(list);
            // Return lista
            return collection;
        }
    }
}
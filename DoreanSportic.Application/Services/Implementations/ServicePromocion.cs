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
    public class ServicePromocion : IServicePromocion
    {
        private readonly IRepositoryPromocion _repository;
        private readonly IMapper _mapper;
        public ServicePromocion(IRepositoryPromocion repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public async Task<PromocionDTO> FindByIdAsync(int id)
        {
            var @object = await _repository.FindByIdAsync(id);
            var objectMapped = _mapper.Map<PromocionDTO>(@object);
            return objectMapped;
        }
        public async Task<ICollection<PromocionDTO>> ListAsync()
        {
            //Obtener datos del repositorio
            var list = await _repository.ListAsync();
            // Map List<Promocion> a ICollection<PromocionDTO>
            var collection = _mapper.Map<ICollection<PromocionDTO>>(list);
            // Return lista
            return collection;
        }
    }
}
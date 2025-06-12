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
    public class ServicePedido : IServicePedido
    {
        private readonly IRepositoryPedido _repository;
        private readonly IMapper _mapper;
        public ServicePedido(IRepositoryPedido repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public async Task<PedidoDTO> FindByIdAsync(int id)
        {
            var @object = await _repository.FindByIdAsync(id);
            var objectMapped = _mapper.Map<PedidoDTO>(@object);
            return objectMapped;
        }
        public async Task<ICollection<PedidoDTO>> ListAsync()
        {
            //Obtener datos del repositorio
            var list = await _repository.ListAsync();
            // Map List<Pedido> a ICollection<PedidoDTO>
            var collection = _mapper.Map<ICollection<PedidoDTO>>(list);
            // Return lista
            return collection;
        }
    }
}
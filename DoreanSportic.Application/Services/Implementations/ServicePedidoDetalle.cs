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
    public class ServicePedidoDetalle : IServicePedidoDetalle
    {
        private readonly IRepositoryPedidoDetalle _repository;
        private readonly IMapper _mapper;
        public ServicePedidoDetalle(IRepositoryPedidoDetalle repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public async Task<PedidoDetalleDTO> FindByIdAsync(int id)
        {
            var @object = await _repository.FindByIdAsync(id);
            var objectMapped = _mapper.Map<PedidoDetalleDTO>(@object);
            return objectMapped;
        }
        public async Task<ICollection<PedidoDetalleDTO>> ListAsync()
        {
            //Obtener datos del repositorio
            var list = await _repository.ListAsync();
            // Map List<PedidoDetalle> a ICollection<PedidoDetalleDTO>
            var collection = _mapper.Map<ICollection<PedidoDetalleDTO>>(list);
            // Return lista
            return collection;
        }

        public async Task<ICollection<PedidoDetalleDTO>> GetDetallesPorPedido(string idPedido)
        {
            //Obtener datos del repositorio
            var list = await _repository.GetDetallesPorPedido(idPedido);
            // Map List<PedidoDetalle> a ICollection<PedidoDetalleDTO>
            var collection = _mapper.Map<ICollection<PedidoDetalleDTO>>(list);

            // Return lista
            return collection;
        }
    }
}
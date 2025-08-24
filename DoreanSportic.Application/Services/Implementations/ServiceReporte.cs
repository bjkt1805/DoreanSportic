using DoreanSportic.Application.Services.Interfaces;
using DoreanSportic.Infrastructure.Repository.Interfaces;
using System;
using System.Threading.Tasks;

namespace DoreanSportic.Application.Services.Implementations
{
    public class ServiceReporte : IServiceReporte
    {
        private readonly IRepositoryReporte _repository;

        public ServiceReporte(IRepositoryReporte repository)
        {
            _repository = repository;
        }

        public Task<object> VentasPorDiaAsync(DateTime from, DateTime to)
        {
            return _repository.VentasPorDiaAsync(from, to);
        }

        public Task<object> VentasPorMesAsync(int year)
        {
            return _repository.VentasPorMesAsync(year);
        }

        public Task<object> PedidosPorEstadoAsync()
        {
            return _repository.PedidosPorEstadoAsync();
        }

        public Task<object> TopProductosAsync(int n)
        {
            return _repository.TopProductosAsync(n);
        }

        public Task<object> ResennasRecientesAsync(int n)
        {
            return _repository.ResennasRecientesAsync(n);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoreanSportic.Infrastructure.Repository.Interfaces
{
    public interface IRepositoryReporte
    {
        Task<object> VentasPorDiaAsync(DateTime from, DateTime to);
        Task<object> VentasPorMesAsync(int year);
        Task<object> PedidosPorEstadoAsync();
        Task<object> TopProductosAsync(int n);
        Task<object> ResennasRecientesAsync(int n);
    }
}

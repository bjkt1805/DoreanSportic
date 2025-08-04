using DoreanSportic.Application.DTOs;
using DoreanSportic.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace DoreanSportic.Application.Services.Interfaces
{
    public interface IServiceCarritoDetalle
    {
        Task<ICollection<CarritoDetalleDTO>> ListAsync();
        Task<CarritoDetalleDTO> FindByIdAsync(int id);
        Task<int> AddAsync(CarritoDetalleDTO dto);
        Task<List<CarritoDetalleDTO>> GetByCarritoIdAsync(int idCarrito);

    }
}
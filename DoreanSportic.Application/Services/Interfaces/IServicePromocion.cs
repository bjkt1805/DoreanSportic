using DoreanSportic.Application.DTOs;
using DoreanSportic.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace DoreanSportic.Application.Services.Interfaces
{
    public interface IServicePromocion
    {
        // Listado de promociones 
        Task<ICollection<PromocionDTO>> ListAsync();
        // Listado de promociones por ID (detalle)
        Task<PromocionDTO> FindByIdAsync(int id);
        // Borrar la promoción (por su ID)
        Task DeleteAsync(int id);
        // Actualizar la promoción
        Task UpdateAsync(int id, PromocionDTO dto);
    }
}
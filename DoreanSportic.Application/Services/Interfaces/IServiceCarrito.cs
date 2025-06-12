using DoreanSportic.Application.DTOs;
using DoreanSportic.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace DoreanSportic.Application.Services.Interfaces
{
    public interface IServiceCarrito
    {
        Task<ICollection<CarritoDTO>> ListAsync();
        Task<CarritoDTO> FindByIdAsync(int id);
    }
}
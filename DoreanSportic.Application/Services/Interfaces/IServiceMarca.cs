using DoreanSportic.Application.DTOs;
using DoreanSportic.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace DoreanSportic.Application.Services.Interfaces
{
    public interface IServiceMarca
    {
        Task<ICollection<MarcaDTO>> ListAsync();
        Task<MarcaDTO> FindByIdAsync(int id);
    }
}
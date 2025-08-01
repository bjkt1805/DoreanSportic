﻿using DoreanSportic.Application.DTOs;
using DoreanSportic.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace DoreanSportic.Application.Services.Interfaces
{
    public interface IServiceResennaValoracion
    {
        Task<ICollection<ResennaValoracionDTO>> ListAsync();

        Task<ICollection<ResennaValoracionDTO>> GetResennasPorProducto(int idProducto);

        Task<ResennaValoracionDTO> FindByIdAsync(int id);

        Task<int> AddAsync(ResennaValoracionDTO dto);
    }
}
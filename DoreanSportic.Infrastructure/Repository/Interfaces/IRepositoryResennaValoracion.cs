﻿using DoreanSportic.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoreanSportic.Infrastructure.Repository.Interfaces
{
    public interface IRepositoryResennaValoracion
    {
        Task<ICollection<ResennaValoracion>> ListAsync();
        Task<ICollection<ResennaValoracion>> GetResennasPorProducto(int idProducto);

        Task<ResennaValoracion> FindByIdAsync(int id);

        Task<int> AddAsync(ResennaValoracion entity);
    }
}

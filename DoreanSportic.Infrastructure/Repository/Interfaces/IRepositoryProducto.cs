﻿using DoreanSportic.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoreanSportic.Infrastructure.Repository.Interfaces
{
    public interface IRepositoryProducto
    {
        Task<ICollection<Producto>> ListAsync();

        Task<ICollection<Producto>> GetProductoByCategoria(int idCategoria);
        Task<Producto> FindByIdAsync(int id);
        Task<int> AddAsync(Producto entity);
        Task UpdateAsync(Producto entity);


    }
}

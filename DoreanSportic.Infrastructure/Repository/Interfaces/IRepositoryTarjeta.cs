﻿using DoreanSportic.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoreanSportic.Infrastructure.Repository.Interfaces
{
    public interface IRepositoryTarjeta
    {
        Task<ICollection<Tarjeta>> ListAsync();
        Task<Tarjeta> FindByIdAsync(int id);
    }
}

﻿using DoreanSportic.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoreanSportic.Infrastructure.Repository.Interfaces
{
    public interface IRepositoryEmpaque
    {
        Task<ICollection<Empaque>> ListAsync();
        Task<Empaque> FindByIdAsync(int id);
    }
}

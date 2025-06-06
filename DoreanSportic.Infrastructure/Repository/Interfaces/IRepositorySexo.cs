using DoreanSportic.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoreanSportic.Infrastructure.Repository.Interfaces
{
    internal interface IRepositorySexo
    {
        Task<ICollection<Sexo>> ListAsync();
        Task<Sexo> FindByIdAsync(int id);
    }
}

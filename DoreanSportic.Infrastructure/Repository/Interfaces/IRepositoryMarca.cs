using DoreanSportic.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoreanSportic.Infrastructure.Repository.Interfaces
{
    internal interface IRepositoryMarca
    {
        Task<ICollection<Marca>> ListAsync();
        Task<Marca> FindByIdAsync(int id);
    }
}

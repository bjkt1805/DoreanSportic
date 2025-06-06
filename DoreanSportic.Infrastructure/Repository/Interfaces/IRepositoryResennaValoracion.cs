using DoreanSportic.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoreanSportic.Infrastructure.Repository.Interfaces
{
    internal interface IRepositoryResennaValoracion
    {
        Task<ICollection<ResennaValoracion>> ListAsync();
        Task<ResennaValoracion> FindByIdAsync(int id);
    }
}

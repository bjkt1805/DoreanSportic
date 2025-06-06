using DoreanSportic.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoreanSportic.Infrastructure.Repository.Interfaces
{
    internal interface IRepositoryCarritoDetalle
    {
        Task<ICollection<CarritoDetalle>> ListAsync();
        Task<CarritoDetalle> FindByIdAsync(int id);

    }
}

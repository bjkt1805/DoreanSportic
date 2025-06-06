using DoreanSportic.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoreanSportic.Infrastructure.Repository.Interfaces
{
    internal interface IRepositoryCarrito
    {
        Task<ICollection<Carrito>> ListAsync();
        Task<Carrito> FindByIdAsync(int id);

    }
}

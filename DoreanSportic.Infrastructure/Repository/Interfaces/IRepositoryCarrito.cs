using DoreanSportic.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoreanSportic.Infrastructure.Repository.Interfaces
{
    public interface IRepositoryCarrito
    {
        Task<ICollection<Carrito>> ListAsync();
        Task<Carrito> FindByIdAsync(int id);

    }
}

using DoreanSportic.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoreanSportic.Infrastructure.Repository.Interfaces
{
    public interface IRepositoryCarritoDetalle
    {
        Task<ICollection<CarritoDetalle>> ListAsync();
        Task<CarritoDetalle> FindByIdAsync(int id);
        Task<int> AddAsync(CarritoDetalle entity);

        Task<List<CarritoDetalle>> GetByCarritoIdAsync(int idCarrito);
    }
}

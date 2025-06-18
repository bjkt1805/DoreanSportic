using DoreanSportic.Application.DTOs;
using DoreanSportic.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace DoreanSportic.Application.Services.Interfaces
{
    public interface IServiceProducto
    {
        // Listado de productos 
        Task<ICollection<ProductoDTO>> ListAsync();


        // Obtener los productos por categoría
        Task<ICollection<ProductoDTO>> GetProductoByCategoria(int IdCategoria);

        // Listado de producto por ID (detalle)
        Task<ProductoDTO> FindByIdAsync(int id);

        // Borrar el producto (por su ID)
        Task DeleteAsync(int id);

        // Actualizar el producto
        Task UpdateAsync(int id, ProductoDTO dto, string[] selectedCategorias);


    }
}
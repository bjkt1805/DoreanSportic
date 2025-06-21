using AutoMapper;
using DoreanSportic.Application.DTOs;
using DoreanSportic.Application.Services.Interfaces;
using DoreanSportic.Infrastructure.Models;
using DoreanSportic.Infrastructure.Repository.Implementations;
using DoreanSportic.Infrastructure.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace DoreanSportic.Application.Services.Implementations
{
    public class ServiceProducto : IServiceProducto
    {
        private readonly IRepositoryProducto _repository;
        private readonly IMapper _mapper;
        public ServiceProducto(IRepositoryProducto repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public Task DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<ProductoDTO> FindByIdAsync(int id)
        {
            var @object = await _repository.FindByIdAsync(id);
            var objectMapped = _mapper.Map<ProductoDTO>(@object);
            return objectMapped;
        }

        public async Task<ICollection<ProductoDTO>> ListAsync()
        {
            //Obtener datos del repositorio
            var list = await _repository.ListAsync();
            // Map List<Producto> a ICollection<ProductoDTO>
            var collection = _mapper.Map<ICollection<ProductoDTO>>(list);
            // Return lista
            return collection;
        }

        public async Task<ICollection<ProductoDTO>> GetProductoByCategoria(int IdCategoria)
        {
            var list = await _repository.GetProductoByCategoria(IdCategoria);

            //var collection = _mapper.Map<ICollection<ProductoDTO>>(list);

            // Para poder traer la primera imagen del producto
            // (la cual no está mappeada en la base de datos)
            var collection = list.Select(p => new ProductoDTO
            {
                Id = p.Id,
                Nombre = p.Nombre,
                Descripcion = p.Descripcion,
                PrecioBase = p.PrecioBase,
                Stock = p.Stock,
                IdMarcaNavigation = p.IdMarcaNavigation,
                IdCategoriaNavigation = p.IdCategoriaNavigation,
                IdCategoria = p.IdCategoria,
                Estado = p.Estado,
                // Traer la primera imagen del producto
                PrimeraImagen = p.PrimeraImagen,
                IdPromocion = p.IdPromocion
            }).ToList();

            return collection;
        }

        public Task UpdateAsync(int id, ProductoDTO dto, string[] selectedCategorias)
        {
            throw new NotImplementedException();
        }
    }
}
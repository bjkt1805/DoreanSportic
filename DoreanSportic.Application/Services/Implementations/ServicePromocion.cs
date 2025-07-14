using AutoMapper;
using DoreanSportic.Application.DTOs;
using DoreanSportic.Application.Services.Interfaces;
using DoreanSportic.Infrastructure.Models;
using DoreanSportic.Infrastructure.Repository.Implementations;
using DoreanSportic.Infrastructure.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace DoreanSportic.Application.Services.Implementations
{
    public class ServicePromocion : IServicePromocion
    {
        private readonly IRepositoryPromocion _repository;
        private readonly IMapper _mapper;
        public ServicePromocion(IRepositoryPromocion repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public Task DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<PromocionDTO> FindByIdAsync(int id)
        {
            var @object = await _repository.FindByIdAsync(id);
            var objectMapped = _mapper.Map<PromocionDTO>(@object);

            // Mapear productos seleccionados
            objectMapped.IdProductosSeleccionados = @object.IdProducto
                .Select(p => p.Id)
                .ToList();

            // Mapear categoría seleccionada
            objectMapped.IdCategoriaSeleccionada = @object.IdCategoria.FirstOrDefault()?.Id;

            return objectMapped;
        }

        public async Task<ICollection<PromocionDTO>> ListAsync()
        {
            //Obtener datos del repositorio
            var list = await _repository.ListAsync();
            // Map List<Producto> a ICollection<ProductoDTO>
            var collection = _mapper.Map<ICollection<PromocionDTO>>(list);
            // Return lista
            return collection;
        }
        public async Task<int> AddAsync(PromocionDTO dto,List<int> listaProductosSeleccionados)
        {
            var objectMapped = _mapper.Map<Promocion>(dto);
            return await _repository.AddAsync(objectMapped, listaProductosSeleccionados);
        }
        public async Task UpdateAsync(PromocionDTO dto)
        {
            // Obtener la categoría seleccionada (si aplica)
            var nuevasCategorias = new List<Categoria>();

            // Obtener los productos seleccionados (si hay)
            var nuevosProductos = new List<Producto>();

            // Obtener la promoción desde la base (opcional, si ya no se usa directamente)
            var promocionExistente = await _repository.FindByIdAsync(dto.Id);
            if (promocionExistente == null)
            {
                throw new Exception("Promoción no encontrada");
            }

            // Detectar si el estado cambió de activo (true) a inactivo (false)
            bool cambioEstadoAInactivo = promocionExistente.Estado && !dto.Estado;

            //Mapeo manual
            promocionExistente.Nombre = dto.Nombre;
            promocionExistente.Descripcion = dto.Descripcion;
            promocionExistente.PorcentajeDescuento = dto.PorcentajeDescuento;
            promocionExistente.FechaInicio = dto.FechaInicio;
            promocionExistente.FechaFin = dto.FechaFin;
            promocionExistente.Estado = dto.Estado;

            if (cambioEstadoAInactivo)
            {
                // Si se desactiva la promoción, eliminar relaciones
                // promocion_producto o promocion_categoria
                promocionExistente.IdProducto.Clear();
                promocionExistente.IdCategoria.Clear();
            }

            else
            {
                if (dto.IdCategoriaSeleccionada.HasValue)
                {
                    var categoria = await _repository.ObtenerCategoriaPorIdAsync(dto.IdCategoriaSeleccionada.Value);
                    if (categoria != null)
                    {
                        nuevasCategorias.Add(categoria);
                    }
                }

                if (dto.IdProductosSeleccionados != null && dto.IdProductosSeleccionados.Any())
                {
                    nuevosProductos = (await _repository.ObtenerProductosPorIdsAsync(dto.IdProductosSeleccionados)).ToList();
                }
            }

            // Llamar al repositorio con DTO + relaciones listas
            await _repository.UpdateAsync(promocionExistente, nuevasCategorias, nuevosProductos);
        }

    }
}
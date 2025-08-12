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
    public class ServicePedidoDetalle : IServicePedidoDetalle
    {
        private readonly IRepositoryPedidoDetalle _repository;
        private readonly IMapper _mapper;
        public ServicePedidoDetalle(IRepositoryPedidoDetalle repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public async Task<PedidoDetalleDTO> FindByIdAsync(int id)
        {
            var @object = await _repository.FindByIdAsync(id);
            var objectMapped = _mapper.Map<PedidoDetalleDTO>(@object);
            return objectMapped;
        }
        public async Task<ICollection<PedidoDetalleDTO>> ListAsync()
        {
            //Obtener datos del repositorio
            var list = await _repository.ListAsync();
            // Map List<PedidoDetalle> a ICollection<PedidoDetalleDTO>
            var collection = _mapper.Map<ICollection<PedidoDetalleDTO>>(list);
            // Return lista
            return collection;
        }

        public async Task<int> AddAsync(PedidoDetalleDTO dto)
        {
            var objectMapped = _mapper.Map<PedidoDetalle>(dto);
            return await _repository.AddAsync(objectMapped);
        }
        public async Task<ICollection<PedidoDetalleDTO>> GetDetallesPorPedido(int idPedido)
        {
            //Obtener datos del repositorio
            var list = await _repository.GetDetallesPorPedido(idPedido);
            // Map List<PedidoDetalle> a ICollection<PedidoDetalleDTO>
            var collection = _mapper.Map<ICollection<PedidoDetalleDTO>>(list);

            // Return lista
            return collection;
        }

        public async Task<List<PedidoDetalleDTO>> GetByPedidoIdAsync(int idCarrito)
        {
            var detalles = await _repository.GetByPedidoIdAsync(idCarrito);
            return _mapper.Map<List<PedidoDetalleDTO>>(detalles);
        }

        // Método para actualizar la cantidad de productos en un detalle del pedido
        public async Task<(PedidoDetalleDTO? det, bool eliminado, int? pedidoId)> ActualizarCantidadAsync(int detalleId, int nuevaCantidad)
        {
            // obtener el pedidoId antes de hacer cambios en el detalle
            var pedidoId = await _repository.GetPedidoIdByDetalleAsync(detalleId);
            
            // Si la nueva cantidad es 0 o menor, eliminar el detalle del pedido
            if (nuevaCantidad <= 0)
            {
                await _repository.RemoveAsync(detalleId);

                // Retornar null, indicando que el detalle fue eliminado, y el pedidoId
                return (null, true, pedidoId);
            }

            // Obtener el detalle del pedido por ID
            var detalle = await _repository.FindByIdAsync(detalleId);

            // ---- Cálculo del precio unitario con promociones (igual que en la vista parcial de detalles, para poder incluir
            // precios de descuento (si aplican)----

            // Obtener el producto y calcular el precio unitario con descuentos
            var producto = detalle.IdProductoNavigation!;
            var hoy = DateTime.Today;
            var precioBase = producto.PrecioBase;

            // Verificar si el producto tiene promociones activas
            var promoProducto = producto.IdPromocion?.FirstOrDefault(p => p.FechaInicio <= hoy && p.FechaFin >= hoy);
            var descProd = promoProducto?.PorcentajeDescuento ?? 0;

            // Verificar si la categoría del producto tiene promociones activas
            var promoCat = producto.IdCategoriaNavigation?.IdPromocion?.FirstOrDefault(p => p.FechaInicio <= hoy && p.FechaFin >= hoy);
            var descCat = promoCat?.PorcentajeDescuento ?? 0;

            // Calcular el descuento total (producto + categoría, máximo 100%)
            var descTotal = Math.Min(100, descProd + descCat);
            var precioUnit = descTotal > 0 ? (precioBase - (precioBase * descTotal / 100m)) : precioBase;

            // Personalizaciones (si aplican en el modelo)
            decimal extras = 0m;
            if (detalle.IdEmpaqueNavigation != null)
            {
                decimal mensaje = 1000m;
                decimal foto = 1500m;
                extras += (detalle.IdEmpaqueNavigation.PrecioBase ?? 0m) + mensaje + foto;
            }

            // Calcular el subtotal, impuesto y total
            var precioUnitFinal = precioUnit + extras;
            var sub = precioUnitFinal * nuevaCantidad;
            var imp = Math.Round(sub * 0.13m, 2); // IVA 13%
            var tot = sub + imp;

            // Actualizar la cantidad, subtotal, impuesto y total en el repositorio
            await _repository.UpdateCantidadAsync(detalleId, nuevaCantidad, sub);

            var actualizado = await _repository.FindByIdAsync(detalleId);
            var dto = _mapper.Map<PedidoDetalleDTO>(actualizado);

            return (dto, false, pedidoId);
        }

        // Método para eliminar un detalle de pedido
        public Task EliminarAsync(int detalleId) => _repository.RemoveAsync(detalleId);

        public Task<int?> GetPedidoIdByDetalleAsync(int detalleId) => _repository.GetPedidoIdByDetalleAsync(detalleId);
    }
}
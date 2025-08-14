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
    public class ServicePedido : IServicePedido
    {
        private readonly IRepositoryPedido _repository;
        private readonly IRepositoryPedidoDetalle _repositoryDetalle;
        private readonly IRepositoryProducto _repositoryProducto;
        private readonly IMapper _mapper;
        public ServicePedido(IRepositoryPedido repository, 
            IRepositoryPedidoDetalle repositoryDetalle, 
            IRepositoryProducto repositoryProducto,
            IMapper mapper)
        {
            _repository = repository;
            _repositoryDetalle = repositoryDetalle;
            _repositoryProducto = repositoryProducto;
            _mapper = mapper;
        }
        public async Task<PedidoDTO> FindByIdAsync(int id)
        {
            var @object = await _repository.FindByIdAsync(id);
            var objectMapped = _mapper.Map<PedidoDTO>(@object);
            return objectMapped;
        }
        public async Task<ICollection<PedidoDTO>> ListAsync()
        {
            //Obtener datos del repositorio
            var list = await _repository.ListAsync();
            // Map List<Pedido> a ICollection<PedidoDTO>
            var collection = _mapper.Map<ICollection<PedidoDTO>>(list);
            // Return lista
            return collection;
        }

        public async Task<int> AddAsync(PedidoDTO dto)
        {
            var objectMapped = _mapper.Map<Pedido>(dto);
            return await _repository.AddAsync(objectMapped);
        }

        // Método para actualizar el encabezado del pedido (dirección de envío, etc)
        public async Task<bool> ActualizarEncabezadoAsync(int pedidoId, int clienteId, string? direccionEnvio)
        {
            // Actlizar el encabezado del pedido
            await _repository.UpdateHeaderAsync(pedidoId, clienteId, direccionEnvio);
            return true;
        }

        // Método para actualizar los totales y el estado de pago del pedido
        public async Task<(decimal sub, decimal imp, decimal total)> RecalcularTotalesAsync(int pedidoId)
        {
            var detalles = await _repositoryDetalle.GetByPedidoIdAsync(pedidoId);
            var subTotal = detalles.Sum(d => d.SubTotal);

            // Calcular impuesto y total
            var imp = Math.Round(subTotal * 0.13m, 2);
            var total = subTotal + imp;

            // Persistir solo el subtotal
            await _repository.UpdateTotalsAndStateAsync(pedidoId, subTotal, imp, total);

            return (subTotal, imp, total);
        }

        public async Task<(bool ok, List<(int detalleId, string nombre, int stockDisp, int cant)> errores)>
            ValidarStockAsync(int pedidoId)
        {
            var detalles = await _repositoryDetalle.GetByPedidoIdAsync(pedidoId);
            var errores = new List<(int, string, int, int)>();
            foreach (var detalle in detalles.Where(x => x.IdProducto != 0))
            {
                var p = await _repositoryProducto.FindByIdAsync(detalle.IdProducto);
                if (p == null) { errores.Add((detalle.Id, "Producto no existe", 0, detalle.Cantidad)); continue; }
                if (p.Stock < detalle.Cantidad) errores.Add((detalle.Id, p.Nombre, p.Stock, detalle.Cantidad));
            }
            return (errores.Count == 0, errores);
        }

        public async Task<bool> ConfirmarAsync(int pedidoId)
        {
            // Verificar si el pedido tiene detalles
            if (!await _repository.AnyDetalleAsync(pedidoId)) return false;

            // Validar stock de los detalles del pedido
            var validacionStock = await ValidarStockAsync(pedidoId);

            // Si hay errores de stock, no se puede confirmar el pedido
            if (!validacionStock.ok) return false;

            // Obtener detalles del pedido
            var detalles = await _repositoryDetalle.GetByPedidoIdAsync(pedidoId);
            
            // Descontar stock (solo líneas con producto)
            foreach (var detalle in detalles.Where(x => x.IdProducto != 0))
            {
                var ok = await _repositoryProducto.TryDescontarStockAsync(detalle.IdProducto, detalle.Cantidad);
                if (!ok) throw new InvalidOperationException("Race condition de stock.");
            }
            // EstadoPago = "Registrado" (o el que uses)
            var totals = await RecalcularTotalesAsync(pedidoId);
            await _repository.UpdateTotalsAndStateAsync(pedidoId, totals.sub, totals.imp, totals.total, "Pagado");
            return true;
        }
    }
}
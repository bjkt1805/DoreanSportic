using DoreanSportic.Application.DTOs;
using DoreanSportic.Application.Services.Implementations;
using DoreanSportic.Application.Services.Interfaces;
using DoreanSportic.Infrastructure.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
// Solo permitir acceso a usuarios autenticados
using Microsoft.AspNetCore.Authorization;
using X.PagedList.Extensions;

namespace DoreanSportic.Controllers
{
    [Authorize] // Asegurar que el usuario esté autenticado para acceder a los métodos de este controlador
    public class PedidoController : Controller
    {
        private readonly IServicePedido _servicePedido;
        private readonly IServicePedidoDetalle _servicePedidoDetalle;

        public PedidoController(IServicePedido servicePedido, IServicePedidoDetalle servicePedidoDetalle)
        {
            _servicePedido = servicePedido;
            _servicePedidoDetalle = servicePedidoDetalle;
        }

        // GET: PedidoController
        public async Task<ActionResult> Index(int? page)
        {
            var collection = await _servicePedido.ListAsync();
            return View(collection.ToPagedList(page ?? 1, 5));
        }

        // GET: ProductoController para el ADMIN

        //public async Task<ActionResult> IndexAdmin(int? page)
        //{
        //    var collection = await _serviceLibro.ListAsync();
        //    return View(collection.ToPagedList(page ?? 1, 5));
        //}

        // GET: PedidoController/Details/5
        public async Task<ActionResult> Details(int id)
        {
            var @object = await _servicePedido.FindByIdAsync(id);
            return View(@object);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActualizarEncabezado([FromBody] ActualizarEncabezadoReq req)
        {
            // userId aquí es el IdCliente
            var ok = await _servicePedido.ActualizarEncabezadoAsync(req.PedidoId, req.UserId, req.DireccionEnvio);
            if (!ok) return Json(new { success = false, mensaje = "No fue posible guardar" });

            // devolver estado y totales actuales (opc.)
            var (sub, imp, total) = await _servicePedido.RecalcularTotalesAsync(req.PedidoId);
            return Json(new { success = true, estadoNombre = "EstadoPago", totals = new { sub, imp, total } });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Confirmar(int pedidoId)
        {
            var (ok, errores) = await _servicePedido.ValidarStockAsync(pedidoId);
            if (!ok) return Json(new { success = false, errores = errores.Select(e => new { e.detalleId, e.nombre, e.stockDisp, e.cant }) });

            var confirmado = await _servicePedido.ConfirmarAsync(pedidoId);
            if (!confirmado) return Json(new { success = false, mensaje = "No fue posible confirmar el pedido" });

            // Tras confirmar, totales ya quedan persistidos con estado.
            var (sub, imp, total) = await _servicePedido.RecalcularTotalesAsync(pedidoId);
            return Json(new { success = true, mensaje = "Pedido registrado", totals = new { sub, imp, total } });
        }

        public record ActualizarEncabezadoReq
        {
            public int PedidoId { get; init; }
            public int UserId { get; init; }
            public string? DireccionEnvio { get; init; }
        }

    }
}

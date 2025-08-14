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


        // Record (datos) para actualizar el encabezado del pedido
        public record ActualizarEncabezadoReq
        {
            public int PedidoId { get; init; }
            public int UserId { get; init; }

            public int MetodoPago { get; init; }
            public string? DireccionEnvio { get; init; }
        }

        // Record (datos) para completar la compra
        public record CompletarCompraRequest
        {
            public int PedidoId { get; set; }
            public string? DireccionEnvio { get; set; }
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
        public async Task<ActionResult> DetailsEditable(int id)
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


        // Completar la compra: valida stock, descuenta stock (dentro de ConfirmarAsync), 
        // actualiza dirección, cambia estado y devuelve totales.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CompletarCompra([FromBody] CompletarCompraRequest req)
        {
            // Validar que la solicitud no sea nula y que el PedidoId sea válido
            if (req is null || req.PedidoId <= 0)
                return Json(new { success = false, mensaje = "Solicitud inválida." });

            // Verificar que venga dirección de envío para actualizar el pedido (encabezado)
            if (!string.IsNullOrWhiteSpace(req.DireccionEnvio))
            {

                // Llamar al servicio para actualizar el encabezado del pedido
                var actualizado = await _servicePedido.ActualizarEncabezadoAsync(
                    pedidoId: req.PedidoId,
                    userId: 0,
                    direccionEnvio: req.DireccionEnvio
                );

                // Si no se pudo actualizar el encabezado, devolver un mensaje de error
                if (!actualizado)
                    return Json(new { success = false, mensaje = "No fue posible guardar la dirección de envío." });
            }

            // Validar stock de los detalles del pedido
            var (okStock, errores) = await _servicePedido.ValidarStockAsync(req.PedidoId);

            // Si no hay stock suficiente, devolver la lista de errores
            if (!okStock)
            {
                // Devolver la lista de errores en formato JSON
                return Json(new
                {
                    success = false,

                    // Mensaje de error general
                    errores = errores.Select(e => new
                    {
                        e.detalleId,
                        e.nombre,
                        e.stockDisp,
                        e.cant
                    })
                });
            }

            // Confirmar el pedido: descuenta stock y cambia estado
            var confirmado = await _servicePedido.ConfirmarAsync(req.PedidoId);

            // Si no se pudo confirmar el pedido, devolver un mensaje de error
            if (!confirmado)
                return Json(new { success = false, mensaje = "No fue posible completar la compra." });

            // Si todo salió bien, recalcular los totales del pedido
            var (sub, imp, total) = await _servicePedido.RecalcularTotalesAsync(req.PedidoId);

            // Devolver la respuesta JSON con el éxito de la operación y los totales
            return Json(new
            {
                success = true,
                mensaje = "Compra completada.",
                totals = new { sub, imp, total }
            });
        }



    }
}

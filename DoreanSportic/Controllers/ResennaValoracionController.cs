using DoreanSportic.Application.DTOs;
using DoreanSportic.Application.Services.Implementations;
using DoreanSportic.Application.Services.Interfaces;
using DoreanSportic.Infrastructure.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.SqlServer.Server;
using System.Security.Claims;
using X.PagedList.Extensions;

namespace DoreanSportic.Web.Controllers
{
    public class ResennaValoracionController : Controller
    {
        private readonly IServiceResennaValoracion _serviceResennaValoracion;
        private readonly IServicePedido _servicePedido;

        public ResennaValoracionController(IServiceResennaValoracion serviceResennaValoracion, IServicePedido servicePedido)
        {
            _serviceResennaValoracion = serviceResennaValoracion;
            _servicePedido = servicePedido;
        }

        // GET: ResennaValoracionController
        public async Task<ActionResult> Index(int? page)
        {
            var collection = await _serviceResennaValoracion.ListAsync();
            return View(collection.ToPagedList(page ?? 1, 5));
        }

        // GET: ResennaValoracionController DASHBOARDADMIN
        public async Task<ActionResult> IndexAdmin(int? page)
        {
            var collection = await _serviceResennaValoracion.ListAsync();
            return PartialView("_IndexAdmin", collection.ToPagedList(page ?? 1, 5));
        }


        // GET: ResennaValoracionController/TablaAdmin
        [HttpGet]
        public async Task<IActionResult> TablaAdmin(int? page)
        {
            var data = await _serviceResennaValoracion.ListAsync();
            return PartialView("_TablaResennas", data.ToPagedList(page ?? 1, 5));
        }

        // GET: ResennaValoracionController 
        [HttpGet]
        public async Task<ActionResult> GetResennasPorProducto(int idProducto, int? calificacion)
        {
            // Listar las reseñas asociadas a un producto
            var collection = await _serviceResennaValoracion.GetResennasPorProducto(idProducto, calificacion);

            return PartialView("_ResennasProducto", collection);
        }

        // GET: ResennaValoracionController ADMIN
        [HttpGet]
        public async Task<ActionResult> GetResennasPorProductoAdmin(int idProducto)
        {
            // Listar las reseñas asociadas a un producto
            var collection = await _serviceResennaValoracion.GetResennasPorProducto(idProducto);

            return PartialView("_ResennasProductoVistaEditProducto", collection);
        }
        public async Task<ActionResult> Details(int id)
        {
            var @object = await _serviceResennaValoracion.FindByIdAsync(id);
            return View(@object);
        }

        // GET: ResennaValoracionController/DetailsAdmin/{id}
        public async Task<ActionResult> DetailsAdmin(int id)
        {
            var @object = await _serviceResennaValoracion.FindByIdAsync(id);
            return PartialView("_DetailsAdmin", @object);
        }

        // GET: ResennaValoracionController/GetPromedioPorProducto/{idProducto}
        [HttpGet]
        public async Task<IActionResult> GetPromedioPorProducto(int idProducto)
        {
            var resennas = await _serviceResennaValoracion.GetResennasPorProducto(idProducto);
            return PartialView("_PromedioCalificacion", resennas);
        }

        // POST: ResennaValoracion/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ResennaValoracionDTO dto)
        {
            // Validar el modelo
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Debe estar autenticado
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var rolClaim = User.FindFirstValue(ClaimTypes.Role);
            if (string.IsNullOrWhiteSpace(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                return Forbid();

            // Solo rol Cliente puede reseñar
            if (rolClaim != "2")
                return Forbid();

            // Blindaje: forzar que la reseña sea del mismo usuario autenticado
            dto.IdUsuario = userId;

            // Debe haber comprado el producto (pedido pagado con ese producto)
            var compro = await _servicePedido.UsuarioComproProductoAsync(userId, dto.IdProducto);
            if (!compro)
                return BadRequest(new { error = "Debe haber comprado este producto para dejar una reseña" });


            // Revisar que si se hecho una reseña para ese producto
            var yaExiste = await _serviceResennaValoracion.ExistsByUserProductAsync(userId, dto.IdProducto);
            if (yaExiste)
                return BadRequest(new { error = "Ya registraste una reseña para este producto." });

            // Guardar en BD
            await _serviceResennaValoracion.AddAsync(dto); 

            return Ok(new { success = true });
        }

        // GET: ResennaValoracionController/ResennaStats
        [HttpGet]
        public async Task<IActionResult> ResennaStats()
        {
            var stats = await _serviceResennaValoracion.GetStatsAsync();
            return Json(stats);
        }

        // POST: ResennaValoracionController/Report
        // Reportar una reseña (solo usuarios autenticados)
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Report(int idResenna, string? observacion)
        {
            // Id y nombre del usuario que REPORTA (NO EL AUTOR DE LA RESEÑA)
            var idUsuarioClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var nombreUsuario = User.Identity?.Name ?? "usuario";

            // Si no se encuentra el id del usuario en los claims, retornar Unauthorized
            if (string.IsNullOrWhiteSpace(idUsuarioClaim) || !int.TryParse(idUsuarioClaim, out var idUsuarioReporta))
                return Unauthorized();

            await _serviceResennaValoracion.ReportarAsync(
                idResenna,
                idUsuarioReporta,
                nombreUsuario,
                observacion
            );

            return Ok(new { success = true });
        }

        // POST: ResennaValoracionController/ToggleEstado
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleEstado(int id, bool estado)
        {
            await _serviceResennaValoracion.UpdateEstadoAsync(id, estado);
            return Ok(new { success = true, estado });
        }

    }
}

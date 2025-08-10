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

    }
}

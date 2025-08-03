using DoreanSportic.Application.DTOs;
using DoreanSportic.Application.Services.Interfaces;
using DoreanSportic.Infrastructure.Models;
using DoreanSportic.Web.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
//using X.PagedList.Extensions;

namespace DoreanSportic.Web.Controllers
{
    public class PedidoDetalleController : Controller
    {
        private readonly IServicePedidoDetalle _servicePedidoDetalle;

        public PedidoDetalleController(IServicePedidoDetalle servicePedidoDetalle)
        {
            _servicePedidoDetalle = servicePedidoDetalle;

        }

        // GET: PedidoDetalleController
        public async Task<ActionResult> Index()
        {
            var collection = await _servicePedidoDetalle.ListAsync();
            return View(collection);
        }

        // GET: PedidoController
        [HttpGet]
        public async Task<ActionResult> GetDetallesPorPedido(int idPedido)
        {
            // Listar las reseñas asociadas a un producto
            var collection = await _servicePedidoDetalle.GetDetallesPorPedido(idPedido);

            return PartialView("_DetallesPedido", collection);
        }

        // GET: PedidoDetalle/Details/{id}
        public async Task<ActionResult> Details(int id)
        {
            var @object = await _servicePedidoDetalle.FindByIdAsync(id);
            return View(@object);
        }

    }
}

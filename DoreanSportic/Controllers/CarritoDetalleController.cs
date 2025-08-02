using DoreanSportic.Application.DTOs;
using DoreanSportic.Application.Services.Interfaces;
using DoreanSportic.Infrastructure.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
//using X.PagedList.Extensions;

namespace DoreanSportic.Web.Controllers
{
    public class CarritoDetalleController : Controller
    {
        private readonly IServiceCarritoDetalle _serviceCarritoDetalle;

        public CarritoDetalleController(IServiceCarritoDetalle serviceCarritoDetalle)
        {
            _serviceCarritoDetalle = serviceCarritoDetalle;

        }

        // GET: CarritoDetalleController
        public async Task<ActionResult> Index()
        {
            var collection = await _serviceCarritoDetalle.ListAsync();
            return View(collection);
        }

        // GET: PedidoDetalle/Details/{id}
        public async Task<ActionResult> Details(int id)
        {
            var @object = await _serviceCarritoDetalle.FindByIdAsync(id);
            return View(@object);
        }
    }
}

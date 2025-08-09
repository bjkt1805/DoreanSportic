using DoreanSportic.Application.Services.Implementations;
using DoreanSportic.Application.Services.Interfaces;
using DoreanSportic.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
namespace DoreanSportic.Web.Controllers
{
    public class CarritoController : Controller
    {
        private readonly IServiceCarrito _serviceCarrito;
        private readonly IServiceCarritoDetalle _serviceCarritoDetalle;
        public CarritoController (IServiceCarrito serviceCarrito, IServiceCarritoDetalle serviceCarritoDetalle)
        {
            _serviceCarrito = serviceCarrito;
            _serviceCarritoDetalle = serviceCarritoDetalle;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var collection = await _serviceCarrito.ListAsync();
            return View(collection);
        }

        // GET: Carrito/Details/5
        public async Task<ActionResult> Details(int id)
        {
            var carrito = await _serviceCarrito.FindByIdAsync(id);

            // Crear un view model para traerse tanto el carrito como sus detalles

            return View(carrito);
        }
    }
}

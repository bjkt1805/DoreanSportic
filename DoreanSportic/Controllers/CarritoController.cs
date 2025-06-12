using DoreanSportic.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
namespace DoreanSportic.Web.Controllers
{
    public class CarritoController : Controller
    {
        private readonly IServiceCarrito _serviceCarrito;
        public CarritoController (IServiceCarrito serviceCarrito)
        {
            _serviceCarrito = serviceCarrito;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var collection = await _serviceCarrito.ListAsync();
            return View(collection);
        }
    }
}

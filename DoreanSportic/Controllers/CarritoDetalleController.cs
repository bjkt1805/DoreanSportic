using DoreanSportic.Application.DTOs;
using DoreanSportic.Application.Services.Implementations;
using DoreanSportic.Application.Services.Interfaces;
using DoreanSportic.Infrastructure.Models;
using DoreanSportic.Web.ViewModel;
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

        // GET: CarritoDetalleController/Details/{id}
        public async Task<ActionResult> Details(int id)
        {
            var @object = await _serviceCarritoDetalle.FindByIdAsync(id);
            return View(@object);
        }

        // POST: CarritoDetalleController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CarritoDetalleDTO dto, IFormFile? foto)
        {

            // Validar que haya al menos una imagen
            if (foto == null)
            {
                ModelState.AddModelError("Foto", "Debe insertar al menos una imagen.");
            }

            // Validar el estado del modelo 
            if (!ModelState.IsValid)
            {
                // Enviar respuesta por JSON para la vista parcial
                return Json(new { success = false, errores = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
            }

            // Si el estado del modelo es correcto, proceder con la creación
            // del producto-
            if (ModelState.IsValid)
            {
                // Procesar la imagen como byte[]
                using (var ms = new MemoryStream())
                {
                    await foto.CopyToAsync(ms);
                    dto.Foto = ms.ToArray();
                }

                // Guardar el detalle del carrito usando service Carrito Detalle
                await _serviceCarritoDetalle.AddAsync(dto);

                // Retornar por medio de JSON la respuesta del modelo/servidor
                return Json(new { success = true, mensaje = "¡Producto agregado al carrito!"});

            }
            // Retornar por medio de JSON la respuesta del modelo/servidor
            return Json(new { success = true, mensaje = "¡Producto agregado al carrito!"});
        }
    }
}

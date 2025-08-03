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
        private readonly IServiceCarrito _serviceCarrito

        public CarritoDetalleController(IServiceCarritoDetalle serviceCarritoDetalle, IServiceCarrito serviceCarrito)
        {
            _serviceCarritoDetalle = serviceCarritoDetalle;
            _serviceCarrito = serviceCarrito;

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
            // Antes de crear la entrada de detalle carrito, hay que revisar si existe
            // carrito en la sesión del navegador

            // Intentar obtener el id del carrito en sesión (puede ser null, por eso "int?")
            int? idCarrito = HttpContext.Session.GetInt32("IdCarrito");

            // Si no existe el carrito, crear uno
            if (idCarrito == null)
            {
                // variable para crear un nuevo objeto carrito
                var nuevoCarrito = new Carrito
                {
                    // Si no hay cliente registrado/con sesión inicada (visita anónima) 
                    // no asignar un IdCliente al carrito todavía
                    FechaCreacion = DateTime.Now,
                    EstadoPago = "Pendiente",
                    Estado = true
                };

                // Como carrito no existe, hay que enviar el objeto Carrito al servicio de Carrito 
                // para agregarlo a la base de datos 
                idCarrito = await _serviceCarrito.CrearCarritoYObtenerIdAsync(nuevoCarrito);
            }

            // Validar el estado del modelo 
            if (!ModelState.IsValid)
            {
                // Enviar respuesta por JSON para la vista parcial
                return Json(new { success = false, errores = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
            }

            // Si el estado del modelo es correcto, proceder con la creación
            // del detalle del carrito-
            if (ModelState.IsValid)
            {
                // Como la foto es nullable entonces solo 
                // se agrega cuando se agrega en el frontend
                if (foto != null)
                {
                    // Procesar la imagen como byte[]
                    using (var ms = new MemoryStream())
                    {
                        await foto.CopyToAsync(ms);
                        dto.Foto = ms.ToArray();
                    }
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

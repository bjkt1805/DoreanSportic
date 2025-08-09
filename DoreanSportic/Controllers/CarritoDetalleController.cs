using DoreanSportic.Application.DTOs;
using DoreanSportic.Application.Services.Implementations;
using DoreanSportic.Application.Services.Interfaces;
using DoreanSportic.Infrastructure.Models;
using DoreanSportic.Web.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Transactions;
//using X.PagedList.Extensions;

namespace DoreanSportic.Web.Controllers
{
    public class CarritoDetalleController : Controller
    {
        private readonly IServiceCarritoDetalle _serviceCarritoDetalle;
        private readonly IServiceCarrito _serviceCarrito;

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
        // Como carritoDetalle está ligado a un view model, las propiedades se acceden como 
        // DetalleCarrito.[atributo] entonces hay que utilizar la propiedad [Bind(Prefix = "DetalleCarrito")] 
        // en el parámetro del método
        public async Task<IActionResult> Create([Bind(Prefix = "DetalleCarrito")] CarritoDetalleDTO dto)
        {

            // Antes de crear la entrada de detalle carrito, hay que revisar si existe
            // carrito en la sesión del navegador

            // Intentar obtener el id del carrito en sesión (puede ser null, por eso "int?")
            int? carritoId = HttpContext.Session.GetInt32("IdCarrito");

            // Si no existe el carrito, crear uno
            if (carritoId == null)
            {
                // variable para crear un nuevo objeto carrito
                var nuevoCarrito = new CarritoDTO
                {
                    // Si no hay cliente registrado/con sesión inicada (visita anónima) 
                    // no asignar un IdCliente al carrito todavía
                    FechaCreacion = DateTime.Now,
                    EstadoPago = "Pendiente",
                    Estado = true
                };

                // Como carrito no existe, hay que enviar el objeto Carrito al servicio de Carrito 
                // para agregarlo a la base de datos 
                carritoId = await _serviceCarrito.AddAsync(nuevoCarrito);

                // Guardar el ID del carrito en la sesión del navegador
                HttpContext.Session.SetInt32("IdCarrito", carritoId.Value);
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
                if (dto.FotoArchivo != null && dto.FotoArchivo.Length > 0)
                {
                    // Procesar la imagen como byte[]
                    using (var ms = new MemoryStream())
                    {
                        await dto.FotoArchivo.CopyToAsync(ms);
                        dto.Foto = ms.ToArray();
                    }
                }

                // Antes de guardar el detalle del carrito en base de datos, asignar el Id del carrito 
                // al objeto carritoDetalle (dto)
                dto.IdCarrito = carritoId.Value;

                // Guardar el detalle del carrito usando service Carrito Detalle
                await _serviceCarritoDetalle.AddAsync(dto);

                // Retornar por medio de JSON la respuesta del modelo/servidor
                return Json(new { success = true, mensaje = "ProductoAgregadoAlCarrito" });

            }
            // Retornar por medio de JSON la respuesta del modelo/servidor
            return Json(new { success = true, mensaje = "ProductoAgregadoAlCarrito" });
        }

        // GET: Método para cargar la vista parcial de los detalles del carrito
        public async Task<IActionResult> NavbarCarrito()
        {
            // Obtener el ID del carrito de la sesión
            int? idCarrito = HttpContext.Session.GetInt32("IdCarrito");

            // Crear una lista vacía de detalles del carrito
            var detalles = new List<CarritoDetalleDTO>();

            // Si el ID del carrito no es nulo, obtener los detalles del carrito
            if (idCarrito.HasValue)
            {
                detalles = await _serviceCarritoDetalle.GetByCarritoIdAsync(idCarrito.Value);
            }
            return PartialView("_CarritoNavbar", detalles);
        }
    }
}

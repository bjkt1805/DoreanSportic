using DoreanSportic.Application.DTOs;
using DoreanSportic.Application.Services.Implementations;
using DoreanSportic.Application.Services.Interfaces;
using DoreanSportic.Infrastructure.Models;
using DoreanSportic.Web.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
//using X.PagedList.Extensions;

namespace DoreanSportic.Web.Controllers
{
    public class PedidoDetalleController : Controller
    {
        private readonly IServicePedidoDetalle _servicePedidoDetalle;
        private readonly IServicePedido _servicePedido;

        public PedidoDetalleController(IServicePedidoDetalle servicePedidoDetalle, IServicePedido servicePedido)
        {
            _servicePedidoDetalle = servicePedidoDetalle;
            _servicePedido = servicePedido;
        }

        // GET: PedidoDetalleController
        public async Task<ActionResult> Index()
        {
            var collection = await _servicePedidoDetalle.ListAsync();
            return View(collection);
        }

        // GET: PedidoDetalle/Details/{id}
        public async Task<ActionResult> Details(int id)
        {
            var @object = await _servicePedidoDetalle.FindByIdAsync(id);
            return View(@object);
        }

        // GET: PedidoController/DetallesPorPedido/{idPedido}
        [HttpGet]
        public async Task<ActionResult> GetDetallesPorPedidoEditable(int idPedido)
        {
            // Listar los detalles asociados a un pedido
            var collection = await _servicePedidoDetalle.GetDetallesPorPedido(idPedido);
            // Pasar el ID del pedido a la vista parcial por medio de ViewData
            ViewData["PedidoId"] = idPedido;
            return PartialView("_DetallesPedidoEditable", collection);
        }

        // GET: PedidoController/DetallesPorPedido/{idPedido}
        [HttpGet]
        public async Task<ActionResult> GetDetallesPorPedido(int idPedido)
        {
            // Listar los detalles asociadados a un pedido
            var collection = await _servicePedidoDetalle.GetDetallesPorPedido(idPedido);
            return PartialView("_DetallesPedido", collection);
        }

        // POST: PedidoDetalleController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        // Como pedidoDetalle está ligado a un view model, las propiedades se acceden como 
        // DetallePedido.[atributo] entonces hay que utilizar la propiedad [Bind(Prefix = "DetallePedido")] 
        // en el parámetro del método
        public async Task<IActionResult> Create([Bind(Prefix = "DetallePedido")] PedidoDetalleDTO dto)
        {

            // Antes de crear la entrada de detalle pedido, hay que revisar si existe
            // pedido en la sesión del navegador

            // Intentar obtener el id del pedido en sesión (puede ser null, por eso "int?")
            int? pedidoId = HttpContext.Session.GetInt32("IdPedido");

            // Si no existe el pedido, crear uno
            if (pedidoId == null)
            {
                // variable para crear un nuevo objeto carrito
                var nuevoPedido = new PedidoDTO
                {
                    // Si no hay cliente registrado/con sesión inicada (visita anónima) 
                    // no asignar un IdCliente al pedido todavía
                    FechaPedido = DateTime.Now,
                    EstadoPago = "Pendiente",
                    Estado = true
                };

                // Como pedido no existe, hay que enviar el objeto Pedido al servicio de Pedido 
                // para agregarlo a la base de datos 
                pedidoId = await _servicePedido.AddAsync(nuevoPedido);

                // Guardar el ID del pedido en la sesión del navegador
                HttpContext.Session.SetInt32("IdPedido", pedidoId.Value);
            }

            // Validar el estado del modelo 
            if (!ModelState.IsValid)
            {
                // Enviar respuesta por JSON para la vista parcial
                return Json(new { success = false, errores = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
            }

            // Si el estado del modelo es correcto, proceder con la creación
            // del detalle del pedido
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

                // Antes de guardar el detalle del pedido en base de datos, asignar el Id del pedido 
                // al objeto pedidoDetalle (dto)
                dto.IdPedido  = pedidoId.Value;

                // Guardar el detalle del pedido usando service del Pedido Detalle
                await _servicePedidoDetalle.AddAsync(dto);

                // Retornar por medio de JSON la respuesta del modelo/servidor
                return Json(new { success = true});

            }
            // Retornar por medio de JSON la respuesta del modelo/servidor
            return Json(new { success = true});
        }

        // GET: Método para cargar la vista parcial de los detalles del carrito
        public async Task<IActionResult> NavbarCarrito()
        {
            // Obtener el ID del pedido de la sesión
            int? idPedido = HttpContext.Session.GetInt32("IdPedido");

            // Crear una lista vacía de detalles del pedido
            var detalles = new List<PedidoDetalleDTO>();

            // Si el ID del carrito no es nulo, obtener los detalles del carrito
            if (idPedido.HasValue)
            {
                detalles = await _servicePedidoDetalle.GetByPedidoIdAsync(idPedido.Value);
            }

            var viewModelCarrito = new CarritoCarritoDetalleViewModel
            {
                PedidoId = idPedido,
                PedidoDetalles = detalles,
            };

            return PartialView("_CarritoNavBar", viewModelCarrito);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActualizarCantidad(int detalleId, int cantidad)
        {
            // Actualizar la cantidad del detalle del pedido y obtener el detalle actualizado y si fue eliminado
            var (detalle, eliminado, pedidoId) = await _servicePedidoDetalle.ActualizarCantidadAsync(detalleId, cantidad);

            // Si el pedidoId es null, retornar error
            if (pedidoId is null)
                return Json(new { success = false, mensaje = "No se encontró el pedido" });

            // Recalcular totales del pedido y asignarlos a las variables sub, imp, total
            var (sub, imp, total) = await _servicePedido.RecalcularTotalesAsync(pedidoId.Value);

            // Detalle viene como un objeto en navegación de arbol (ejemplo: PedidoDetalleDTO tiene una propiedad ProductoDTO Producto)
            // Entonces, para evitar enviar datos innecesarios al cliente, se puede crear un objeto anónimo con solo las propiedades necesarias

            // Se crar una variable auxiliar para enviar un objeto compacto al cliente de detalle
            var detalleCompacto = (object?)null;
            if (detalle != null)
            {
                detalleCompacto = new PedidoDetalleDTO
                {
                    Id = detalle.Id,
                    Cantidad = detalle.Cantidad,
                    SubTotal = detalle.SubTotal,
                };
            }

            // Retornar el detalle actualizado, si fue eliminado y los totales recalculados
            return Json(new 
            { 
                success = true, 
                detalle = detalleCompacto, eliminado,
                totals = new { sub, imp, total } 
            
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarDetalle(int detalleId, int pedidoId)
        {
            await _servicePedidoDetalle.EliminarAsync(detalleId);
            var (sub, imp, total) = await _servicePedido.RecalcularTotalesAsync(pedidoId);
            return Json(new { success = true, totals = new { sub, imp, total } });
        }

    }
}

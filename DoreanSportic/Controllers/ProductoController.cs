using DoreanSportic.Application.DTOs;
using DoreanSportic.Application.Services.Implementations;
using DoreanSportic.Application.Services.Interfaces;
using DoreanSportic.Infrastructure.Data;
using DoreanSportic.Infrastructure.Models;
using DoreanSportic.Web.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System.Linq;
using System.Security.Claims;
using X.PagedList.Extensions;

namespace DoreanSportic.Controllers
{

    public class ProductoController : Controller
    {
        private readonly IServiceProducto _serviceProducto;
        private readonly IServiceMarca _serviceMarca;
        private readonly IServiceCategoria _serviceCategoria;
        private readonly IServiceEtiqueta _serviceEtiqueta;
        private readonly ILogger<ServiceProducto> _logger;
        private readonly IServiceResennaValoracion _serviceResennaValoracion;
        private readonly IServiceUsuario _serviceUsuario;
        private readonly IServiceEmpaque _serviceEmpaque;
        private readonly IServicePedido _servicePedido;
        private readonly DoreanSporticContext _context;

        public ProductoController(IServiceProducto serviceProducto,
            IServiceMarca serviceMarca,
            IServiceCategoria serviceCategoria,
            IServiceEtiqueta serviceEtiqueta,
            ILogger<ServiceProducto> logger,
            IServiceResennaValoracion serviceResennaValoracion,
            IServiceUsuario serviceUsuario,
            IServiceEmpaque serviceEmpaque,
            IServicePedido servicePedido,
            DoreanSporticContext context)
        {
            _serviceProducto = serviceProducto;
            _serviceMarca = serviceMarca;
            _serviceCategoria = serviceCategoria;
            _serviceEtiqueta = serviceEtiqueta;
            _logger = logger;
            _serviceResennaValoracion = serviceResennaValoracion;
            _serviceUsuario = serviceUsuario;
            _serviceEmpaque = serviceEmpaque;
            _servicePedido = servicePedido;
            _context = context;
        }

        // GET: ProductoController
        public async Task<ActionResult> Index()
        {
            var collection = await _serviceProducto.ListAsync();

            // Cargar etiquetas y ponerlas en ViewBag
            var etiquetas = await _context.Etiqueta
            .AsNoTracking()
            .OrderBy(e => e.Nombre)
            .Select(e => new EtiquetaDTO { Id = e.Id, Nombre = e.Nombre })
            .ToListAsync();

            ViewBag.Etiquetas = etiquetas;
            return View(collection);
        }

        //GET: ProductoController para el ADMIN
        public async Task<ActionResult> IndexAdmin()
        {
            var collection = await _serviceProducto.ListAsync();
            return PartialView("_IndexAdmin", collection);
        }

        // GET: ProductoController
        [HttpGet]
        public async Task<ActionResult> FiltrarPorCategoria(int idCategoria)
        {
            // Listar los productos por categoría
            var collection = await _serviceProducto.GetProductoByCategoria(idCategoria);

            return PartialView("_CardsProducto", collection);
        }

        // GET: ProductoControllerAdmin
        [HttpGet]
        public async Task<ActionResult> FiltrarPorCategoriaAdmin(int idCategoria)
        {
            // Listar los productos por categoría
            var collection = await _serviceProducto.GetProductoByCategoriaAdmin(idCategoria);

            return PartialView("_CardsProductoAdmin", collection);
        }


        // GET: ProductoController/Details/{id}
        public async Task<ActionResult> Details(int id)
        {
            var producto = await _serviceProducto.FindByIdAsync(id);
            // PARA EFECTOS DEL AVANCE 4, SE VA A CARGAR EL PRIMER USUARIO
            // DE LA BASE DE DATOS PARA PODER REALIZAR EL EJERCICIO DE 
            // DEJAR UNA RESEÑA DEL PRODUCTO 

            var usuario = await _serviceUsuario.PrimerUsuario();

            // Obtener los empaques disponibles para la personalización del producto
            var empaques = await _serviceEmpaque.ListAsync();

            // Obtener las reseñas del producto
            var resennas = await _serviceResennaValoracion.GetResennasPorProducto(id);

            // Crear un objeto de ViewModel para poder traer el objeto (DTO)
            // de usuario de la base de datos
            var viewModel = new DetalleProductoViewModel
            {
                Producto = producto,
                UsuarioActual = usuario, 
                Resennas = resennas,
                EmpaquesDisponibles = empaques,
            };

            // Verificar si el usuario autenticado (rol cliente) ha comprado el producto
            var haComprado = false;

            // Verificar si ya se ha dejado una reseña para este producto
            var yaResenno = false;

            if (User.Identity.IsAuthenticated && User.FindFirstValue(ClaimTypes.Role) == "2")
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                haComprado = await _servicePedido.UsuarioComproProductoAsync(userId, producto.Id);
                yaResenno = await _serviceResennaValoracion.ExistsByUserProductAsync(userId, producto.Id);
            }
            viewModel.HaComprado = haComprado;
            viewModel.YaResenno = yaResenno;

            return View(viewModel);
        }

        // GET: ProductoController/Dashboard (Producto/Details/{id})
        public async Task<ActionResult> DetailsAdmin(int id)
        {
            var producto = await _serviceProducto.FindByIdAsync(id);

            // PARA EFECTOS DEL AVANCE 4, SE VA A CARGAR EL PRIMER USUARIO
            // DE LA BASE DE DATOS PARA PODER REALIZAR EL EJERCICIO DE 
            // DEJAR UNA RESEÑA DEL PRODUCTO 

            var usuario = await _serviceUsuario.PrimerUsuario();

            // Obtener las reseñas del producto
            var resennas = await _serviceResennaValoracion.GetResennasPorProducto(id);

            var viewModel = new DetalleProductoViewModel
            {
                Producto = producto,
                UsuarioActual = usuario,
                Resennas = resennas,
            };
            return PartialView("_DetailsAdmin", viewModel);

        }

        // GET: ProductoController/Create
        public async Task<IActionResult> Create()
        {
            //var viewModel = new CrearProductoViewModel
            //{
            //    Producto = new ProductoDTO(),
            //    Resennas = _serviceResennaValoracion.GetResennasPorProducto(0)
            //};

            // Viewbag para cargar la lista de marcas desde 
            // el servicio de marcas
            var marcas = await _serviceMarca.ListAsync();
            ViewBag.ListMarcas = new SelectList(marcas, "Id", "Nombre");

            // Viewbag para cargar la lista de categorías desde 
            // el servicio de categorías
            var categorias = await _serviceCategoria.ListAsync();
            ViewBag.ListCategorias = new SelectList(categorias, "Id", "Nombre");

            // Viewbag para cargar la lista de etiquetas desde
            // el servicio de etiquetas
            ViewBag.ListaEtiquetas = await _serviceEtiqueta.ListAsync();

            return PartialView("_CreateProducto");

        }

        // POST: ProductoController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductoDTO dto, List<IFormFile> imagenes, string[] selectedEtiquetas)
        {
            // Validar etiquetas
            if (selectedEtiquetas == null || selectedEtiquetas.Length == 0)
            {
                ModelState.AddModelError("", "Debe asignar al menos una etiqueta.");
            }

            // Validar que haya al menos una imagen
            if (imagenes == null || !imagenes.Any(f => f != null && f.Length > 0))
            {
                ModelState.AddModelError("ImagenesProducto", "Debe insertar al menos una imagen.");
            }

            //if (!ModelState.IsValid)
            //{
            //    // Lee del ModelState todos los errores que
            //    // vienen por el lado del server
            //    string errors = string.Join("; ", ModelState.Values
            //                       .SelectMany(x => x.Errors)
            //                       .Select(x => x.ErrorMessage));
            //    // Response errores
            //    return BadRequest(errors);
            //}

            // Si el estado del modelo es correcto, proceder con la creación
            // del producto-
            if (ModelState.IsValid)
            {
                // Crear la lista de objetos ImagenProducto
                var listaImagenes = new List<ImagenProducto>();

                foreach (var file in imagenes)
                {
                    if (file != null && file.Length > 0)
                    {
                        using var ms = new MemoryStream();
                        await file.CopyToAsync(ms);
                        var imagenBytes = ms.ToArray();

                        listaImagenes.Add(new ImagenProducto
                        {
                            Imagen = imagenBytes,
                            Descripcion = file.FileName,
                            Estado = true
                        });
                    }
                }

                // Asignar imágenes al DTO
                dto.ImagenesProducto = listaImagenes;

                // Asignar etiquetas (para la relación muchos a muchos)
                dto.IdEtiqueta = selectedEtiquetas.Select(id => new Etiqueta { Id = int.Parse(id) }).ToList();

                // Guardar el producto usando service Producto
                await _serviceProducto.AddAsync(dto, selectedEtiquetas);

                //return RedirectToAction("Index", new { mensaje = "Producto creado exitosamente", tipo = "success" });

                // Como _IndexAdmin es una vista parcial, hay que devolver un JSON ya que RedirectToAction no sirve
                // con vistas parciales
                return Json(new { success = true, mensaje = "ProductoAgregado" });


            }
            // Como _IndexAdmin es una vista parcial, hay que devolver un JSON ya que RedirectToAction no sirve
            // con vistas parciales
            return Json(new { success = true, mensaje = "ProductoAgregado" });
        }


        // GET: ProductoController/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var producto = await _serviceProducto.FindByIdAsync(id);

            if (producto == null)
            {
                return NotFound();
            }

            // Viewbag para cargar la lista de marcas desde 
            // el servicio de marcas
            var marcas = await _serviceMarca.ListAsync();
            ViewBag.ListMarcas = new SelectList(marcas, "Id", "Nombre");

            // Viewbag para cargar la lista de categorías desde 
            // el servicio de categorías
            var categorias = await _serviceCategoria.ListAsync();
            ViewBag.ListCategorias = new SelectList(categorias, "Id", "Nombre");

            // Viewbag para cargar la lista de etiquetas desde
            // el servicio de etiquetas
            ViewBag.ListaEtiquetas = await _serviceEtiqueta.ListAsync();

            return PartialView("_EditProducto", producto);
        }

        // POST: ProductoController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductoDTO dto, List<IFormFile> imagenes, string[] selectedEtiquetas, string[] imagenesConservar)
        {
            if (selectedEtiquetas == null || selectedEtiquetas.Length == 0)
            {
                ModelState.AddModelError("", "Debe asignar al menos una etiqueta.");
            }

            //if (!ModelState.IsValid)
            //{
            //    // Volver a cargar listas por si hay errores
            //    ViewBag.ListMarcas = new SelectList(await _serviceMarca.ListAsync(), "Id", "Nombre");
            //    ViewBag.ListCategorias = new SelectList(await _serviceCategoria.ListAsync(), "Id", "Nombre");
            //    ViewBag.ListaEtiquetas = await _serviceEtiqueta.ListAsync();


            //    return PartialView("_EditProducto", dto);
            //}

            var listaImagenes = new List<ImagenProducto>();

            // Procesar nuevas imágenes si las hay
            if (imagenes != null && imagenes.Any())
            {
                foreach (var file in imagenes)
                {
                    if (file != null && file.Length > 0)
                    {
                        using var ms = new MemoryStream();
                        await file.CopyToAsync(ms);
                        var imagenBytes = ms.ToArray();

                        listaImagenes.Add(new ImagenProducto
                        {
                            Imagen = imagenBytes,
                            Descripcion = file.FileName 
                        });
                    }
                }
            }

            // Actualizar DTO
            //dto.ImagenesProducto = listaImagenes;
            dto.IdEtiqueta = selectedEtiquetas.Select(id => new Etiqueta { Id = int.Parse(id) }).ToList();

            // Llamar al servicio para actualizar el producto
            await _serviceProducto.UpdateAsync(id, dto, selectedEtiquetas, listaImagenes);

            return Json(new
            {
                success = true,
                mensaje = "ProductoActualizado"
            });
        }

        // GET: ProductoController/Buscar
        [HttpGet]
        public async Task<IActionResult> Buscar(int? idCategoria, string? q, [FromQuery] List<string>? tagsNombres)
        {
            // Construir la consulta base a enviar a la base de datos por LINQ
            var productos = _context.Producto
                .AsNoTracking()
                .Include(p => p.IdMarcaNavigation)
                .Include(p => p.IdCategoriaNavigation).ThenInclude(c => c.IdPromocion)
                .Include(p => p.IdPromocion)
                .Include(p => p.IdEtiqueta)
                .Include(p => p.ImagenesProducto)
                .Where(p => p.Estado == true)
                .AsQueryable();

            // Si se envía idCategoria, filtrar por esa categoría
            if (idCategoria.HasValue)
                productos = productos.Where(p => p.IdCategoria == idCategoria.Value);

            // Si se envía q (texto de busqueda), filtrar por nombre que contenga q (case insensitive)
            if (!string.IsNullOrWhiteSpace(q))
            {
                var qLower = q.ToLower();
                productos = productos.Where(p => p.Nombre.ToLower().Contains(qLower));
            }

            // Si se envían tagsNombres, filtrar por productos que tengan todas las etiquetas indicadas
            if (tagsNombres != null && tagsNombres.Count > 0)
            {
                // Normalizar: sin nulos/espacios, en minúsculas y sin duplicados
                var tagsLower = tagsNombres
                    .Where(t => !string.IsNullOrWhiteSpace(t))
                    .Select(t => t.Trim().ToLower())
                    .Distinct()
                    .ToList();

                // Filtrar productos que tengan SOLO todas las etiquetas indicadas
                productos = productos.Where(p =>
                    // Para cada tag solicitado, el producto debe tener alguna etiqueta con ese nombre
                    tagsLower.All(t =>
                        p.IdEtiqueta
                         .Select(e => e.Nombre.ToLower())
                         .Contains(t)
                    )
                );
            }

            // Traer la lista final de productos ya filtrada
            var listaDto = await productos
                .OrderBy(p => p.Nombre)
                .Select(p => new DoreanSportic.Application.DTOs.ProductoDTO
                {
                    Id = p.Id,
                    Nombre = p.Nombre,
                    PrecioBase = p.PrecioBase,
                    Stock = p.Stock,
                    PrimeraImagen = p.PrimeraImagen,
                    IdMarcaNavigation = new DoreanSportic.Infrastructure.Models.Marca
                    {
                        Nombre = p.IdMarcaNavigation.Nombre
                    },
                    IdCategoriaNavigation = new DoreanSportic.Infrastructure.Models.Categoria
                    {
                        IdPromocion = p.IdCategoriaNavigation.IdPromocion.ToList()
                    },
                    IdPromocion = p.IdPromocion.ToList(),
                    IdEtiqueta = p.IdEtiqueta.ToList()
                })
                .ToListAsync();

            return PartialView("_CardsProducto", listaDto);
        }

    }
}

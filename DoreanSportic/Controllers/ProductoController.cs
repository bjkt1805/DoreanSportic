using DoreanSportic.Application.DTOs;
using DoreanSportic.Application.Services.Implementations;
using DoreanSportic.Application.Services.Interfaces;
using DoreanSportic.Infrastructure.Models;
using DoreanSportic.Web.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using X.PagedList.Extensions;
//using X.PagedList.Extensions;

namespace DoreanSportic.Controllers
{
    public class ProductoController : Controller
    {
        private readonly IServiceProducto _serviceProducto;
        private readonly IServiceMarca _serviceMarca;
        private readonly IServiceCategoria _serviceCategoria;
        //private readonly IServiceResennaValoracion _serviceResennaValoracion;
        private readonly IServiceEtiqueta _serviceEtiqueta;
        private readonly ILogger<ServiceProducto> _logger;

        public ProductoController(IServiceProducto serviceProducto,
            IServiceMarca serviceMarca,
            IServiceCategoria serviceCategoria,
            //IServiceResennaValoracion serviceResennaValoracion,
            IServiceEtiqueta serviceEtiqueta,
            ILogger<ServiceProducto> logger)
        {
            _serviceProducto = serviceProducto;
            _serviceMarca = serviceMarca;
            _serviceCategoria = serviceCategoria;
            _serviceEtiqueta = serviceEtiqueta;
            _logger = logger;
            _logger = logger;
        }

        // GET: ProductoController
        public async Task<ActionResult> Index()
        {
            var collection = await _serviceProducto.ListAsync();
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
            var @object = await _serviceProducto.FindByIdAsync(id);
            return View(@object);
        }

        // GET: ProductoController/Dashboard (Producto/Details/{id})
        public async Task<ActionResult> DetailsAdmin(int id)
        {
            var @object = await _serviceProducto.FindByIdAsync(id);
            return PartialView("_DetailsAdmin", @object);
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
        public async Task<IActionResult> Create(ProductoDTO dto, List<IFormFile> imagenesProducto, string[] selectedEtiquetas)
        {
            // Validar etiquetas
            if (selectedEtiquetas == null || selectedEtiquetas.Length == 0)
            {
                ModelState.AddModelError("", "Debe asignar al menos una etiqueta.");
            }

            // Validar que haya al menos una imagen
            if (imagenesProducto == null || !imagenesProducto.Any(f => f != null && f.Length > 0))
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

                foreach (var file in imagenesProducto)
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
                            // Podés agregar otras propiedades como NombreArchivo, MimeType, etc.
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
                return Json(new { success = true, mensaje = "Producto creado exitosamente" });

            }

            //// Redirigir o confirmar
            //return RedirectToAction("Index");
            // Como _IndexAdmin es una vista parcial, hay que devolver un JSON ya que RedirectToAction no sirve
            // con vistas parciales
            return Json(new { success = true, mensaje = "Producto creado exitosamente" });
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
        public async Task<IActionResult> Edit(int id, ProductoDTO dto, List<IFormFile> nuevasImagenes, string[] selectedEtiquetas)
        {
            if (selectedEtiquetas == null || selectedEtiquetas.Length == 0)
            {
                ModelState.AddModelError("", "Debe asignar al menos una etiqueta.");
            }

            if (!ModelState.IsValid)
            {
                // Volver a cargar listas por si hay errores
                ViewBag.ListMarcas = new SelectList(await _serviceMarca.ListAsync(), "Id", "Nombre");
                ViewBag.ListCategorias = new SelectList(await _serviceCategoria.ListAsync(), "Id", "Nombre");
                ViewBag.ListaEtiquetas = await _serviceEtiqueta.ListAsync();

                return PartialView("_EditProducto", dto);
            }

            var listaImagenes = new List<ImagenProducto>();

            // Procesar nuevas imágenes si las hay
            if (nuevasImagenes != null && nuevasImagenes.Any())
            {
                foreach (var file in nuevasImagenes)
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
            dto.ImagenesProducto = listaImagenes;
            dto.IdEtiqueta = selectedEtiquetas.Select(id => new Etiqueta { Id = int.Parse(id) }).ToList();

            // Llamar al servicio para actualizar el producto
            await _serviceProducto.UpdateAsync(id, dto, selectedEtiquetas);

            return Json(new
            {
                success = true,
                mensaje = "Producto actualizado correctamente"
            });
        }


        // GET: ProductoController/Delete/5
        //public ActionResult Delete(int id)
        //{
        //    return View();
        //}

        // POST: ProductoController/Delete/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Delete(int id, IFormCollection collection)
        //{
        //    try
        //    {
        //        return RedirectToAction(nameof(Index));
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}
    }
}

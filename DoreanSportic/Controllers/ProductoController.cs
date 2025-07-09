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
            return PartialView("_DetailsAdmin",@object);
        }

        //GET: ProductoController/Create
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
            ViewBag.ListCategorias = new SelectList (categorias, "Id", "Nombre");


            // Viewbag para cargar la lista de etiquetas desde
            // el servicio de etiquetas
            ViewBag.ListaEtiquetas = await _serviceEtiqueta.ListAsync();

            return PartialView("_CreateProducto");

        }

        //POST: Crear producto
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductoDTO dto, List<IFormFile> imagenesProducto, string[] selectedEtiquetas)
        {
            // Validar si hay etiquetas asignadas ANTES del ModelState
            if (selectedEtiquetas == null || selectedEtiquetas.Length == 0)
            {
                ModelState.AddModelError("", "Debe asignar al menos una etiqueta.");
            }

            if (ModelState.IsValid)
            {
                foreach (var file in imagenesProducto)
                {
                    if (file != null && file.Length > 0)
                    {
                        using var ms = new MemoryStream();
                        await file.CopyToAsync(ms);
                        var imagenBytes = ms.ToArray();
                    }
                }
                // Resto de lógica para crear producto...
            }

            return View(dto);
        }


        // GET: ProductoController/Edit/5
        //public ActionResult Edit(int id)
        //{
        //    return View();
        //}

        // POST: ProductoController/Edit/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit(int id, IFormCollection collection)
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

using DoreanSportic.Application.DTOs;
using DoreanSportic.Application.Services.Implementations;
using DoreanSportic.Application.Services.Interfaces;
using DoreanSportic.Infrastructure.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using X.PagedList.Extensions;

namespace DoreanSportic.Controllers
{
    public class PromocionController : Controller
    {
        private readonly IServicePromocion _servicePromocion;
        private readonly IServiceCategoria _serviceCategoria;
        private readonly IServiceProducto _serviceProducto;

        public PromocionController(IServicePromocion servicePromocion, 
            IServiceCategoria serviceCategoria, 
            IServiceProducto serviceProducto)
        {
            _servicePromocion = servicePromocion;
            _serviceCategoria = serviceCategoria;
            _serviceProducto = serviceProducto;
        }

        // GET: PromocionController
        public async Task<ActionResult> Index(int? page)
        {
            var collection = await _servicePromocion.ListAsync();
            return View(collection.ToPagedList(page ?? 1, 5));
        }

        // GET: ResennaValoracionController DASHBOARDADMIN
        public async Task<ActionResult> IndexAdmin(int? page)
        {
            var collection = await _servicePromocion.ListAsync();
            return PartialView("_IndexAdmin", collection.ToPagedList(page ?? 1, 5));
        }

        // GET: PromocionController/Details/5
        public async Task<ActionResult> Details(int id)
        {
            var @object = await _servicePromocion.FindByIdAsync(id);
            return View(@object);
        }

        // GET: PromocionController/DetailsAdmin/{id}
        public async Task<ActionResult> DetailsAdmin(int id)
        {
            var @object = await _servicePromocion.FindByIdAsync(id);
            return PartialView("_DetailsAdmin", @object);
        }

        //GET: PromocionController/Create
        public async Task<IActionResult> Create()
        {
            //// Viewbag para cargar la lista de categorías desde 
            //// el servicio de categorías
            //ViewBag.ListCategorias = new SelectList(await _serviceCategoria.ListAsync(), "Id", "Nombre");

            //// Viewbag para cargar la lista de marcas desde 
            //// el servicio de marcas
            //ViewBag.ListProductos = new SelectList(await _serviceProducto.ListAsync(), "Id", "Nombre");

            // Viewbag para cargar la lista de categorías desde 
            // el servicio de categoría
            var categorias = await _serviceCategoria.ListAsync();
            ViewBag.ListCategorias = new SelectList(categorias, "Id", "Nombre");

            // Viewbag para cargar la lista de productos desde
            // el servicio de producto
            var productos = await _serviceProducto.ListAsync();
            ViewBag.ListProductos = new MultiSelectList(await _serviceProducto.ListAsync(), "Id", "Nombre");

            return PartialView("_CreatePromocion");

        }

        // POST: ProductoController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PromocionDTO dto, int IdCategoriaSeleccionada, List<int> IdProductoSeleccionado)
        {
            if (ModelState.IsValid)
            {
                // Lógica para crear promoción con dto.IdCategoria y dto.IdProducto (lista de int)
                await _servicePromocion.AddAsync(dto, IdCategoriaSeleccionada, IdProductoSeleccionado);

                return RedirectToAction("IndexAdmin");
            }

            // Recargar combos
            ViewBag.ListCategorias = new SelectList(await _serviceCategoria.ListAsync(), "Id", "Nombre");
            ViewBag.ListProductos = new MultiSelectList(await _serviceProducto.ListAsync(), "Id", "Nombre");

            return PartialView("_CreatePromocion", dto);
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

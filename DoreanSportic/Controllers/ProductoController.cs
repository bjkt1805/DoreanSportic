using DoreanSportic.Application.DTOs;
using DoreanSportic.Application.Services.Interfaces;
using DoreanSportic.Infrastructure.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
//using X.PagedList.Extensions;

namespace Libreria.Web.Controllers
{
    public class ProductoController : Controller
    {
        private readonly IServiceProducto _serviceProducto;

        public ProductoController(IServiceProducto serviceProducto)
        {
            _serviceProducto = serviceProducto;

        }

        // GET: ProductoController
        public async Task<ActionResult> Index()
        {
            var collection = await _serviceProducto.ListAsync();
            return View(collection);
        }

        // GET: ProductoController
        [HttpGet]
        public async Task<ActionResult> FiltrarPorCategoria(int idCategoria)
        {
            // Listar los productos por categoría
            var collection = await _serviceProducto.GetProductoByCategoria(idCategoria);

            return PartialView("_CardsProducto", collection);
        }

        // GET: ProductoController para el ADMIN

        //public async Task<ActionResult> IndexAdmin(int? page)
        //{
        //    var collection = await _serviceLibro.ListAsync();
        //    return View(collection.ToPagedList(page ?? 1, 5));
        //}

        // GET: ProductoController/Details/5
        public async Task<ActionResult> Details(int id)
        {
            var @object = await _serviceProducto.FindByIdAsync(id);
            return View(@object);
        }

        // GET: ProductoController/Create
        //public ActionResult Create()
        //{
        //    return View();
        //}

        // POST: ProductoController/Create
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create(IFormCollection collection)
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

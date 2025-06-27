using DoreanSportic.Application.DTOs;
using DoreanSportic.Application.Services.Implementations;
using DoreanSportic.Application.Services.Interfaces;
using DoreanSportic.Infrastructure.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using X.PagedList.Extensions;

namespace DoreanSportic.Controllers
{
    public class PedidoController : Controller
    {
        private readonly IServicePedido _servicePedido;

        public PedidoController(IServicePedido servicePedido)
        {
            _servicePedido = servicePedido;

        }

        // GET: PedidoController
        public async Task<ActionResult> Index(int? page)
        {
            var collection = await _servicePedido.ListAsync();
            return View(collection.ToPagedList(page ?? 1, 5));
        }

        // GET: ProductoController para el ADMIN

        //public async Task<ActionResult> IndexAdmin(int? page)
        //{
        //    var collection = await _serviceLibro.ListAsync();
        //    return View(collection.ToPagedList(page ?? 1, 5));
        //}

        // GET: PedidoController/Details/5
        public async Task<ActionResult> Details(int id)
        {
            var @object = await _servicePedido.FindByIdAsync(id);
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

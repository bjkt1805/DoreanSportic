using DoreanSportic.Application.DTOs;
using DoreanSportic.Application.Services.Interfaces;
using DoreanSportic.Infrastructure.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using X.PagedList.Extensions;

namespace DoreanSportic.Web.Controllers
{
    public class ResennaValoracionController : Controller
    {
        private readonly IServiceResennaValoracion _serviceResennaValoracion;

        public ResennaValoracionController(IServiceResennaValoracion serviceProducto)
        {
            _serviceResennaValoracion = serviceProducto;

        }

        // GET: ResennaValoracionController
        public async Task<ActionResult> Index(int? page)
        {
            var collection = await _serviceResennaValoracion.ListAsync();
            return View(collection.ToPagedList(page ?? 1, 5));
        }

        // GET: ResennaValoracionController DASHBOARDADMIN
        public async Task<ActionResult> IndexAdmin(int? page)
        {
            var collection = await _serviceResennaValoracion.ListAsync();
            return PartialView("_IndexAdmin", collection.ToPagedList(page ?? 1, 5));
        }

        // GET: ResennaValoracionController 
        [HttpGet]
        public async Task<ActionResult> GetResennasPorProducto(int idProducto)
        {
            // Listar las reseñas asociadas a un producto
            var collection = await _serviceResennaValoracion.GetResennasPorProducto(idProducto);

            return PartialView("_ResennasProducto", collection);
        }

        // GET: ResennaValoracionController ADMIN
        [HttpGet]
        public async Task<ActionResult> GetResennasPorProductoAdmin(int idProducto)
        {
            // Listar las reseñas asociadas a un producto
            var collection = await _serviceResennaValoracion.GetResennasPorProducto(idProducto);

            return PartialView("_ResennasProductoVistaEditProducto", collection);
        }
        public async Task<ActionResult> Details(int id)
        {
            var @object = await _serviceResennaValoracion.FindByIdAsync(id);
            return View(@object);
        }

        // GET: ResennaValoracionController/DetailsAdmin/{id}
        public async Task<ActionResult> DetailsAdmin(int id)
        {
            var @object = await _serviceResennaValoracion.FindByIdAsync(id);
            return PartialView("_DetailsAdmin", @object);
        }
    }
}

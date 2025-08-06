using DoreanSportic.Application.DTOs;
using DoreanSportic.Application.Services.Implementations;
using DoreanSportic.Application.Services.Interfaces;
using DoreanSportic.Infrastructure.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
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
        public async Task<IActionResult> Create(PromocionDTO dto,List<int> IdProductoSeleccionado)
        {
            if (ModelState.IsValid)
            {
                // Para poder salvar categoría con promoción en la relación muchos a muchos en la base 
                // de datos se construye la categoría seleccionada como Categoria (en la lista)
                if (dto.IdCategoriaSeleccionada.HasValue)
                {
                    // Solo almacenar el ID como auxiliar, no instancias completas de Categoria
                    // EF lo interpretará correctamente si se configura como many-to-many y se usa con Attach
                    dto.IdCategoria = new List<Categoria>
                    {
                        new Categoria { Id = dto.IdCategoriaSeleccionada.Value }
                    };
                }

                // Lógica para crear promoción con dto.IdCategoria y dto.IdProducto (lista de int)
                await _servicePromocion.AddAsync(dto, IdProductoSeleccionado);

                // Como _IndexAdmin es una vista parcial, hay que devolver un JSON ya que RedirectToAction no sirve
                // con vistas parciales
                return Json(new { success = true, mensaje = "PromocionCreada" });
            }

            // Recargar combos si hay error
            ViewBag.ListCategorias = new SelectList(await _serviceCategoria.ListAsync(), "Id", "Nombre");
            ViewBag.ListProductos = new MultiSelectList(await _serviceProducto.ListAsync(), "Id", "Nombre");

            // Como _IndexAdmin es una vista parcial, hay que devolver un JSON ya que RedirectToAction no sirve
            // con vistas parciales
            return Json(new { success = true, mensaje = "PromocionCreada" });
        }

        // GET: PromocionController/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var promocion = await _servicePromocion.FindByIdAsync(id);

            if (promocion == null)
                return NotFound();

            // Validar que la fecha de hoy sea igual o menor a la fecha de 
            // fin de la promoción para habilitar la edición
            var hoy = DateTime.Today;
            bool esEditable = hoy <= promocion.FechaFin;

            // Viewbag que se enviará a la vista para determinar si la promoción es editable
            ViewBag.EsEditable = esEditable;

            // Obtener categorías y productos
            var categorias = await _serviceCategoria.ListAsync();
            var productos = await _serviceProducto.ListAsync();

            ViewBag.ListCategorias = new SelectList(categorias, "Id", "Nombre");
            ViewBag.ListProductos = productos;

            return PartialView("_EditPromocion", promocion);
        }

        // POST: PromocionController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(PromocionDTO dto)
        {
            if (!ModelState.IsValid)
            {
                // Cargar ViewBags por si se reenvía a la vista
                var categorias = await _serviceCategoria.ListAsync();
                var productos = await _serviceProducto.ListAsync();
                ViewBag.ListCategorias = new SelectList(categorias, "Id", "Nombre");
                ViewBag.ListProductos = productos.Select(p =>
                    new SelectListItem { Value = p.Id.ToString(), Text = p.Nombre }).ToList();

                return PartialView("_EditPromocion", dto);
            }

            try
            {
                await _servicePromocion.UpdateAsync(dto);
                return Json(new { success = true, mensaje = "PromocionActualizada" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, mensaje = "Error" });
            }
        }


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

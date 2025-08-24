using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using DoreanSportic.Application.Services.Interfaces;

namespace DoreanSportic.Web.Controllers
{
    public class ReporteController : Controller
    {
        private readonly IServiceReporte _serviceReporte;

        public ReporteController(IServiceReporte serviceReporte)
        {
            _serviceReporte = serviceReporte;
        }

        // Vista Parcial que contiene todos los canvas/tarjetas
        [HttpGet]
        public IActionResult Estadisticas()
        {
            return PartialView("_Estadisticas");
        }

        // Ventas por día (cantidad de pedidos por día)
        // /Dashboard/VentasPorDia?from=2025-01-01&to=2025-12-31
        [HttpGet]
        public async Task<IActionResult> VentasPorDia(DateTime? from, DateTime? to)
        {
            // últimas 2 semanas por defecto 
            var _from = from ?? DateTime.Today.AddDays(-14); 
            var _to = to ?? DateTime.Today;

            var result = await _serviceReporte.VentasPorDiaAsync(_from, _to);
            return Json(result);
        }

        // Ventas por mes (cantidad de pedidos por mes)
        // /Dashboard/VentasPorMes?year=2025
        [HttpGet]
        public async Task<IActionResult> VentasPorMes(int? year)
        {
            var y = year ?? DateTime.Today.Year;
            var result = await _serviceReporte.VentasPorMesAsync(y);
            return Json(result); // { labels:[], data:[] }
        }

        // 3) Pedidos por estado
        [HttpGet]
        public async Task<IActionResult> PedidosPorEstado()
        {
            var result = await _serviceReporte.PedidosPorEstadoAsync();
            return Json(result); 
        }

        // Top 3 de productos por número de ventas (cantidad de ítems vendidos)
        // /Dashboard/TopProductos?n=3
        [HttpGet]
        public async Task<IActionResult> TopProductos(int n = 3)
        {
            var result = await _serviceReporte.TopProductosAsync(n);
            return Json(result); // { labels:[], data:[] }
        }

        // Últimas 3 reseñas
        [HttpGet]
        public async Task<IActionResult> ResennasRecientes(int n = 3)
        {
            var result = await _serviceReporte.ResennasRecientesAsync(n);
            // lista de { producto, usuario, calificacion, comentario, fecha }
            return Json(result); 
        }
    }
}

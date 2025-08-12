// DoreanSportic.Web/Controllers/ClienteController.cs
using DoreanSportic.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoreanSportic.Web.Controllers
{
    [Authorize]
    public class ClienteController : Controller
    {
        private readonly IServiceCliente _serviceCliente;
        public ClienteController(IServiceCliente serviceCliente)
        {
            _serviceCliente = serviceCliente;
        }

        // GET /Cliente/GetByUserId?userId=xxxxx
        [HttpGet]
        public async Task<IActionResult> GetByUserId(int userId)
        {
            var cliente = await _serviceCliente.GetByUserIdAsync(userId);
            if (cliente is null)
                return NotFound(new { message = "Cliente no encontrado" });

            // Devuelve solo lo necesario para el resumen
            return Json(new
            {
                nombre = cliente.Nombre,
                apellido = cliente.Apellido,
                email = cliente.Email,
                telefono = cliente.Telefono
            });
        }
    }
}

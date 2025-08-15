using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoreanSportic.Web.Controllers
{
    // Controlador para el Dashboard, accesible solo por usuarios con rol 1
    [Authorize(Roles = "1")] // Solo usuarios con rol 1
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View("DashboardAdmin");
        }
    }
}

using Microsoft.AspNetCore.Mvc;

namespace DoreanSportic.Web.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View("DashboardAdmin");
        }
    }
}

using DoreanSportic.Application.Services.Interfaces;
using DoreanSportic.Infrastructure.Models;
using DoreanSportic.Models;
using DoreanSportic.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
// importar librería de localización
using Microsoft.Extensions.Localization;

namespace DoreanSportic.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IServiceUsuario _serviceUsuario;
        private readonly IStringLocalizer<HomeController> _localizer;

        public HomeController(ILogger<HomeController> logger, IServiceUsuario serviceUsuario, IStringLocalizer<HomeController> localizer)
        {
            _logger = logger;
            _serviceUsuario = serviceUsuario;
            _localizer = localizer;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

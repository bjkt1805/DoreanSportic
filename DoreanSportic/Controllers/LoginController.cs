using DoreanSportic.Application.Services.Interfaces;
using DoreanSportic.Infrastructure.Models;
using DoreanSportic.Web.Utils;
using DoreanSportic.Web.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using System.Security.Claims;
using System.Security.Principal;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Libreria.Web.Controllers
{
    public class LoginController : Controller
    {

        private readonly IServiceUsuario _serviceUsuario;
        private readonly ILogger<LoginController> _logger;
        public LoginController(IServiceUsuario serviceUsuario, ILogger<LoginController> logger)
        {
            _serviceUsuario = serviceUsuario;
            _logger = logger;
        }

        // GET: LoginController
        // Método para mostrar la vista de login (si el usuario no está autenticado)
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            // Retornar la vista con el ViewModelLogin vacío
            return View(new LoginViewModel());
        }

        // POST: LoginController/LogIn
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogIn(LoginViewModel viewModel, string? returnUrl = null)
        {
            // Si el viewModel no es válido, retornar la vista con los errores
            if (!ModelState.IsValid) return View(viewModel);

            // Autenticar usuario (el servicio LoginAsync debe verificar hash de contraseña)
            var resultado = await _serviceUsuario.LoginAsync(viewModel.UserName, viewModel.Password);

            if (resultado == null)
            {
                // Si el resultado es nulo, significa que las credenciales son incorrectas
                ModelState.AddModelError(string.Empty, "Usuario o contraseña incorrectos.");
                return View(viewModel);
            }

            var usuario = resultado; // ASignar el valor retornado por el servicio a la variable 'usuario'
            var cliente = usuario.IdClienteNavigation; // Acceder al cliente asociado al usuario

            // Claims

            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()), // Asignar el ID del usuario como NameIdentifier
                new Claim(ClaimTypes.Name, $"{cliente.Nombre} {cliente.Apellido}"), // Asignar el nombre completo del cliente
                new Claim("ClienteId", cliente.Id.ToString()), // Asignar el ID del cliente como Claim personalizado
                new Claim(ClaimTypes.Role, usuario.IdRol.ToString()) // Asignar el rol del usuario
            };

            // Crear el objeto ClaimsIdentity con los claims
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(
                           CookieAuthenticationDefaults.AuthenticationScheme,
                           new ClaimsPrincipal(identity),
                           new AuthenticationProperties { IsPersistent = true, AllowRefresh = true });
            )
            // Guardar IdCliente en sesión para acceso rápido en otras partes de la aplicación
            HttpContext.Session.SetInt32("IdCliente", cliente.Id);

            // Registrar la información de inicio de sesión
            _logger.LogInformation("Login OK para usuario {User}", viewModel.UserName);

            // Redirigir al usuario a la URL de retorno o a la página principal
            return Redirect(returnUrl ?? "/");
        }

        // LogOff: Método para cerrar sesión del usuario
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            _logger.LogInformation("Logout de {User}", User.Identity?.Name);
            await HttpContext.SignOutAsync();
            return RedirectToAction(nameof(Login));
        }
        public IActionResult Forbidden()
        {
            return View();
        }
    }
}
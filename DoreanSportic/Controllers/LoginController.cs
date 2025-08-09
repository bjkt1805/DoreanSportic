using DoreanSportic.Application.DTOs;
using DoreanSportic.Application.Services.Implementations;
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
        private readonly IServiceCliente _serviceCliente;
        private readonly ILogger<LoginController> _logger;
        public LoginController(IServiceUsuario serviceUsuario, 
            IServiceCliente serviceCliente, 
            ILogger<LoginController> logger)
        {
            _serviceUsuario = serviceUsuario;
            _serviceCliente = serviceCliente;
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
                           new ClaimsPrincipal(claimsIdentity),
                           new AuthenticationProperties { IsPersistent = true, AllowRefresh = true });
            
            // Guardar IdCliente en sesión para acceso rápido en otras partes de la aplicación
            HttpContext.Session.SetInt32("IdCliente", cliente.Id);

            // Registrar la información de inicio de sesión
            _logger.LogInformation("Login OK para usuario {User}", viewModel.UserName);

            // Redirigir al usuario a la URL de retorno o a la página principal
            return Redirect(returnUrl ?? "/");
        }

        // Devolver la vista de Registro del usuario
        [HttpGet]
        public IActionResult Register() => View(new RegistroViewModel());

        // POST: LoginController/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegistroViewModel viewModel)
        {
            // Si el viewModel no es válido, retornar la vista con los errores
            if (!ModelState.IsValid) return View(viewModel);

            // Validar si el usuario ya existe
            if (await _serviceUsuario.ExisteUserNameAsync(viewModel.UserName))
            {
                // Si el usuario ya existe, agregar error al modelo y retornar la vista
                ModelState.AddModelError(nameof(viewModel.UserName), "El usuario ya existe.");
                return View(viewModel);
            }

            // Crear Cliente
            var clienteId = await _serviceCliente.CrearClienteAsync(new ClienteDTO
            {
                Nombre = viewModel.Nombre,
                Apellido = viewModel.Apellido,
                Email = viewModel.Email,
                Telefono = viewModel.Telefono,
                Estado = true
            });

            // crear Usuario (junto con Hash de Contraseña)
            var usuarioId = await _serviceUsuario.CrearUsuarioAsync(new UsuarioDTO
            {
                IdCliente = clienteId,
                UserName = viewModel.UserName,
                // Crear el hash de la contraseña utilizando PasswordHasher
                PasswordHash = PasswordHasher.Hash(viewModel.Password),
                FechaRegistro = DateTime.Now,
                EsActivo = true,
                Estado = true,
                IdRol = 2 // “Cliente”
            });

            // Si el usuario se creó correctamente, redirigir a la página de inicio de sesión
            TempData["Msg"] = "Registro exitoso. Inicie sesión.";
            return RedirectToAction(nameof(Login));
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
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
using Microsoft.AspNetCore.Mvc.Rendering;
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
        private readonly IServiceSexo _serviceSexo;
        private readonly ILogger<LoginController> _logger;
        public LoginController(IServiceUsuario serviceUsuario,
            IServiceCliente serviceCliente,
            IServiceSexo serviceSexo,
            ILogger<LoginController> logger)
        {
            _serviceUsuario = serviceUsuario;
            _serviceCliente = serviceCliente;
            _serviceSexo = serviceSexo;
            _logger = logger;
        }

        // Obtener la lista de sexos (para el registro de usuario)
        private async Task<IEnumerable<SelectListItem>> ObtenerSexosAsync()
        {
            // Obtener la lista de sexos desde el servicio
            var sexos = await _serviceSexo.ListAsync();
            // Mapearlos a SelectListItem para el combo
            return sexos.Select(s => new SelectListItem
            {
                Value = s.Id.ToString(),
                Text = s.Nombre
            });
        }

        // GET: LoginController
        // Método para mostrar la vista de login (si el usuario no está autenticado)
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            // Si el usuario ya está autenticado, redirigir a la página principal
            ViewBag.ReturnUrl = returnUrl;
            // Retornar la vista con el ViewModelLogin vacío
            return View(new LoginViewModel());
        }

        // POST: LoginController/LogIn
        // Método para procesar el login del usuario
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

        // GET: LoginController/Register
        // Devolver la vista de Registro del usuario
        [HttpGet]
        public async Task<IActionResult> Registrar()
        {
            var viewModel = new RegistroViewModel
            {
                Sexos = await ObtenerSexosAsync()// Obtener la lista de sexos para el combo
            };
            return View(viewModel);
        }

        // POST: LoginController/Register
        // Método para procesar el registro del usuario
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registrar(RegistroViewModel viewModel)
        {
            // Si el viewModel no es válido, retornar la vista con los errores
            if (!ModelState.IsValid) {
                
                viewModel.Sexos = await ObtenerSexosAsync(); // Reasignar los sexos al viewModel
                return View(viewModel);
            }

            // Validar si el usuario ya existe
            if (await _serviceUsuario.ExisteUserNameAsync(viewModel.UserName))
            {
                // Si el usuario ya existe, agregar error al modelo y retornar la vista
                ModelState.AddModelError(nameof(viewModel.UserName), "El usuario ya existe.");
                viewModel.Sexos = await ObtenerSexosAsync(); // Reasignar los sexos al viewModel
                return View(viewModel);
            }

            // Validar si el email ya está registrado
            if (await _serviceCliente.ExisteEmailAsync(viewModel.Email))
            {
                // Si el email ya está registrado, agregar error al modelo y retornar la vista
                ModelState.AddModelError(nameof(viewModel.Email), "El correo electrónico ya está registrado.");
                viewModel.Sexos = await ObtenerSexosAsync(); // Reasignar los sexos al viewModel
                return View(viewModel);
            }

            // Crear Cliente
            var clienteId = await _serviceCliente.CrearClienteAsync(new ClienteDTO
            {
                Nombre = viewModel.Nombre,
                Apellido = viewModel.Apellido,
                Email = viewModel.Email,
                Telefono = viewModel.Telefono,
                IdSexo = viewModel.IdSexo,
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

        // GET: LoginController/CambiarContrasenna
        // Mostrar la vista para cambiar la contraseña
        [HttpGet]
        public IActionResult CambiarContrasenna()
        {
            var viewModel = new CambiarContrasennaViewModel();
                return View(viewModel);
        }

        // POST: LoginController/CambiarContrasenna
        // Método para procesar el cambio de contraseña
        [HttpPost]
        // [AllowAnonymous] // Permitir acceso anónimo para que cualquier usuario pueda cambiar su contraseña
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RecuperarContrasenna (CambiarContrasennaViewModel viewModel)
        {
            // Si el modelo no es válido, retornar la vista con los errores
            if (!ModelState.IsValid) return View(viewModel);
            
            // Intentar cambiar la contraseña utilizando el servicio
            var resultado = await _serviceUsuario.RecuperarContrasennaAsync(viewModel.Usuario, viewModel.ContrasennaNueva);

            // Si el resultado es falso, significa que la contraseña actual es incorrecta
            if (!resultado)
            {
                // Si el cambio de contraseña falla, agregar error al modelo y retornar la vista
                ModelState.AddModelError(nameof(viewModel.Usuario), "El usuario no existe o está inactivo.");
                return View(viewModel);
            }

            // Si el cambio de contraseña es exitoso, redirigir al login nuevamente
            TempData["Msg"] = "Contraseña recuperada exitosamente. Vuelve a iniciar sesión.";
            return RedirectToAction("Login", "Login");
        }

    }
}
using DoreanSportic.Application.DTOs;
using DoreanSportic.Application.Services.Implementations;
using DoreanSportic.Application.Services.Interfaces;
using DoreanSportic.Infrastructure.Models;
using DoreanSportic.Web.ViewModels;
using DoreanSportic.Web.Utils;
using Libreria.Web.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using X.PagedList.Extensions;

namespace DoreanSportic.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly IServiceUsuario _serviceUsuario;
        private readonly IServiceCliente _serviceCliente;
        private readonly IServiceRol _serviceRol;
        private readonly IServiceSexo _serviceSexo;
        private readonly ILogger<UsuarioController> _logger;

        public UsuarioController(
            IServiceUsuario serviceUsuario, 
            IServiceCliente serviceCliente, 
            IServiceRol serviceRol, 
            IServiceSexo serviceSexo, 
            ILogger<UsuarioController> logger)
        {
            _serviceUsuario = serviceUsuario;
            _serviceCliente = serviceCliente;
            _serviceRol = serviceRol;
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

        // GET: UsuarioController
        public async Task<ActionResult> Index(int? page)
        {
            var collection = await _serviceUsuario.ListAsync();
            return View(collection.ToPagedList(page ?? 1, 5));
        }

        // GET: UsuarioController DASHBOARDADMIN
        public async Task<ActionResult> IndexAdmin(int? page)
        {
            var collection = await _serviceUsuario.ListAsync();
            return PartialView("_IndexAdmin", collection.ToPagedList(page ?? 1, 5));
        }

        // GET: UsuarioController/Details/5
        public async Task<ActionResult> Details(int id)
        {
            var @object = await _serviceUsuario.FindByIdAsync(id);
            return View(@object);
        }

        // GET: UsuarioController/DetailsAdmin/{id}
        public async Task<ActionResult> DetailsAdmin(int id)
        {
            var @object = await _serviceUsuario.FindByIdAsync(id);
            return PartialView("_DetailsAdmin", @object);
        }

        //GET: UsuarioController/Create
        public async Task<IActionResult> Create()
        {
            var roles = await _serviceRol.ListAsync();
            var viewModel = new RegistroViewModel
            {
                // Obtener la lista de tipos de usuario para el combo
                TiposUsuario = roles
                    .Select(t => new SelectListItem
                    {
                        Value = t.Id.ToString(),
                        Text = t.Nombre
                    })
                    .ToList(),

                // Obtener la lista de sexos para el combo
                Sexos = await ObtenerSexosAsync()
            };
            return PartialView("_CreateUsuario", viewModel);

        }

        // POST: UsuarioController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RegistroViewModel viewModel)
        {
            // Si el viewModel no es válido, retornar la vista con los errores
            if (!ModelState.IsValid)
            {

                var errores = ModelState.Where(kv => kv.Value!.Errors.Any())
                    .ToDictionary(kv => kv.Key, kv => kv.Value!.Errors.Select(e => e.ErrorMessage).ToArray());
                return Json(new { success = false, errors = errores });
            }

            // Validar si el email ya está registrado
            if (await _serviceCliente.ExisteEmailAsync(viewModel.Email))
            {
                // Reasignar los sexos al viewModel
                viewModel.Sexos = await ObtenerSexosAsync();

                // Si el correo ya está registrada, enviar el mensaje de error en formato JSON
                return Json(new
                {
                    success = false,
                    errors = new
                    {
                        Email = "CorreoElectronicoYaRegistrado"
                    }
                });

            }

            // Validar si el usuario ya existe
            if (await _serviceUsuario.ExisteUserNameAsync(viewModel.UserName))
            {
                // Reasignar los sexos al viewModel
                viewModel.Sexos = await ObtenerSexosAsync();

                // Si el usuario ya existe, enviar el mensaje de error en formato JSON
                return Json(new
                {
                    success = false,
                    errors = new
                    {
                        UserName = "UsuarioYaExiste"
                    }
                });

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
                Estado = true,
                IdRol = viewModel.IdTipoUsuario
            });

            // Si el usuario se creó correctamente, redirigir a la página de inicio de sesión
            return Json(new { success = true });
        }

        // GET: UsuarioController/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var usuario = await _serviceUsuario.FindByIdAsync(id);
            if (usuario is null) return NotFound();

            var cliente = await _serviceCliente.FindByIdAsync(usuario.IdCliente);

            var vm = new RegistroViewModel
            {
                Id = usuario.Id,
                IdCliente = usuario.IdCliente,
                Nombre = cliente.Nombre,
                Apellido = cliente.Apellido,
                Email = cliente.Email,
                Telefono = cliente.Telefono,
                IdSexo = cliente.IdSexo ?? 3,
                UserName = usuario.UserName,
                IdTipoUsuario = usuario.IdRol,
                Estado = usuario.Estado,
                Sexos = await ObtenerSexosAsync(),
                TiposUsuario = (await _serviceRol.ListAsync()).Select(r => new SelectListItem { Value = r.Id.ToString(), Text = r.Nombre })
            };

            // Si la cargarás por AJAX:
            return PartialView("_EditUsuario", vm);
        }

        // POST: UsuarioController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(RegistroViewModel vm, [FromForm(Name = "_Estado")] bool? estadoFallback)
        {
            if (!ModelState.IsValid)
            {
                var errores = ModelState.Where(x => x.Value!.Errors.Any())
                    .ToDictionary(k => k.Key, v => v.Value!.Errors.Select(e => e.ErrorMessage).ToArray());
                return Json(new { success = false, errors = errores });
            }

            //// Resolver Estado desde toggle + fallback
            //var estadoFinal = vm.Estado || (estadoFallback.HasValue && estadoFallback.Value);

            // Actualizar Cliente
            await _serviceCliente.ActualizarClienteAsync(new ClienteDTO
            {
                Id = vm.IdCliente,
                Nombre = vm.Nombre,
                Apellido = vm.Apellido,
                Email = vm.Email,
                Telefono = vm.Telefono,
                IdSexo = vm.IdSexo,
                Estado = true
            });

            // Hashear la contraseña si se proporcionó una nueva
            var nuevoPasswordHash = PasswordHasher.Hash(vm.Password);

            // Actualizar Usuario
            await _serviceUsuario.ActualizarUsuarioAsync(new UsuarioDTO
            {
                Id = vm.Id,
                IdCliente = vm.IdCliente,
                UserName = vm.UserName,
                IdRol = vm.IdTipoUsuario,
                Estado = vm.Estado,
                PasswordHash = nuevoPasswordHash
            });

            return Json(new { success = true });
        }
    }
}

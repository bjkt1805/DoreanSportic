using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace DoreanSportic.Web.ViewModels
{
    public class RegistroViewModel
    {
        // Usuario requerido y que no supere los 30 caracteres
        [Required(ErrorMessage = "El usuario es requerido")]
        [StringLength(30, MinimumLength = 8, ErrorMessage = "El usuario debe tener entre {2} y {1} caracteres")]
        [RegularExpression(@"^[A-Za-z0-9._-]+$", ErrorMessage = "Solo letras, números, punto, guion y guion bajo")]
        [Display(Name = "Usuario")]
        public string UserName { get; set; } = "";

        // Contraseña requerida, que no supere los 64, cumpla con validaciones (Regex)
        [Required(ErrorMessage = "La contraseña es requerida")]
        // DataType.Password para ocultar el texto ingresado
        [DataType(DataType.Password)]
        [StringLength(64, MinimumLength = 8, ErrorMessage = "La contraseña debe tener entre {2} y {1} caracteres")]
        // Validación por regex: al menos una mayúscula, una minúscula, un número y un carácter especial
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\w\s]).{8,}$",
        ErrorMessage = "Debe tener al menos una mayúscula, una minúscula, un número y un carácter especial")]
        [Display(Name = "Contraseña")]
        public string Password { get; set; } = "";

        // Confirmar contraseña, que sea igual a la contraseña configurada
        [Required(ErrorMessage = "La confirmación es requerida")]
        // DataType.Password para ocultar el texto ingresado
        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Las contraseñas no coinciden")]
        [Display(Name = "Confirmar contraseña")]
        public string ConfirmPassword { get; set; } = "";

        // DATOS DEL CLIENTE //

        // Nombre del cliente requerido, que no supere los 30 caracteres
        [Required(ErrorMessage = "El nombre es requerido")]
        [StringLength(30, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre {2} y {1} caracteres")]
        // Validación por regex: que incluya solo letras (incluidos los acentos), espacios y apóstrofes
        [RegularExpression(@"^[A-Za-zÀ-ÿ' ]+$", ErrorMessage = "El nombre solo debe contener letras")]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; } = "";

        // Apellido del cliente requerido, que no supere los 30 caracteres
        [Required(ErrorMessage = "El apellido es requerido")]
        [StringLength(30, MinimumLength = 2, ErrorMessage = "El apellido debe tener entre {2} y {1} caracteres")]
        // Validación por regex: que incluya solo letras (incluidos los acentos), espacios y apóstrofes
        [RegularExpression(@"^[A-Za-zÀ-ÿ' ]+$", ErrorMessage = "El apellido solo debe contener letras")]
        [Display(Name = "Apellido")]
        public string Apellido { get; set; } = "";

        // Email del cliente requerido, con validación de formato de email
        [Required(ErrorMessage = "El correo electrónico es requerido")]
        [EmailAddress(ErrorMessage = "Correo electrónico no válido")]
        // Validación por regex: que incluya el TLD (.com, .net, .org, etc.)
        [RegularExpression(
            @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.((com|org|net|es|edu|gov|mil|info|co|io)|[a-zA-Z]{2,6})$",
            ErrorMessage = "El correo debe terminar en un dominio válido (.com, .org, .es, etc.)"
        )]
        // DataType.EmailAddress para validar el formato de email
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Correo electrónico")]
        public string Email { get; set; } = "";

        // Teléfono del cliente opcional, acepta +506xxxxxxxx o números con separadores simples
        // Validación por regex: acepta números con separadores simples o el formato internacional
        [RegularExpression(
        @"^(\+?\d{7,15}|(\+?\d{1,4}[-.\s]?)?(\d{2,4}[-.\s]?){2,4}\d{2,4})$",
        ErrorMessage = "Número de teléfono no válido")]
        [Display(Name = "Teléfono")]
        public string? Telefono { get; set; }

        [Display(Name = "Sexo")]
        public int IdSexo { get; set; }

        // Combo para cargar los sexos 
        public IEnumerable<SelectListItem>? Sexos { get; set; }
    }
}

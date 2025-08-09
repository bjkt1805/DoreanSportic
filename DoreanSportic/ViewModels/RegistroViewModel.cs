using System.ComponentModel.DataAnnotations;

namespace DoreanSportic.Web.ViewModels
{
    public class RegistroViewModel
    {
        // Usuario requerido y que no supere los 30 caracteres
        [Required, StringLength(30)] 
        public string UserName { get; set; } = "";

        // Contraseña requerida, que no supere los 100 caracteres y que coincida con la confirmación
        [Required, StringLength(100)] 
        public string Password { get; set; } = "";
        [Required, Compare(nameof(Password))] 
        public string ConfirmPassword { get; set; } = "";

        // Datos del cliente
        [Required] 
        public string Nombre { get; set; } = "";
        [Required] 
        public string Apellido { get; set; } = "";
        [Required, EmailAddress] 
        public string Email { get; set; } = "";
        public string? Telefono { get; set; }
    }
}

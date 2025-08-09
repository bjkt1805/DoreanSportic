using System.ComponentModel.DataAnnotations;
namespace DoreanSportic.Web.ViewModels
{
    public class LoginViewModel
    {
        [Display(Name = "Usuario")]
        [Required(ErrorMessage = "{0} es requerido")]
        [DataType(DataType.EmailAddress)]
        public string UserName { get; set; } = default!;

        [StringLength(15, MinimumLength = 6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres y no sobrepasar los 15 caracteres")]
        [RegularExpression("^[a-zA-Z0-9]*$", ErrorMessage = "Solo se permiten números y letras")]
        [Required(ErrorMessage = "{0} es requerida")]
        [Display(Name = "Contraseña")]
        public string Password { get; set; } = default!;
    }
}

using System.ComponentModel.DataAnnotations;
namespace DoreanSportic.Web.ViewModels
{
    public class LoginViewModel
    {
        [Display(Name = "Usuario")]
        [Required(ErrorMessage = "{0} es requerido")]
        public string UserName { get; set; } = default!;

        [Display(Name = "Contraseña")]
        [Required(ErrorMessage = "{0} es requerida")]
        public string Password { get; set; } = default!;
    }
}

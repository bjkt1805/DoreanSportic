using System.ComponentModel.DataAnnotations;

namespace DoreanSportic.Web.ViewModels
{
    public class CambiarContrasennaViewModel
    {
        [Required, DataType(DataType.Password)]
        [Display(Name = "Contraseña actual")]
        public string ContrasennaActual { get; set; } = default!;

        [Required, DataType(DataType.Password)]
        [Range(8, 15, ErrorMessage = "Debe contener entre 8 y 15 caracteres")]
        [Display(Name = "Nueva contraseña")]
        // Validación por regex: al menos una mayúscula, una minúscula, un número y un carácter especial
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\w\s]).{8,}$",
        ErrorMessage = "Debe tener mayúscula, minúscula, número y símbolo.")]
        public string ContrasennaNueva { get; set; } = default!;

        [Required, DataType(DataType.Password)]
        [Compare(nameof(ContrasennaNueva), ErrorMessage = "Las contraseñas no coinciden")]
        [Display(Name = "Confirmar contraseña")]
        public string ConfirmarContrasenna { get; set; } = default!;
    }
}

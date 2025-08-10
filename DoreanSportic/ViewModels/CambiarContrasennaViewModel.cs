using System.ComponentModel.DataAnnotations;

namespace DoreanSportic.Web.ViewModels
{
    public class CambiarContrasennaViewModel
    {
        [Required]
        [Display(Name = "Usuario")]
        public string Usuario { get; set; } = default!;

        [Required, DataType(DataType.Password)]
        [StringLength(64, MinimumLength = 8, ErrorMessage = "La contraseña debe tener entre {2} y {1} caracteres")]
        [Display(Name = "Nueva contraseña")]
        // Validación por regex: al menos una mayúscula, una minúscula, un número y un carácter especial
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\w\s]).{8,}$",
        ErrorMessage = "Debe tener al menos una mayúscula, una minúscula, un número y un carácter especial")]
        public string ContrasennaNueva { get; set; } = default!;

        [Required, DataType(DataType.Password)]
        // Validación para confirmar la nueva contraseña (DataAnnotation Compare)
        [Compare(nameof(ContrasennaNueva), ErrorMessage = "Las contraseñas no coinciden")]
        [Display(Name = "Confirmar contraseña")]
        public string ConfirmarContrasenna { get; set; } = default!;
    }
}

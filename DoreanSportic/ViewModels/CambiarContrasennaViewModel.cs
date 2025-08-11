using DoreanSportic.Web.Resources.ViewModels;
using System.ComponentModel.DataAnnotations;

namespace DoreanSportic.Web.ViewModels
{
    public class CambiarContrasennaViewModel
    {
        [Display(Name = "UserName_DisplayName", ResourceType = typeof(RegistroViewModelResources))]
        [Required(ErrorMessageResourceType = typeof(RegistroViewModelResources),
              ErrorMessageResourceName = "UserName_Required")]
        public string Usuario { get; set; } = default!;

        [Display(Name = "Password_DisplayName", ResourceType = typeof(RegistroViewModelResources))]
        [Required(ErrorMessageResourceType = typeof(RegistroViewModelResources),
                  ErrorMessageResourceName = "Password_Required")]
        [DataType(DataType.Password)]
        [StringLength(64, MinimumLength = 8,
            ErrorMessageResourceType = typeof(RegistroViewModelResources),
            ErrorMessageResourceName = "Password_Length")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\w\s]).{8,}$",
            ErrorMessageResourceType = typeof(RegistroViewModelResources),
            ErrorMessageResourceName = "Password_Strength")]
        public string ContrasennaNueva { get; set; } = default!;

        [Display(Name = "ConfirmPassword_DisplayName", ResourceType = typeof(RegistroViewModelResources))]
        [Required(ErrorMessageResourceType = typeof(RegistroViewModelResources),
                  ErrorMessageResourceName = "ConfirmPassword_Required")]
        [DataType(DataType.Password)]
        [Compare(nameof(ContrasennaNueva),
            ErrorMessageResourceType = typeof(RegistroViewModelResources),
            ErrorMessageResourceName = "ConfirmPassword_Compare")]
        public string ConfirmarContrasenna { get; set; } = default!;
    }
}

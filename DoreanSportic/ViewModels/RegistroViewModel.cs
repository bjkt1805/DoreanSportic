using Humanizer.Localisation;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using DoreanSportic.Web.Resources.ViewModels;

namespace DoreanSportic.Web.ViewModels;
public class RegistroViewModel
{
    [Display(Name = "UserName_DisplayName", ResourceType = typeof(RegistroViewModelResources))]
    [Required(ErrorMessageResourceType = typeof(RegistroViewModelResources),
              ErrorMessageResourceName = "UserName_Required")]
    [StringLength(30, MinimumLength = 8,
        ErrorMessageResourceType = typeof(RegistroViewModelResources),
        ErrorMessageResourceName = "UserName_Length")]
    [RegularExpression(@"^[A-Za-z0-9._-]+$",
        ErrorMessageResourceType = typeof(RegistroViewModelResources),
        ErrorMessageResourceName = "UserName_Format")]
    public string UserName { get; set; } = "";

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
    public string Password { get; set; } = "";

    [Display(Name = "ConfirmPassword_DisplayName", ResourceType = typeof(RegistroViewModelResources))]
    [Required(ErrorMessageResourceType = typeof(RegistroViewModelResources),
              ErrorMessageResourceName = "ConfirmPassword_Required")]
    [DataType(DataType.Password)]
    [Compare(nameof(Password),
        ErrorMessageResourceType = typeof(RegistroViewModelResources),
        ErrorMessageResourceName = "ConfirmPassword_Compare")]
    public string ConfirmPassword { get; set; } = "";

    [Display(Name = "Nombre_DisplayName", ResourceType = typeof(RegistroViewModelResources))]
    [Required(ErrorMessageResourceType = typeof(RegistroViewModelResources),
              ErrorMessageResourceName = "Nombre_Required")]
    [StringLength(30, MinimumLength = 2,
        ErrorMessageResourceType = typeof(RegistroViewModelResources),
        ErrorMessageResourceName = "Nombre_Length")]
    [RegularExpression(@"^[A-Za-zÀ-ÿ' ]+$",
        ErrorMessageResourceType = typeof(RegistroViewModelResources),
        ErrorMessageResourceName = "Nombre_Format")]
    public string Nombre { get; set; } = "";

    [Display(Name = "Apellido_DisplayName", ResourceType = typeof(RegistroViewModelResources))]
    [Required(ErrorMessageResourceType = typeof(RegistroViewModelResources),
              ErrorMessageResourceName = "Apellido_Required")]
    [StringLength(30, MinimumLength = 2,
        ErrorMessageResourceType = typeof(RegistroViewModelResources),
        ErrorMessageResourceName = "Apellido_Length")]
    [RegularExpression(@"^[A-Za-zÀ-ÿ' ]+$",
        ErrorMessageResourceType = typeof(RegistroViewModelResources),
        ErrorMessageResourceName = "Apellido_Format")]
    public string Apellido { get; set; } = "";

    [Display(Name = "Email_DisplayName", ResourceType = typeof(RegistroViewModelResources))]
    [Required(ErrorMessageResourceType = typeof(RegistroViewModelResources),
              ErrorMessageResourceName = "Email_Required")]
    [EmailAddress(ErrorMessageResourceType = typeof(RegistroViewModelResources),
                  ErrorMessageResourceName = "Email_Invalid")]
    [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.((com|org|net|es|edu|gov|mil|info|co|io)|[a-zA-Z]{2,6})$",
        ErrorMessageResourceType = typeof(RegistroViewModelResources),
        ErrorMessageResourceName = "Email_Tld")]
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; } = "";

    [Display(Name = "Telefono_DisplayName", ResourceType = typeof(RegistroViewModelResources))]
    [RegularExpression(@"^(\+?\d{7,15}|(\+?\d{1,4}[-.\s]?)?(\d{2,4}[-.\s]?){2,4}\d{2,4})$",
        ErrorMessageResourceType = typeof(RegistroViewModelResources),
        ErrorMessageResourceName = "Telefono_Invalid")]
    public string? Telefono { get; set; }

    [ValidateNever]
    [Display(Name = "Sexo_DisplayName", ResourceType = typeof(RegistroViewModelResources))]
    public int IdSexo { get; set; }

    public IEnumerable<SelectListItem>? Sexos { get; set; }
}


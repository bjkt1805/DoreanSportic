using DoreanSportic.Web.Resources.ViewModels;
using System.ComponentModel.DataAnnotations;
namespace DoreanSportic.Web.ViewModels
{
    public class LoginViewModel
    {
        [Display(Name = "UserName_DisplayName", ResourceType = typeof(LoginViewModelResources))]
        [Required(ErrorMessageResourceType = typeof(RegistroViewModelResources),
              ErrorMessageResourceName = "UserName_Required")]
        public string UserName { get; set; } = default!;

        [Display(Name = "Password_DisplayName", ResourceType = typeof(LoginViewModelResources))]
        [Required(ErrorMessageResourceType = typeof(RegistroViewModelResources),
              ErrorMessageResourceName = "Password_Required")]
        public string Password { get; set; } = default!;
    }
}

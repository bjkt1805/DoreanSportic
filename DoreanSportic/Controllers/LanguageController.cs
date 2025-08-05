using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace DoreanSportic.Controllers
{
    public class LanguageController : Controller
    {
        // Acción para cambiar el idioma de la aplicación
        [HttpPost]
        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            // Verifica si la cultura es válida, si no, usa el idioma por defecto (ej. "en-US")
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );

            // Redirige a la página original
            return LocalRedirect(returnUrl ?? "/");
        }
    }
}

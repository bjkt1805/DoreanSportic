using Microsoft.AspNetCore.Mvc;

namespace DoreanSportic.Web.Utils
{
    public enum ToastTipo
    {
        success,
        info,
        warning,
        error
    }

    public static class DaisyUIToastHelper
    {
        public static void SetMensaje(Controller controller, string mensaje, ToastTipo tipo = ToastTipo.info)
        {
            controller.TempData["MensajeTexto"] = mensaje;
            controller.TempData["MensajeTipo"] = tipo.ToString();
        }
    }
} 

using System.Net;
using Newtonsoft.Json;
using System.Text;

namespace DoreanSportic.Web.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;
        private readonly IWebHostEnvironment _env;

        public ErrorHandlingMiddleware(
            RequestDelegate next,
            ILogger<ErrorHandlingMiddleware> logger,
            IWebHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                // En desarrollo, dejar que la Developer Exception Page maneje todo.
                if (_env.IsDevelopment())
                {
                    // Volver a lanzar la excepción
                    throw;
                }

                await HandleErrorAsync(context, ex);
            }
        }

        private async Task HandleErrorAsync(HttpContext context, Exception ex)
        {
            // Si ya empezó la respuesta, no podemos tocar headers/status/redirect.
            if (context.Response.HasStarted)
            {
                _logger.LogWarning("Response already started; cannot redirect. Path: {Path}", context.Request.Path);
                return;
            }

            // Armar info mínima del error
            var eventId = $"{Random.Shared.Next(1, 5000):000000}-{DateTime.Now:yyMMddHHmmss}";
            var errorVm = new ErrorMiddlewareViewModel
            {
                Path = context.Request.Path,
                IdEvent = eventId,
                ListMessages = ex is AggregateException ae
                    ? ae.InnerExceptions.Select(e => e.Message).ToList()
                    : new List<string> { ex.Message }
            };

            // Log detallado (stack trace en logs, no en la query)
            var sb = new StringBuilder()
                .AppendLine()
                .AppendLine($"EventId      : {eventId}")
                .AppendLine($"Path         : {context.Request.Path}")
                .AppendLine($"ErrorList    : {string.Join(" | ", errorVm.ListMessages)}")
                .AppendLine($"StackTrace   : {ex.StackTrace}");
            _logger.LogError(sb.ToString());

            // Serializa y codifica para URL
            var json = JsonConvert.SerializeObject(errorVm);
            var encoded = WebUtility.UrlEncode(json);

            // Limpia y redirige a tu página de error
            context.Response.Clear(); // muy importante
            // Si prefieres 302, Redirect ya establece 302 Found
            context.Response.Redirect($"/Home/ErrorHandler?messagesJson={encoded}");

            await Task.CompletedTask;
        }
    }
}

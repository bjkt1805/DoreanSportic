using DoreanSportic.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DoreanSportic.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UbicacionController : ControllerBase
    {
        private readonly IServiceUbicacion _serviceUbicacion;
        public UbicacionController(IServiceUbicacion serviceUbicacion) {
            _serviceUbicacion = serviceUbicacion;
        
        }

        [HttpGet("Provincias")]
        public async Task<IActionResult> Provincias()
        {
          return (Ok(await _serviceUbicacion.GetProvinciasAsync()));
        }


        [HttpGet("Cantones")]
        public async Task<IActionResult> Cantones([FromQuery] int provinciaId)
        {
            if (provinciaId <= 0) return BadRequest();
            return Ok(await _serviceUbicacion.GetCantonesAsync(provinciaId));
        }

        [HttpGet("Distritos")]
        public async Task<IActionResult> Distritos([FromQuery] int provinciaId, [FromQuery] int cantonId)
        {
            if (provinciaId <= 0 || cantonId <= 0) return BadRequest();
            return Ok(await _serviceUbicacion.GetDistritosAsync(provinciaId, cantonId));
        }
    }
}

using DoreanSportic.Application.DTOs;

namespace DoreanSportic.Web.ViewModels
{
    public class CarritoCarritoDetalleViewModel
    {
        public int? CarritoId { get; set; }

        public int? ClienteId { get; set; }
        public string? EstadoPago { get; set; }
        public DateTime? FechaCreacion { get; set; }

        public List<CarritoDetalleDTO> CarritoDetalles { get; set; } = new List<CarritoDetalleDTO>();

        public decimal Subtotal { get; set; }


    }
}

using DoreanSportic.Application.DTOs;

namespace DoreanSportic.Web.ViewModels
{
    public class CarritoCarritoDetalleViewModel
    {
        public int? PedidoId { get; set; }

        public int? ClienteId { get; set; }
        public string? EstadoPago { get; set; }
        public DateTime? FechaCreacion { get; set; }

        public List<PedidoDetalleDTO> PedidoDetalles { get; set; } = new List<PedidoDetalleDTO>();

        public decimal Subtotal { get; set; }

    }
}

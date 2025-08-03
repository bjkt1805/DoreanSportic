using DoreanSportic.Application.DTOs;
using DoreanSportic.Infrastructure.Models;

namespace DoreanSportic.Web.ViewModel
{
    public class DetalleProductoViewModel
    {
        public ProductoDTO Producto { get; set; }
        public UsuarioDTO UsuarioActual { get; set; }
        public IEnumerable<ResennaValoracionDTO> Resennas { get; set; }
        public IEnumerable<EmpaqueDTO> EmpaquesDisponibles { get; set; }
        // Para poder crear detalle de Carrito
        public CarritoDetalleDTO DetalleCarrito { get; set; }
    }
}

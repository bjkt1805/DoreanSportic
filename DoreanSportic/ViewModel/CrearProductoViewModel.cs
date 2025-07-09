using DoreanSportic.Application.DTOs;

namespace DoreanSportic.Web.ViewModel
{
    public class CrearProductoViewModel
    {
        public ProductoDTO Producto { get; set; }
        public List<ResennaValoracionDTO> Resennas { get; set; } = new();
    }
}

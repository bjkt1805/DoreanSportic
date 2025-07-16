using DoreanSportic.Application.DTOs;

namespace DoreanSportic.Web.ViewModel
{
    public class DetalleProductoViewModel
    {
        public ProductoDTO Producto { get; set; }
        public UsuarioDTO UsuarioActual { get; set; }
        public IEnumerable<ResennaValoracionDTO> Resennas { get; set; }

    }
}

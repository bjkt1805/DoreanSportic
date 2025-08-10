using AutoMapper;
using DoreanSportic.Application.DTOs;
using DoreanSportic.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoraanSportic.Application.Profiles
{
    public class ProductoProfile : Profile
    {
        //public ProductoProfile()
        //{
        //    CreateMap<ProductoDTO, Producto>().ReverseMap();
        //}

        public ProductoProfile()
        {
            CreateMap<ProductoDTO, Producto>()
                .ForMember(dest => dest.ImagenesProducto, opt => opt.Ignore())
                .ForMember(dest => dest.IdEtiqueta, opt => opt.Ignore())
                .ForMember(dest => dest.IdPromocion, opt => opt.Ignore())
                .ForMember(dest => dest.PedidoDetalle, opt => opt.Ignore());

            CreateMap<ProductoDTO, Producto>().ReverseMap();
        }
    }
}

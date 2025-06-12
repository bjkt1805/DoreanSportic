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
    public class EmpaqueProfile : Profile
    {
        public EmpaqueProfile()
        {
            CreateMap<EmpaqueDTO, Empaque>().ReverseMap();
        }
    }
}

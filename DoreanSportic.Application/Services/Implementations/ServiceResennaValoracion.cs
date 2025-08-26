using AutoMapper;
using DoreanSportic.Application.DTOs;
using DoreanSportic.Application.Services.Interfaces;
using DoreanSportic.Infrastructure.Models;
using DoreanSportic.Infrastructure.Repository.Implementations;
using DoreanSportic.Infrastructure.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace DoreanSportic.Application.Services.Implementations
{
    public class ServiceResennaValoracion : IServiceResennaValoracion
    {
        private readonly IRepositoryResennaValoracion _repository;
        private readonly IMapper _mapper;
        public ServiceResennaValoracion(IRepositoryResennaValoracion repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public async Task<ResennaValoracionDTO> FindByIdAsync(int id)
        {
            var @object = await _repository.FindByIdAsync(id);
            var objectMapped = _mapper.Map<ResennaValoracionDTO>(@object);
            return objectMapped;
        }
        public async Task<ICollection<ResennaValoracionDTO>> ListAsync()
        {
            //Obtener datos del repositorio
            var list = await _repository.ListAsync();
            // Map List<ResennaValoracion> a ICollection<ResennaValoracionDTO>
            var collection = _mapper.Map<ICollection<ResennaValoracionDTO>>(list);
            // Return lista
            return collection;
        }

        public async Task<ICollection<ResennaValoracionDTO>> GetResennasPorProducto(int idProducto)
        {
            //Obtener datos del repositorio
            var list = await _repository.GetResennasPorProducto(idProducto);
            // Map List<ResennaValoracion> a ICollection<ResennaValoracionDTO>
            var collection = _mapper.Map<ICollection<ResennaValoracionDTO>>(list);
            // Return lista
            return collection;
        }

        public async Task<int> AddAsync(ResennaValoracionDTO dto)
        {
            var objectMapped = _mapper.Map<ResennaValoracion>(dto);
            return await _repository.AddAsync(objectMapped);
        }

        public async Task<ResennaValoracionStatsDTO> GetStatsAsync()
        {
            // Llamar al repositorio para obtener las estadísticas
            var (s5, s4, s3, s2, s1, total, avg) = await _repository.GetStatsAsync();

            // Mapear los resultados al DTO
            return new ResennaValoracionStatsDTO
            {
                Star5 = s5,
                Star4 = s4,
                Star3 = s3,
                Star2 = s2,
                Star1 = s1,
                Total = total,
                Average = avg
            };
        }

        public async Task ReportarAsync(int idResenna, int idUsuarioReporta, string nombreUsuarioReporta, string? observacion)
        {
            await _repository.ReportarAsync(idResenna, idUsuarioReporta, nombreUsuarioReporta, observacion);
        }

        public async Task UpdateEstadoAsync(int id, bool estado)
        {
            await _repository.UpdateEstadoAsync(id, estado);
        }
    }
}
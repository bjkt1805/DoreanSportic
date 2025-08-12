using Microsoft.Extensions.Caching.Memory;
using DoreanSportic.Application.Services.Interfaces;
using System.Net.Http.Json;
using static DoreanSportic.Application.DTOs.UbicacionDTO;

public class ServiceUbicacion : IServiceUbicacion
{
    private readonly HttpClient _http;
    private readonly IMemoryCache _cache;
    public ServiceUbicacion(HttpClient http, IMemoryCache cache)
    {
        _http = http; _cache = cache;
    }

    // Método para obtener provincias
    public Task<IReadOnlyList<ProvinciaDTO>> GetProvinciasAsync()
        // Utiliza la caché para almacenar las provincias y evitar llamadas repetidas al servidor
        => _cache.GetOrCreateAsync("prov", async e =>
        {
            // Configurar la expiración de la caché a 24 horas
            e.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24);

            // Obtener provincias desde el archivo JSON
            var dict = await _http.GetFromJsonAsync<Dictionary<string, string>>("provincias.json")
                       ?? new Dictionary<string, string>();

            // Convertir el diccionario a una lista de ProvinciaDTO y ordenarla por Id
            var list = dict
                .Select(kv => new ProvinciaDTO(int.Parse(kv.Key), kv.Value))
                .OrderBy(p => p.Id)
                .ToList();

            // Retornar la lista como una colección de solo lectura
            return (IReadOnlyList<ProvinciaDTO>)list;
        })!;

    // Método para obtener cantones por provincia
    public Task<IReadOnlyList<CantonDTO>> GetCantonesAsync(int provinciaId)
        // Utiliza la caché para almacenar los cantones de una provincia específica y evitar llamadas repetidas al servidor
        => _cache.GetOrCreateAsync($"cant_{provinciaId}", async e =>
        {
            // Configurar la expiración de la caché a 24 horas
            e.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24);

            // Obtener los cantones desde el archivo JSON de la provincia específica
            var path = $"provincia/{provinciaId}/cantones.json";
            var dict = await _http.GetFromJsonAsync<Dictionary<string, string>>(path)
                       ?? new Dictionary<string, string>();

            // Convertir el diccionario a una lista de CantonDTO y ordenarla por Id
            var list = dict
                .Select(kv => new CantonDTO(int.Parse(kv.Key), kv.Value, provinciaId))
                .OrderBy(c => c.Id)
                .ToList();

            // Retor la lista como una colección de solo lectura
            return (IReadOnlyList<CantonDTO>)list;
        })!;

    // Método para obtener distritos por provincia y cantón
    public Task<IReadOnlyList<DistritoDTO>> GetDistritosAsync(int provinciaId, int cantonId)
        // Utiliza la caché para almacenar los distritos de un cantón específico y evitar llamadas repetidas al servidor
        => _cache.GetOrCreateAsync($"dist_{provinciaId}_{cantonId}", async e =>
        {
            // Configurar la expiración de la caché a 24 horas
            e.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24);

            // Obtener los distritos desde el archivo JSON del cantón específico en la provincia
            var path = $"provincia/{provinciaId}/canton/{cantonId}/distritos.json";
            var dict = await _http.GetFromJsonAsync<Dictionary<string, string>>(path)
                       ?? new Dictionary<string, string>();

            // Convertir el diccionario a una lista de DistritoDTO y ordenarla por Id
            var list = dict
                .Select(kv => new DistritoDTO(int.Parse(kv.Key), kv.Value, provinciaId, cantonId))
                .OrderBy(d => d.Id)
                .ToList();

            // Retornar la lista como una colección de solo lectura
            return (IReadOnlyList<DistritoDTO>)list;
        })!;
}

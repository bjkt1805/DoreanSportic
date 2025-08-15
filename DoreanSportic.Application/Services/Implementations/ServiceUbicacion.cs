using Microsoft.Extensions.Caching.Memory;
using DoreanSportic.Application.Services.Interfaces;
using System.Net.Http.Json;
using static DoreanSportic.Application.DTOs.UbicacionDTO;

public class ServiceUbicacion : IServiceUbicacion
{
    // Inyección de dependencias (HttpClient y IMemoryCache)
    private readonly HttpClient _http;
    private readonly IMemoryCache _cache;
    public ServiceUbicacion(HttpClient http, IMemoryCache cache)
    {
        _http = http; _cache = cache;
    }

    // Modelos para deserializar la respuesta de la API
    private sealed record ApiResponse<T>(string status, int statusCode, string message, T data);

    private sealed record ProvinciaApiItem(int idProvincia, string descripcion);
    private sealed record CantonApiItem(int idCanton, int idProvincia, string descripcion);
    private sealed record DistritoApiItem(int idDistrito, int idCanton, string descripcion);

    // Lamentablemente, la API envía Heredia con id 3 y Cartago con id 4 (debe hacerse un swap manual)

    // Provincias: GET /provincias (la API devuelve un campo "status" que no se usa)
    public Task<IReadOnlyList<ProvinciaDTO>> GetProvinciasAsync()

        // Cache por 1 año (los datos no cambian)
        => _cache.GetOrCreateAsync("prov_cr_v3", async e =>
        {
            e.AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(365);

            // Lista de provincias desde la API
            ApiResponse<List<ProvinciaApiItem>>? api;

            // Llamada con manejo de errores (si falla, devolver lista vacía)
            try { api = await _http.GetFromJsonAsync<ApiResponse<List<ProvinciaApiItem>>>("provincias"); }
            catch { api = null; }

            // Crear lista de DTOs de Provincias
            var list = new List<ProvinciaDTO>();

            // Recorrer los datos de la API y mapear a DTOs
            foreach (var p in api?.data ?? new List<ProvinciaApiItem>())
            {
                var name = (p.descripcion ?? "").Trim();
                var id = p.idProvincia;

                // Aquí debe hacerse el swap manual de Heredia y Cartago

                // Si el nombre de la provincia es Heredia y el id es 3, cambiar a 4
                if (id == 3 && name.Equals("Heredia", StringComparison.OrdinalIgnoreCase)) id = 4;

                // Si el nombre de la provincia es Cartago y el id es 4, cambiar a 3
                else if (id == 4 && name.Equals("Cartago", StringComparison.OrdinalIgnoreCase)) id = 3;

                // Agregar a la lista de DTOs
                list.Add(new ProvinciaDTO(id, name));
            }

            // Ordenar la lista por Id y devolver como IReadOnlyList
            list = list.OrderBy(x => x.Id).ToList();
            return (IReadOnlyList<ProvinciaDTO>)list;
        })!;

    // Cantones por provincia: GET /provincias/{idProvincia}/cantones
    public Task<IReadOnlyList<CantonDTO>> GetCantonesAsync(int provinciaId)

        // Cache por 1 año (los datos no cambian)
        => _cache.GetOrCreateAsync($"cant_cr_v2_{provinciaId}", async e =>
        {
            e.AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(365);

            // Lista de provincias desde la API
            ApiResponse<List<CantonApiItem>>? api;

            // Llamada con manejo de errores (si falla, devolver lista vacía)
            try { api = await _http.GetFromJsonAsync<ApiResponse<List<CantonApiItem>>>($"provincias/{provinciaId}/cantones"); }
            catch { api = null; }

            // Mapear a DTOs y ordenar por Id
            var list = (api?.data ?? new List<CantonApiItem>())
                .Select(c => new CantonDTO(c.idCanton, c.descripcion, c.idProvincia)) // usamos el idProvincia que trae la API
                .OrderBy(c => c.Id)
                .ToList();

            // Devolver como IReadOnlyList
            return (IReadOnlyList<CantonDTO>)list;
        })!;

    // Distritos por cantón: GET /cantones/{idCanton}/distritos
    // (Mantenemos la firma con provinciaId para tu DTO; la API no lo devuelve, así que usamos el parámetro)
    public Task<IReadOnlyList<DistritoDTO>> GetDistritosAsync(int provinciaId, int cantonId)

        // Cache por 1 año (los datos no cambian)
        => _cache.GetOrCreateAsync($"dist_cr_v2_{provinciaId}_{cantonId}", async e =>
        {
            e.AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(365);

            // Lista de provincias desde la API
            ApiResponse<List<DistritoApiItem>>? api;

            // Llamada con manejo de errores (si falla, devolver lista vacía)
            try { api = await _http.GetFromJsonAsync<ApiResponse<List<DistritoApiItem>>>($"cantones/{cantonId}/distritos"); }
            catch { api = null; }

            // Mapear a DTOs y ordenar por Id
            var list = (api?.data ?? new List<DistritoApiItem>())
                .Select(d => new DistritoDTO(d.idDistrito, d.descripcion, provinciaId, cantonId))
                .OrderBy(d => d.Id)
                .ToList();

            // Devolver como IReadOnlyList
            return (IReadOnlyList<DistritoDTO>)list;
        })!;
}

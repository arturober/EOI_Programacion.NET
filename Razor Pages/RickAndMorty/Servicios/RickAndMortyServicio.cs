using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using RickAndMorty.Configuracion;
using RickAndMorty.DTOs;

namespace RickAndMorty.Servicios;

// Centraliza las peticiones, la caché y el tratamiento de errores.
public class RickAndMortyServicio : IRickAndMortyServicio
{
    private readonly HttpClient _cliente;
    private readonly IMemoryCache _cache;
    private readonly RickAndMortyOpciones _opciones;

    public RickAndMortyServicio(
        HttpClient cliente,
        IMemoryCache cache,
        IOptions<RickAndMortyOpciones> opciones)
    {
        _cliente = cliente;
        _cache = cache;
        _opciones = opciones.Value;
    }

    public Task<PaginaApiDto<PersonajeDto>> BuscarPersonajesAsync(
        string? nombre,
        string? estado,
        string? especie,
        string? genero,
        int pagina,
        CancellationToken cancellationToken = default)
    {
        string ruta = CrearRuta(
            "character",
            pagina,
            new Dictionary<string, string?>
            {
                ["name"] = Limpiar(nombre),
                ["status"] = Limpiar(estado),
                ["species"] = Limpiar(especie),
                ["gender"] = Limpiar(genero)
            });

        return ObtenerPaginaAsync<PersonajeDto>(
            ruta,
            cancellationToken);
    }

    public Task<PersonajeDto> ObtenerPersonajeAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        ValidarId(id, "personaje");
        return ObtenerConCacheAsync<PersonajeDto>(
            $"character/{id}",
            cancellationToken);
    }

    public Task<PaginaApiDto<EpisodioDto>> BuscarEpisodiosAsync(
        string? nombre,
        string? codigo,
        int pagina,
        CancellationToken cancellationToken = default)
    {
        string ruta = CrearRuta(
            "episode",
            pagina,
            new Dictionary<string, string?>
            {
                ["name"] = Limpiar(nombre),
                ["episode"] = Limpiar(codigo)
            });

        return ObtenerPaginaAsync<EpisodioDto>(
            ruta,
            cancellationToken);
    }

    public Task<EpisodioDto> ObtenerEpisodioAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        ValidarId(id, "episodio");
        return ObtenerConCacheAsync<EpisodioDto>(
            $"episode/{id}",
            cancellationToken);
    }

    public Task<PaginaApiDto<LocalizacionDto>> BuscarLocalizacionesAsync(
        string? nombre,
        string? tipo,
        string? dimension,
        int pagina,
        CancellationToken cancellationToken = default)
    {
        string ruta = CrearRuta(
            "location",
            pagina,
            new Dictionary<string, string?>
            {
                ["name"] = Limpiar(nombre),
                ["type"] = Limpiar(tipo),
                ["dimension"] = Limpiar(dimension)
            });

        return ObtenerPaginaAsync<LocalizacionDto>(
            ruta,
            cancellationToken);
    }

    public Task<LocalizacionDto> ObtenerLocalizacionAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        ValidarId(id, "localización");
        return ObtenerConCacheAsync<LocalizacionDto>(
            $"location/{id}",
            cancellationToken);
    }

    public Task<IReadOnlyList<EpisodioDto>> ObtenerEpisodiosPorUrlsAsync(
        IEnumerable<string> urls,
        int maximo = 60,
        CancellationToken cancellationToken = default)
    {
        return ObtenerVariosAsync<EpisodioDto>(
            "episode",
            urls,
            maximo,
            cancellationToken);
    }

    public Task<IReadOnlyList<PersonajeDto>> ObtenerPersonajesPorUrlsAsync(
        IEnumerable<string> urls,
        int maximo = 40,
        CancellationToken cancellationToken = default)
    {
        return ObtenerVariosAsync<PersonajeDto>(
            "character",
            urls,
            maximo,
            cancellationToken);
    }

    private async Task<PaginaApiDto<T>> ObtenerPaginaAsync<T>(
        string ruta,
        CancellationToken cancellationToken)
    {
        try
        {
            return await ObtenerConCacheAsync<PaginaApiDto<T>>(
                ruta,
                cancellationToken);
        }
        catch (RickAndMortyApiExcepcion excepcion)
            when (excepcion.CodigoEstado == HttpStatusCode.NotFound)
        {
            // La API utiliza 404 para una búsqueda correcta sin resultados.
            return new PaginaApiDto<T>();
        }
    }

    private async Task<IReadOnlyList<T>> ObtenerVariosAsync<T>(
        string recurso,
        IEnumerable<string> urls,
        int maximo,
        CancellationToken cancellationToken)
        where T : class
    {
        List<int> ids = urls
            .Select(TextoRickAndMorty.ExtraerId)
            .Where(id => id.HasValue)
            .Select(id => id!.Value)
            .Distinct()
            .Take(Math.Clamp(maximo, 1, 100))
            .ToList();

        if (ids.Count == 0)
        {
            return Array.Empty<T>();
        }

        if (ids.Count == 1)
        {
            T elemento = await ObtenerConCacheAsync<T>(
                $"{recurso}/{ids[0]}",
                cancellationToken);

            return new List<T> { elemento }.AsReadOnly();
        }

        string ruta = $"{recurso}/{string.Join(",", ids)}";
        List<T> elementos = await ObtenerConCacheAsync<List<T>>(
            ruta,
            cancellationToken);

        return elementos.AsReadOnly();
    }

    private async Task<T> ObtenerConCacheAsync<T>(
        string ruta,
        CancellationToken cancellationToken)
        where T : class
    {
        string clave = $"rick-and-morty:{ruta.ToLowerInvariant()}";

        T? resultado = await _cache.GetOrCreateAsync(
            clave,
            async entrada =>
            {
                entrada.AbsoluteExpirationRelativeToNow =
                    TimeSpan.FromMinutes(
                        Math.Clamp(_opciones.MinutosCache, 5, 240));

                return await ObtenerJsonAsync<T>(
                    ruta,
                    cancellationToken);
            });

        return resultado
            ?? throw new RickAndMortyApiExcepcion(
                "La API ha devuelto una respuesta vacía.");
    }

    private async Task<T> ObtenerJsonAsync<T>(
        string ruta,
        CancellationToken cancellationToken)
    {
        try
        {
            using HttpResponseMessage respuesta =
                await _cliente.GetAsync(ruta, cancellationToken);

            if (!respuesta.IsSuccessStatusCode)
            {
                throw CrearExcepcion(respuesta.StatusCode);
            }

            T? datos = await respuesta.Content.ReadFromJsonAsync<T>(
                cancellationToken: cancellationToken);

            return datos
                ?? throw new RickAndMortyApiExcepcion(
                    "La API ha devuelto una respuesta vacía.");
        }
        catch (OperationCanceledException)
            when (!cancellationToken.IsCancellationRequested)
        {
            throw new RickAndMortyApiExcepcion(
                "La API ha tardado demasiado en responder.");
        }
        catch (HttpRequestException)
        {
            throw new RickAndMortyApiExcepcion(
                "No se ha podido conectar con The Rick and Morty API.");
        }
        catch (JsonException)
        {
            throw new RickAndMortyApiExcepcion(
                "La API ha devuelto datos con un formato inesperado.");
        }
    }

    private static string CrearRuta(
        string recurso,
        int pagina,
        Dictionary<string, string?> filtros)
    {
        filtros["page"] = Math.Max(1, pagina).ToString();

        Dictionary<string, string?> parametros = filtros
            .Where(filtro => !string.IsNullOrWhiteSpace(filtro.Value))
            .ToDictionary(filtro => filtro.Key, filtro => filtro.Value);

        return QueryHelpers.AddQueryString(recurso, parametros);
    }

    private static string? Limpiar(string? texto)
    {
        return string.IsNullOrWhiteSpace(texto)
            ? null
            : texto.Trim();
    }

    private static void ValidarId(int id, string recurso)
    {
        if (id <= 0)
        {
            throw new RickAndMortyApiExcepcion(
                $"El identificador del {recurso} no es válido.",
                HttpStatusCode.BadRequest);
        }
    }

    private static RickAndMortyApiExcepcion CrearExcepcion(
        HttpStatusCode codigo)
    {
        string mensaje = codigo switch
        {
            HttpStatusCode.BadRequest =>
                "La API no ha aceptado los filtros enviados.",
            HttpStatusCode.NotFound =>
                "No se han encontrado datos para esta consulta.",
            HttpStatusCode.TooManyRequests =>
                "Se ha alcanzado el límite de peticiones. Inténtalo más tarde.",
            _ =>
                "The Rick and Morty API no ha podido completar la consulta."
        };

        return new RickAndMortyApiExcepcion(mensaje, codigo);
    }
}

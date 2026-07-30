using System.Globalization;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Videojuegos.Configuracion;
using Videojuegos.DTOs;
using Videojuegos.Modelos;

namespace Videojuegos.Servicios;

// Centraliza las peticiones, la caché y la conversión de datos externos.
public class RawgServicio : IRawgServicio
{
    private readonly HttpClient _cliente;
    private readonly IMemoryCache _cache;
    private readonly RawgOpciones _opciones;

    public RawgServicio(
        HttpClient cliente,
        IMemoryCache cache,
        IOptions<RawgOpciones> opciones)
    {
        _cliente = cliente;
        _cache = cache;
        _opciones = opciones.Value;
    }

    public Task<PaginaVideojuegos> ObtenerListadoAsync(
        TipoListado tipo,
        int pagina = 1,
        CancellationToken cancellationToken = default)
    {
        DateTime hoy = DateTime.Today;

        Dictionary<string, string?> filtros = tipo switch
        {
            TipoListado.MejorValorados => new()
            {
                ["ordering"] = "-metacritic",
                ["metacritic"] = "75,100"
            },
            TipoListado.Novedades => new()
            {
                ["dates"] =
                    $"{hoy.AddMonths(-4):yyyy-MM-dd},{hoy:yyyy-MM-dd}",
                ["ordering"] = "-released"
            },
            TipoListado.Proximamente => new()
            {
                ["dates"] =
                    $"{hoy:yyyy-MM-dd},{hoy.AddYears(1):yyyy-MM-dd}",
                ["ordering"] = "-added"
            },
            TipoListado.Accion => CrearFiltroGenero("action"),
            TipoListado.Rol => CrearFiltroGenero("role-playing-games-rpg"),
            TipoListado.Estrategia => CrearFiltroGenero("strategy"),
            TipoListado.Indie => CrearFiltroGenero("indie"),
            TipoListado.Deportes => CrearFiltroGenero("sports"),
            TipoListado.Carreras => CrearFiltroGenero("racing"),
            _ => new() { ["ordering"] = "-added" }
        };

        return ObtenerPaginaAsync(
            filtros,
            pagina,
            $"listado:{tipo}",
            cancellationToken);
    }

    public Task<PaginaVideojuegos> BuscarAsync(
        string texto,
        int pagina = 1,
        CancellationToken cancellationToken = default)
    {
        string busqueda = texto.Trim();
        if (busqueda.Length < 2)
        {
            return Task.FromResult(new PaginaVideojuegos());
        }

        return ObtenerPaginaAsync(
            new Dictionary<string, string?>
            {
                ["search"] = busqueda,
                ["search_precise"] = "false"
            },
            pagina,
            $"buscar:{busqueda.ToLowerInvariant()}",
            cancellationToken);
    }

    public async Task<VideojuegoDetalle> ObtenerDetalleAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        if (id <= 0)
        {
            throw new RawgExcepcion(
                "El identificador del videojuego no es válido.",
                HttpStatusCode.BadRequest);
        }

        ComprobarApiKey();
        string claveCache = $"rawg:detalle:{id}";

        VideojuegoDetalle? detalle =
            await _cache.GetOrCreateAsync(claveCache, async entrada =>
            {
                entrada.AbsoluteExpirationRelativeToNow =
                    TimeSpan.FromMinutes(
                        Math.Clamp(_opciones.MinutosCache * 2, 10, 240));

                VideojuegoRawgDto dto =
                    await ObtenerJsonAsync<VideojuegoRawgDto>(
                        $"games/{id}",
                        cancellationToken);

                PaginaRawgDto<CapturaRawgDto> capturas =
                    await ObtenerJsonAsync<PaginaRawgDto<CapturaRawgDto>>(
                        $"games/{id}/screenshots",
                        cancellationToken);

                VideojuegoResumen resumen = ConvertirResumen(dto);

                return new VideojuegoDetalle
                {
                    Id = resumen.Id,
                    Slug = resumen.Slug,
                    Nombre = resumen.Nombre,
                    FechaLanzamiento = resumen.FechaLanzamiento,
                    ImagenUrl = resumen.ImagenUrl,
                    Puntuacion = resumen.Puntuacion,
                    NumeroValoraciones = resumen.NumeroValoraciones,
                    Metacritic = resumen.Metacritic,
                    TiempoJuego = resumen.TiempoJuego,
                    Generos = resumen.Generos,
                    Plataformas = resumen.Plataformas,
                    Descripcion = dto.Descripcion,
                    SitioWeb = NormalizarUrl(dto.SitioWeb),
                    ClasificacionEdad =
                        dto.ClasificacionEdad?.Nombre ?? "Sin clasificar",
                    Desarrolladores = dto.Desarrolladores
                        .Select(elemento => elemento.Nombre)
                        .ToList()
                        .AsReadOnly(),
                    Editores = dto.Editores
                        .Select(elemento => elemento.Nombre)
                        .ToList()
                        .AsReadOnly(),
                    Tiendas = dto.Tiendas
                        .Select(elemento => elemento.Tienda.Nombre)
                        .ToList()
                        .AsReadOnly(),
                    Capturas = capturas.Resultados
                        .Select(elemento => elemento.Imagen)
                        .Where(url => !string.IsNullOrWhiteSpace(url))
                        .Take(12)
                        .ToList()
                        .AsReadOnly()
                };
            });

        return detalle
            ?? throw new RawgExcepcion(
                "No se ha podido preparar la ficha del videojuego.");
    }

    private async Task<PaginaVideojuegos> ObtenerPaginaAsync(
        Dictionary<string, string?> filtros,
        int pagina,
        string clave,
        CancellationToken cancellationToken)
    {
        ComprobarApiKey();

        pagina = Math.Max(1, pagina);
        int tamano = Math.Clamp(_opciones.TamanoPagina, 6, 40);
        string claveCache = $"rawg:{clave}:{pagina}:{tamano}";

        PaginaVideojuegos? resultado =
            await _cache.GetOrCreateAsync(claveCache, async entrada =>
            {
                entrada.AbsoluteExpirationRelativeToNow = DuracionCache();

                Dictionary<string, string?> parametros = new(filtros)
                {
                    ["page"] = pagina.ToString(CultureInfo.InvariantCulture),
                    ["page_size"] =
                        tamano.ToString(CultureInfo.InvariantCulture)
                };

                PaginaRawgDto<VideojuegoRawgDto> dto =
                    await ObtenerJsonAsync<PaginaRawgDto<VideojuegoRawgDto>>(
                        CrearRuta("games", parametros),
                        cancellationToken);

                int totalPaginas = dto.Total == 0
                    ? 0
                    : (int)Math.Ceiling(dto.Total / (double)tamano);

                return new PaginaVideojuegos
                {
                    Pagina = pagina,
                    TotalPaginas = totalPaginas,
                    TotalResultados = dto.Total,
                    Resultados = dto.Resultados
                        .Where(videojuego => videojuego.Id > 0)
                        .Select(ConvertirResumen)
                        .ToList()
                        .AsReadOnly()
                };
            });

        return resultado ?? new PaginaVideojuegos();
    }

    private async Task<T> ObtenerJsonAsync<T>(
        string ruta,
        CancellationToken cancellationToken)
    {
        string rutaConClave = QueryHelpers.AddQueryString(
            ruta,
            "key",
            _opciones.ApiKey.Trim());

        try
        {
            using HttpResponseMessage respuesta =
                await _cliente.GetAsync(rutaConClave, cancellationToken);

            if (!respuesta.IsSuccessStatusCode)
            {
                throw CrearExcepcion(respuesta.StatusCode);
            }

            T? datos = await respuesta.Content.ReadFromJsonAsync<T>(
                cancellationToken: cancellationToken);

            return datos
                ?? throw new RawgExcepcion(
                    "RAWG ha devuelto una respuesta vacía.");
        }
        catch (OperationCanceledException)
            when (!cancellationToken.IsCancellationRequested)
        {
            throw new RawgExcepcion(
                "RAWG ha tardado demasiado en responder.");
        }
        catch (HttpRequestException)
        {
            throw new RawgExcepcion(
                "No se ha podido conectar con RAWG.");
        }
        catch (JsonException)
        {
            throw new RawgExcepcion(
                "RAWG ha devuelto datos con un formato inesperado.");
        }
    }

    private string CrearRuta(
        string ruta,
        Dictionary<string, string?> parametros)
    {
        return QueryHelpers.AddQueryString(ruta, parametros);
    }

    private static Dictionary<string, string?> CrearFiltroGenero(
        string genero)
    {
        return new()
        {
            ["genres"] = genero,
            ["ordering"] = "-rating"
        };
    }

    private static VideojuegoResumen ConvertirResumen(
        VideojuegoRawgDto dto)
    {
        IReadOnlyList<string> plataformas = dto.PlataformasPadre.Count > 0
            ? dto.PlataformasPadre
                .Select(elemento => elemento.Plataforma.Nombre)
                .ToList()
                .AsReadOnly()
            : dto.Plataformas
                .Select(elemento => elemento.Plataforma.Nombre)
                .Distinct()
                .ToList()
                .AsReadOnly();

        return new VideojuegoResumen
        {
            Id = dto.Id,
            Slug = dto.Slug,
            Nombre = dto.Nombre,
            FechaLanzamiento = dto.FechaLanzamiento,
            ImagenUrl = dto.ImagenUrl,
            Puntuacion = dto.Puntuacion,
            NumeroValoraciones = dto.NumeroValoraciones,
            Metacritic = dto.Metacritic,
            TiempoJuego = dto.TiempoJuego,
            Generos = dto.Generos
                .Select(elemento => elemento.Nombre)
                .ToList()
                .AsReadOnly(),
            Plataformas = plataformas
        };
    }

    private static string NormalizarUrl(string? url)
    {
        if (!Uri.TryCreate(url, UriKind.Absolute, out Uri? resultado))
        {
            return "";
        }

        return resultado.Scheme is "http" or "https"
            ? resultado.ToString()
            : "";
    }

    private void ComprobarApiKey()
    {
        if (!_opciones.TieneApiKey)
        {
            throw new RawgExcepcion(
                "Todavía no se ha configurado la clave de RAWG.");
        }
    }

    private static RawgExcepcion CrearExcepcion(HttpStatusCode codigo)
    {
        return codigo switch
        {
            HttpStatusCode.BadRequest or
            HttpStatusCode.Unauthorized or
            HttpStatusCode.Forbidden =>
                new RawgExcepcion(
                    "La clave de RAWG no es válida o no tiene acceso.",
                    codigo),
            HttpStatusCode.NotFound =>
                new RawgExcepcion(
                    "RAWG no ha encontrado ese videojuego.",
                    codigo),
            HttpStatusCode.TooManyRequests =>
                new RawgExcepcion(
                    "Se ha alcanzado temporalmente el límite de RAWG.",
                    codigo),
            _ =>
                new RawgExcepcion(
                    $"RAWG ha respondido con el código {(int)codigo}.",
                    codigo)
        };
    }

    private TimeSpan DuracionCache()
    {
        int minutos = Math.Clamp(_opciones.MinutosCache, 1, 120);
        return TimeSpan.FromMinutes(minutos);
    }
}

using System.Globalization;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Peliculas.Configuracion;
using Peliculas.DTOs;
using Peliculas.Modelos;

namespace Peliculas.Servicios;

// Centraliza las direcciones, la autenticación y la transformación de TMDB.
public class TmdbServicio : ITmdbServicio
{
    private readonly HttpClient _cliente;
    private readonly IMemoryCache _cache;
    private readonly TmdbOpciones _opciones;

    public bool EstaConfigurado =>
        !string.IsNullOrWhiteSpace(_opciones.TokenAcceso);

    public TmdbServicio(
        HttpClient cliente,
        IMemoryCache cache,
        IOptions<TmdbOpciones> opciones)
    {
        _cliente = cliente;
        _cache = cache;
        _opciones = opciones.Value;

        _cliente.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));

        if (EstaConfigurado)
        {
            // El token viaja en una cabecera y nunca forma parte de la URL.
            _cliente.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(
                    "Bearer", _opciones.TokenAcceso);
        }
    }

    public async Task<PaginaPeliculas> ObtenerListadoAsync(
        TipoListado tipo,
        int pagina = 1,
        CancellationToken cancellationToken = default)
    {
        ComprobarConfiguracion();
        pagina = Math.Clamp(pagina, 1, 500);

        string endpoint = tipo switch
        {
            TipoListado.Tendencias => "trending/movie/week",
            TipoListado.EnCartelera => "movie/now_playing",
            TipoListado.MejorValoradas => "movie/top_rated",
            TipoListado.Proximamente => "movie/upcoming",
            _ => "movie/popular"
        };

        string claveCache = $"tmdb:listado:{tipo}:{pagina}";

        PaginaPeliculas? resultado =
            await _cache.GetOrCreateAsync(claveCache, async entrada =>
            {
                entrada.AbsoluteExpirationRelativeToNow = DuracionCache();

                string ruta = CrearRuta(endpoint,
                    new Dictionary<string, string?>
                    {
                        ["language"] = _opciones.Idioma,
                        ["region"] = _opciones.Region,
                        ["page"] = pagina.ToString(
                            CultureInfo.InvariantCulture)
                    });

                PaginaPeliculasDto dto =
                    await ObtenerJsonAsync<PaginaPeliculasDto>(
                        ruta, cancellationToken);

                return ConvertirPagina(dto);
            });

        return resultado ?? new PaginaPeliculas();
    }

    public async Task<PaginaPeliculas> BuscarAsync(
        string texto,
        int pagina = 1,
        CancellationToken cancellationToken = default)
    {
        ComprobarConfiguracion();

        string consulta = texto.Trim();
        pagina = Math.Clamp(pagina, 1, 500);
        string claveCache =
            $"tmdb:buscar:{consulta.ToLowerInvariant()}:{pagina}";

        PaginaPeliculas? resultado =
            await _cache.GetOrCreateAsync(claveCache, async entrada =>
            {
                entrada.AbsoluteExpirationRelativeToNow = DuracionCache();

                string ruta = CrearRuta("search/movie",
                    new Dictionary<string, string?>
                    {
                        ["query"] = consulta,
                        ["include_adult"] = "false",
                        ["language"] = _opciones.Idioma,
                        ["region"] = _opciones.Region,
                        ["page"] = pagina.ToString(
                            CultureInfo.InvariantCulture)
                    });

                PaginaPeliculasDto dto =
                    await ObtenerJsonAsync<PaginaPeliculasDto>(
                        ruta, cancellationToken);

                return ConvertirPagina(dto);
            });

        return resultado ?? new PaginaPeliculas();
    }

    public async Task<PeliculaDetalle> ObtenerDetalleAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        ComprobarConfiguracion();

        if (id <= 0)
        {
            throw new TmdbExcepcion(
                "El identificador de la película no es válido.");
        }

        string claveCache = $"tmdb:detalle:{id}";

        PeliculaDetalle? resultado =
            await _cache.GetOrCreateAsync(claveCache, async entrada =>
            {
                entrada.AbsoluteExpirationRelativeToNow =
                    TimeSpan.FromMinutes(
                        Math.Clamp(_opciones.MinutosCache * 2, 5, 120));

                string ruta = CrearRuta($"movie/{id}",
                    new Dictionary<string, string?>
                    {
                        ["language"] = _opciones.Idioma,
                        ["append_to_response"] =
                            "credits,videos,recommendations,watch/providers"
                    });

                PeliculaDetalleDto dto =
                    await ObtenerJsonAsync<PeliculaDetalleDto>(
                        ruta, cancellationToken);

                return ConvertirDetalle(dto);
            });

        return resultado
            ?? throw new TmdbExcepcion(
                "No se han podido preparar los detalles de la película.");
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
                ?? throw new TmdbExcepcion(
                    "TMDB ha devuelto una respuesta vacía.");
        }
        catch (OperationCanceledException)
            when (!cancellationToken.IsCancellationRequested)
        {
            throw new TmdbExcepcion(
                "TMDB ha tardado demasiado en responder.");
        }
        catch (HttpRequestException)
        {
            throw new TmdbExcepcion(
                "No se ha podido conectar con TMDB.");
        }
        catch (JsonException)
        {
            throw new TmdbExcepcion(
                "TMDB ha devuelto datos con un formato inesperado.");
        }
    }

    private static string CrearRuta(
        string endpoint,
        Dictionary<string, string?> parametros)
    {
        return QueryHelpers.AddQueryString(endpoint, parametros);
    }

    private void ComprobarConfiguracion()
    {
        if (!EstaConfigurado)
        {
            throw new TmdbExcepcion(
                "Falta configurar el API Read Access Token de TMDB.");
        }
    }

    private TimeSpan DuracionCache()
    {
        int minutos = Math.Clamp(_opciones.MinutosCache, 1, 60);
        return TimeSpan.FromMinutes(minutos);
    }

    private static TmdbExcepcion CrearExcepcion(HttpStatusCode codigo)
    {
        return codigo switch
        {
            HttpStatusCode.Unauthorized =>
                new TmdbExcepcion(
                    "El token de TMDB no es válido.", codigo),
            HttpStatusCode.NotFound =>
                new TmdbExcepcion(
                    "TMDB no ha encontrado esa película.", codigo),
            HttpStatusCode.TooManyRequests =>
                new TmdbExcepcion(
                    "Se ha alcanzado el límite temporal de TMDB.", codigo),
            _ =>
                new TmdbExcepcion(
                    $"TMDB ha respondido con el código {(int)codigo}.",
                    codigo)
        };
    }

    private static PaginaPeliculas ConvertirPagina(
        PaginaPeliculasDto dto)
    {
        return new PaginaPeliculas
        {
            Pagina = dto.Pagina,
            TotalPaginas = Math.Min(dto.TotalPaginas, 500),
            TotalResultados = dto.TotalResultados,
            Resultados = dto.Resultados
                .Select(ConvertirResumen)
                .ToList()
                .AsReadOnly()
        };
    }

    private PeliculaDetalle ConvertirDetalle(PeliculaDetalleDto dto)
    {
        EquipoDto? director = dto.Creditos.Equipo.FirstOrDefault(
            persona => persona.Trabajo.Equals(
                "Director", StringComparison.OrdinalIgnoreCase));

        VideoDto? trailer = dto.Videos.Resultados
            .Where(video =>
                video.Sitio.Equals(
                    "YouTube", StringComparison.OrdinalIgnoreCase)
                && video.Tipo.Equals(
                    "Trailer", StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(video => video.Oficial)
            .FirstOrDefault();

        dto.Proveedores.Regiones.TryGetValue(
            _opciones.Region, out ProveedoresRegionDto? proveedores);

        PeliculaResumen resumen = ConvertirResumen(dto);

        return new PeliculaDetalle
        {
            Id = resumen.Id,
            Titulo = resumen.Titulo,
            TituloOriginal = resumen.TituloOriginal,
            Sinopsis = resumen.Sinopsis,
            RutaPoster = resumen.RutaPoster,
            RutaFondo = resumen.RutaFondo,
            FechaEstreno = resumen.FechaEstreno,
            Puntuacion = resumen.Puntuacion,
            NumeroVotos = resumen.NumeroVotos,
            Eslogan = dto.Eslogan,
            DuracionMinutos = dto.Duracion,
            Estado = dto.Estado,
            PaginaOficial = dto.PaginaOficial,
            ImdbId = dto.ImdbId,
            Generos = dto.Generos
                .Select(genero => genero.Nombre)
                .ToList()
                .AsReadOnly(),
            Paises = dto.Paises
                .Select(pais => pais.Nombre)
                .ToList()
                .AsReadOnly(),
            Director = director?.Nombre,
            Reparto = dto.Creditos.Reparto
                .OrderBy(persona => persona.Orden)
                .Take(12)
                .Select(persona => new PersonaReparto
                {
                    Id = persona.Id,
                    Nombre = persona.Nombre,
                    Personaje = persona.Personaje,
                    RutaFoto = persona.RutaFoto
                })
                .ToList()
                .AsReadOnly(),
            Trailer = trailer is null
                ? null
                : new VideoTmdb
                {
                    Nombre = trailer.Nombre,
                    ClaveYoutube = trailer.Clave
                },
            Recomendaciones = dto.Recomendaciones.Resultados
                .Take(6)
                .Select(ConvertirResumen)
                .ToList()
                .AsReadOnly(),
            Disponibilidad = ConvertirProveedores(proveedores)
        };
    }

    private static PeliculaResumen ConvertirResumen(PeliculaDto dto)
    {
        DateOnly? fecha = null;

        if (DateOnly.TryParse(
            dto.FechaEstreno,
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out DateOnly fechaConvertida))
        {
            fecha = fechaConvertida;
        }

        return new PeliculaResumen
        {
            Id = dto.Id,
            Titulo = dto.Titulo,
            TituloOriginal = dto.TituloOriginal,
            Sinopsis = string.IsNullOrWhiteSpace(dto.Sinopsis)
                ? "TMDB no dispone de una sinopsis en español."
                : dto.Sinopsis,
            RutaPoster = dto.RutaPoster,
            RutaFondo = dto.RutaFondo,
            FechaEstreno = fecha,
            Puntuacion = dto.Puntuacion,
            NumeroVotos = dto.NumeroVotos
        };
    }

    private static DisponibilidadTmdb? ConvertirProveedores(
        ProveedoresRegionDto? dto)
    {
        if (dto is null)
        {
            return null;
        }

        return new DisponibilidadTmdb
        {
            Enlace = dto.Enlace,
            Suscripcion = ConvertirListaProveedores(dto.Suscripcion),
            Alquiler = ConvertirListaProveedores(dto.Alquiler),
            Compra = ConvertirListaProveedores(dto.Compra)
        };
    }

    private static IReadOnlyList<ProveedorTmdb> ConvertirListaProveedores(
        IEnumerable<ProveedorDto> proveedores)
    {
        return proveedores
            .OrderBy(proveedor => proveedor.Prioridad)
            .Select(proveedor => new ProveedorTmdb
            {
                Id = proveedor.Id,
                Nombre = proveedor.Nombre,
                RutaLogo = proveedor.RutaLogo
            })
            .ToList()
            .AsReadOnly();
    }
}

using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Futbol.Configuracion;
using Futbol.DTOs;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Futbol.Servicios;

// Centraliza las peticiones HTTP, los errores y la caché de la API.
public class FutbolServicio : IFutbolServicio
{
    private readonly HttpClient _cliente;
    private readonly IMemoryCache _cache;
    private readonly FootballDataOpciones _opciones;

    public FutbolServicio(
        HttpClient cliente,
        IMemoryCache cache,
        IOptions<FootballDataOpciones> opciones)
    {
        _cliente = cliente;
        _cache = cache;
        _opciones = opciones.Value;
    }

    public bool EstaConfigurada =>
        !string.IsNullOrWhiteSpace(_opciones.ApiKey);

    public async Task<IReadOnlyList<CompeticionDto>>
        ObtenerCompeticionesAsync(
            CancellationToken cancellationToken = default)
    {
        CompeticionesRespuestaDto respuesta =
            await ObtenerConCacheAsync<CompeticionesRespuestaDto>(
                "competiciones",
                "competitions",
                DuracionCache(larga: true),
                cancellationToken);

        return respuesta.Competiciones
            .Where(competicion =>
                !string.IsNullOrWhiteSpace(competicion.Codigo))
            .OrderBy(competicion => competicion.Area?.Nombre)
            .ThenBy(competicion => competicion.Nombre)
            .ToList()
            .AsReadOnly();
    }

    public async Task<IReadOnlyList<PartidoDto>>
        ObtenerPartidosPorFechaAsync(
            DateOnly fecha,
            CancellationToken cancellationToken = default)
    {
        string textoFecha = fecha.ToString("yyyy-MM-dd");
        string ruta = QueryHelpers.AddQueryString(
            "matches",
            new Dictionary<string, string?>
            {
                ["dateFrom"] = textoFecha,
                ["dateTo"] = textoFecha
            });

        PartidosRespuestaDto respuesta =
            await ObtenerConCacheAsync<PartidosRespuestaDto>(
                $"partidos-fecha:{textoFecha}",
                ruta,
                DuracionCache(),
                cancellationToken);

        return OrdenarPartidos(respuesta.Partidos);
    }

    public Task<ClasificacionRespuestaDto> ObtenerClasificacionAsync(
        string codigo,
        CancellationToken cancellationToken = default)
    {
        string codigoSeguro = ValidarCodigo(codigo);

        return ObtenerConCacheAsync<ClasificacionRespuestaDto>(
            $"clasificacion:{codigoSeguro}",
            $"competitions/{codigoSeguro}/standings",
            DuracionCache(),
            cancellationToken);
    }

    public async Task<IReadOnlyList<PartidoDto>>
        ObtenerPartidosCompeticionAsync(
            string codigo,
            CancellationToken cancellationToken = default)
    {
        string codigoSeguro = ValidarCodigo(codigo);
        DateOnly hoy = DateOnly.FromDateTime(DateTime.Today);

        string ruta = QueryHelpers.AddQueryString(
            $"competitions/{codigoSeguro}/matches",
            new Dictionary<string, string?>
            {
                ["dateFrom"] = hoy.AddDays(-14).ToString("yyyy-MM-dd"),
                ["dateTo"] = hoy.AddDays(45).ToString("yyyy-MM-dd")
            });

        PartidosRespuestaDto respuesta =
            await ObtenerConCacheAsync<PartidosRespuestaDto>(
                $"partidos-competicion:{codigoSeguro}",
                ruta,
                DuracionCache(),
                cancellationToken);

        return OrdenarPartidos(respuesta.Partidos);
    }

    public Task<GoleadoresRespuestaDto> ObtenerGoleadoresAsync(
        string codigo,
        CancellationToken cancellationToken = default)
    {
        string codigoSeguro = ValidarCodigo(codigo);
        string ruta = QueryHelpers.AddQueryString(
            $"competitions/{codigoSeguro}/scorers",
            "limit",
            "20");

        return ObtenerConCacheAsync<GoleadoresRespuestaDto>(
            $"goleadores:{codigoSeguro}",
            ruta,
            DuracionCache(),
            cancellationToken);
    }

    public Task<EquiposRespuestaDto> ObtenerEquiposAsync(
        string codigo,
        CancellationToken cancellationToken = default)
    {
        string codigoSeguro = ValidarCodigo(codigo);

        return ObtenerConCacheAsync<EquiposRespuestaDto>(
            $"equipos:{codigoSeguro}",
            $"competitions/{codigoSeguro}/teams",
            DuracionCache(larga: true),
            cancellationToken);
    }

    public Task<EquipoDetalleDto> ObtenerEquipoAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        ValidarIdEquipo(id);

        return ObtenerConCacheAsync<EquipoDetalleDto>(
            $"equipo:{id}",
            $"teams/{id}",
            DuracionCache(larga: true),
            cancellationToken);
    }

    public async Task<IReadOnlyList<PartidoDto>>
        ObtenerPartidosEquipoAsync(
            int id,
            string estado,
            CancellationToken cancellationToken = default)
    {
        ValidarIdEquipo(id);

        string estadoSeguro = estado is "FINISHED" or "SCHEDULED"
            ? estado
            : throw new FutbolApiExcepcion(
                "El estado solicitado no es válido.",
                HttpStatusCode.BadRequest);

        string ruta = QueryHelpers.AddQueryString(
            $"teams/{id}/matches",
            new Dictionary<string, string?>
            {
                ["status"] = estadoSeguro,
                ["limit"] = "5"
            });

        PartidosRespuestaDto respuesta =
            await ObtenerConCacheAsync<PartidosRespuestaDto>(
                $"partidos-equipo:{id}:{estadoSeguro}",
                ruta,
                DuracionCache(),
                cancellationToken);

        IEnumerable<PartidoDto> partidos = respuesta.Partidos;

        // Los próximos se ordenan hacia delante y los resultados hacia atrás.
        partidos = estadoSeguro == "FINISHED"
            ? partidos.OrderByDescending(partido => partido.FechaUtc)
            : partidos.OrderBy(partido => partido.FechaUtc);

        return partidos.Take(5).ToList().AsReadOnly();
    }

    private async Task<T> ObtenerConCacheAsync<T>(
        string clave,
        string ruta,
        TimeSpan duracion,
        CancellationToken cancellationToken)
        where T : class
    {
        ComprobarApiKey();
        string claveCompleta = $"football-data:{clave}";

        T? resultado = await _cache.GetOrCreateAsync(
            claveCompleta,
            async entrada =>
            {
                entrada.AbsoluteExpirationRelativeToNow = duracion;
                return await ObtenerJsonAsync<T>(ruta, cancellationToken);
            });

        return resultado
            ?? throw new FutbolApiExcepcion(
                "La API ha devuelto una respuesta vacía.");
    }

    private async Task<T> ObtenerJsonAsync<T>(
        string ruta,
        CancellationToken cancellationToken)
    {
        using HttpRequestMessage peticion =
            new(HttpMethod.Get, ruta);

        // El token viaja desde el servidor, nunca desde el navegador.
        peticion.Headers.Add("X-Auth-Token", _opciones.ApiKey.Trim());

        try
        {
            using HttpResponseMessage respuesta =
                await _cliente.SendAsync(peticion, cancellationToken);

            if (!respuesta.IsSuccessStatusCode)
            {
                throw CrearExcepcion(respuesta.StatusCode);
            }

            T? datos = await respuesta.Content.ReadFromJsonAsync<T>(
                cancellationToken: cancellationToken);

            return datos
                ?? throw new FutbolApiExcepcion(
                    "football-data.org ha devuelto una respuesta vacía.");
        }
        catch (OperationCanceledException)
            when (!cancellationToken.IsCancellationRequested)
        {
            throw new FutbolApiExcepcion(
                "football-data.org ha tardado demasiado en responder.");
        }
        catch (HttpRequestException)
        {
            throw new FutbolApiExcepcion(
                "No se ha podido conectar con football-data.org.");
        }
        catch (JsonException)
        {
            throw new FutbolApiExcepcion(
                "La API ha devuelto datos con un formato inesperado.");
        }
    }

    private static FutbolApiExcepcion CrearExcepcion(
        HttpStatusCode codigo)
    {
        string mensaje = codigo switch
        {
            HttpStatusCode.BadRequest =>
                "La API no ha aceptado los filtros enviados.",
            HttpStatusCode.Unauthorized =>
                "El token de football-data.org no es válido.",
            HttpStatusCode.Forbidden =>
                "Tu plan de football-data.org no permite consultar estos datos.",
            HttpStatusCode.NotFound =>
                "No se han encontrado datos para esta consulta.",
            HttpStatusCode.TooManyRequests =>
                "Se ha alcanzado el límite de peticiones. Espera un minuto.",
            _ =>
                "football-data.org no ha podido completar la consulta."
        };

        return new FutbolApiExcepcion(mensaje, codigo);
    }

    private void ComprobarApiKey()
    {
        if (!EstaConfigurada)
        {
            throw new FutbolApiExcepcion(
                "Falta configurar FootballData:ApiKey.");
        }
    }

    private static string ValidarCodigo(string codigo)
    {
        string resultado = (codigo ?? "").Trim().ToUpperInvariant();

        if (resultado.Length is < 2 or > 10
            || resultado.Any(caracter =>
                !char.IsLetterOrDigit(caracter)))
        {
            throw new FutbolApiExcepcion(
                "El código de competición no es válido.",
                HttpStatusCode.BadRequest);
        }

        return resultado;
    }

    private static void ValidarIdEquipo(int id)
    {
        if (id <= 0)
        {
            throw new FutbolApiExcepcion(
                "El identificador del equipo no es válido.",
                HttpStatusCode.BadRequest);
        }
    }

    private TimeSpan DuracionCache(bool larga = false)
    {
        int minutos = Math.Clamp(_opciones.MinutosCache, 5, 120);
        return TimeSpan.FromMinutes(larga ? minutos * 4 : minutos);
    }

    private static IReadOnlyList<PartidoDto> OrdenarPartidos(
        IEnumerable<PartidoDto> partidos)
    {
        return partidos
            .OrderBy(partido => partido.FechaUtc)
            .ToList()
            .AsReadOnly();
    }
}

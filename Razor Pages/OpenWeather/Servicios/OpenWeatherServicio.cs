using System.Globalization;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using OpenWeather.Configuracion;
using OpenWeather.DTOs;
using OpenWeather.Modelos;

namespace OpenWeather.Servicios;

// Centraliza toda la comunicación con OpenWeather.
// Las Razor Pages no necesitan conocer direcciones, claves ni estructuras JSON.
public class OpenWeatherServicio : IOpenWeatherServicio
{
    private readonly HttpClient _cliente;
    private readonly IMemoryCache _cache;
    private readonly OpenWeatherOpciones _opciones;

    public bool EstaConfigurado => !string.IsNullOrWhiteSpace(_opciones.ApiKey);

    public OpenWeatherServicio(
        HttpClient cliente,
        IMemoryCache cache,
        IOptions<OpenWeatherOpciones> opciones)
    {
        _cliente = cliente;
        _cache = cache;
        _opciones = opciones.Value;
    }

    public async Task<IReadOnlyList<Lugar>> BuscarLugaresAsync(
        string texto,
        CancellationToken cancellationToken = default)
    {
        ComprobarConfiguracion();

        // Normalizamos el texto para que dos búsquedas equivalentes compartan caché.
        string consulta = texto.Trim();
        string claveCache = $"lugares:{consulta.ToLowerInvariant()}";

        IReadOnlyList<Lugar>? lugares =
            await _cache.GetOrCreateAsync(claveCache, async entrada =>
            {
                entrada.AbsoluteExpirationRelativeToNow = DuracionCache();

                string ruta = CrearRuta("geo/1.0/direct", new Dictionary<string, string?>
                {
                    ["q"] = consulta,
                    ["limit"] = "5",
                    ["appid"] = _opciones.ApiKey
                });

                List<LugarDto> respuesta =
                    await ObtenerJsonAsync<List<LugarDto>>(ruta, cancellationToken);

                return respuesta
                    .Select(ConvertirLugar)
                    .ToList()
                    .AsReadOnly();
            });

        return lugares ?? [];
    }

    public async Task<Lugar?> BuscarLugarPorCoordenadasAsync(
        double latitud,
        double longitud,
        CancellationToken cancellationToken = default)
    {
        ComprobarConfiguracion();
        ComprobarCoordenadas(latitud, longitud);

        string claveCache =
            $"lugar:{latitud.ToString("F4", CultureInfo.InvariantCulture)}:" +
            $"{longitud.ToString("F4", CultureInfo.InvariantCulture)}";

        return await _cache.GetOrCreateAsync(claveCache, async entrada =>
        {
            entrada.AbsoluteExpirationRelativeToNow = DuracionCache();

            string ruta = CrearRuta("geo/1.0/reverse", new Dictionary<string, string?>
            {
                ["lat"] = latitud.ToString(CultureInfo.InvariantCulture),
                ["lon"] = longitud.ToString(CultureInfo.InvariantCulture),
                ["limit"] = "1",
                ["appid"] = _opciones.ApiKey
            });

            List<LugarDto> respuesta =
                await ObtenerJsonAsync<List<LugarDto>>(ruta, cancellationToken);

            LugarDto? primero = respuesta.FirstOrDefault();
            return primero is null ? null : ConvertirLugar(primero);
        });
    }

    public async Task<InformeMeteorologico> ObtenerInformeAsync(
        Lugar lugar,
        Unidades unidades,
        CancellationToken cancellationToken = default)
    {
        ComprobarConfiguracion();
        ComprobarCoordenadas(lugar.Latitud, lugar.Longitud);

        string claveCache =
            $"informe:{lugar.Latitud.ToString("F4", CultureInfo.InvariantCulture)}:" +
            $"{lugar.Longitud.ToString("F4", CultureInfo.InvariantCulture)}:" +
            unidades.ParaApi();

        InformeMeteorologico? informe =
            await _cache.GetOrCreateAsync(claveCache, async entrada =>
            {
                entrada.AbsoluteExpirationRelativeToNow = DuracionCache();

                string latitud = lugar.Latitud.ToString(CultureInfo.InvariantCulture);
                string longitud = lugar.Longitud.ToString(CultureInfo.InvariantCulture);

                // Las tres peticiones comienzan juntas para reducir el tiempo de espera.
                Task<TiempoActualDto> actualTask =
                    ObtenerJsonAsync<TiempoActualDto>(
                        CrearRutaMeteorologica(
                            "data/2.5/weather", latitud, longitud, unidades),
                        cancellationToken);

                Task<PrevisionDto> previsionTask =
                    ObtenerJsonAsync<PrevisionDto>(
                        CrearRutaMeteorologica(
                            "data/2.5/forecast", latitud, longitud, unidades),
                        cancellationToken);

                Task<CalidadAireRespuestaDto?> aireTask =
                    ObtenerCalidadAireSeguraAsync(
                        latitud, longitud, cancellationToken);

                await Task.WhenAll(actualTask, previsionTask, aireTask);

                TiempoActualDto actualDto = await actualTask;
                PrevisionDto previsionDto = await previsionTask;
                CalidadAireRespuestaDto? aireDto = await aireTask;

                // El desfase permite mostrar las horas locales del lugar consultado.
                int desfase = actualDto.DesfaseHorario;

                return new InformeMeteorologico
                {
                    Lugar = lugar,
                    Unidades = unidades,
                    Actual = ConvertirActual(actualDto, unidades),
                    ProximasHoras = ConvertirPeriodos(
                        previsionDto.Periodos, desfase, unidades),
                    ProximosDias = ConvertirDias(
                        previsionDto.Periodos, desfase, unidades),
                    Aire = ConvertirAire(aireDto, desfase)
                };
            });

        // La fábrica siempre devuelve un informe, pero comprobamos el valor por seguridad.
        return informe
            ?? throw new OpenWeatherExcepcion(
                "No se ha podido preparar el informe meteorológico.");
    }

    private async Task<CalidadAireRespuestaDto?> ObtenerCalidadAireSeguraAsync(
        string latitud,
        string longitud,
        CancellationToken cancellationToken)
    {
        string ruta = CrearRuta("data/2.5/air_pollution",
            new Dictionary<string, string?>
            {
                ["lat"] = latitud,
                ["lon"] = longitud,
                ["appid"] = _opciones.ApiKey
            });

        try
        {
            return await ObtenerJsonAsync<CalidadAireRespuestaDto>(
                ruta, cancellationToken);
        }
        catch (OpenWeatherExcepcion excepcion)
            when (excepcion.CodigoEstado is HttpStatusCode.Forbidden
                or HttpStatusCode.NotFound)
        {
            // Algunos planes podrían no incluir este endpoint.
            // El resto del informe sigue siendo útil aunque falte esta sección.
            return null;
        }
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
                ?? throw new OpenWeatherExcepcion(
                    "OpenWeather ha devuelto una respuesta vacía.");
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            throw new OpenWeatherExcepcion(
                "OpenWeather ha tardado demasiado en responder.");
        }
        catch (HttpRequestException)
        {
            throw new OpenWeatherExcepcion(
                "No se ha podido conectar con OpenWeather. Comprueba la conexión.");
        }
        catch (JsonException)
        {
            throw new OpenWeatherExcepcion(
                "OpenWeather ha devuelto datos con un formato inesperado.");
        }
    }

    private string CrearRutaMeteorologica(
        string endpoint,
        string latitud,
        string longitud,
        Unidades unidades)
    {
        return CrearRuta(endpoint, new Dictionary<string, string?>
        {
            ["lat"] = latitud,
            ["lon"] = longitud,
            ["units"] = unidades.ParaApi(),
            ["lang"] = _opciones.Idioma,
            ["appid"] = _opciones.ApiKey
        });
    }

    private static string CrearRuta(
        string endpoint,
        Dictionary<string, string?> parametros)
    {
        // QueryHelpers escapa correctamente espacios, tildes y otros caracteres.
        return QueryHelpers.AddQueryString(endpoint, parametros);
    }

    private static OpenWeatherExcepcion CrearExcepcion(HttpStatusCode codigo)
    {
        return codigo switch
        {
            HttpStatusCode.Unauthorized =>
                new OpenWeatherExcepcion(
                    "La clave de OpenWeather no es válida o todavía no está activa.",
                    codigo),
            HttpStatusCode.TooManyRequests =>
                new OpenWeatherExcepcion(
                    "Se ha alcanzado el límite temporal de peticiones de OpenWeather.",
                    codigo),
            HttpStatusCode.NotFound =>
                new OpenWeatherExcepcion(
                    "OpenWeather no ha encontrado datos para ese lugar.",
                    codigo),
            _ =>
                new OpenWeatherExcepcion(
                    $"OpenWeather ha respondido con el código {(int)codigo}.",
                    codigo)
        };
    }

    private void ComprobarConfiguracion()
    {
        if (!EstaConfigurado)
        {
            throw new OpenWeatherExcepcion(
                "Falta configurar la clave de acceso de OpenWeather.");
        }
    }

    private static void ComprobarCoordenadas(double latitud, double longitud)
    {
        if (latitud is < -90 or > 90 || longitud is < -180 or > 180)
        {
            throw new OpenWeatherExcepcion(
                "Las coordenadas recibidas no son válidas.");
        }
    }

    private TimeSpan DuracionCache()
    {
        // Evitamos duraciones negativas o excesivas introducidas por error.
        int minutos = Math.Clamp(_opciones.MinutosCache, 1, 60);
        return TimeSpan.FromMinutes(minutos);
    }

    private Lugar ConvertirLugar(LugarDto dto)
    {
        // Si la API incluye el nombre en español, se prefiere al nombre general.
        string nombre = dto.NombresLocales?.GetValueOrDefault(_opciones.Idioma)
            ?? dto.Nombre;

        return new Lugar
        {
            Nombre = nombre,
            Region = dto.Region,
            Pais = dto.Pais,
            Latitud = dto.Latitud,
            Longitud = dto.Longitud
        };
    }

    private static TiempoActual ConvertirActual(
        TiempoActualDto dto,
        Unidades unidades)
    {
        EstadoCieloDto estado = dto.Estados.FirstOrDefault() ?? new EstadoCieloDto();

        return new TiempoActual
        {
            Fecha = ConvertirFecha(dto.FechaUnix, dto.DesfaseHorario),
            Descripcion =
                UtilidadesMeteorologicas.PrimeraMayuscula(estado.Descripcion),
            Icono = estado.Icono,
            Temperatura = dto.Valores.Temperatura,
            Sensacion = dto.Valores.Sensacion,
            Humedad = dto.Valores.Humedad,
            Presion = dto.Valores.Presion,
            VisibilidadMetros = dto.Visibilidad,
            Nubosidad = dto.Nubes.Porcentaje,
            Viento = ConvertirViento(dto.Viento.Velocidad, unidades),
            Racha = dto.Viento.Racha is null
                ? null
                : ConvertirViento(dto.Viento.Racha.Value, unidades),
            DireccionViento = dto.Viento.Direccion,
            Amanecer = ConvertirFecha(
                dto.Sistema.AmanecerUnix, dto.DesfaseHorario),
            Atardecer = ConvertirFecha(
                dto.Sistema.AtardecerUnix, dto.DesfaseHorario)
        };
    }

    private static IReadOnlyList<PrevisionPeriodo> ConvertirPeriodos(
        IEnumerable<PeriodoDto> periodos,
        int desfase,
        Unidades unidades)
    {
        return periodos
            .Take(16)
            .Select(periodo =>
            {
                EstadoCieloDto estado =
                    periodo.Estados.FirstOrDefault() ?? new EstadoCieloDto();

                return new PrevisionPeriodo
                {
                    Fecha = ConvertirFecha(periodo.FechaUnix, desfase),
                    Descripcion = UtilidadesMeteorologicas.PrimeraMayuscula(
                        estado.Descripcion),
                    Icono = estado.Icono,
                    Temperatura = periodo.Valores.Temperatura,
                    Sensacion = periodo.Valores.Sensacion,
                    Humedad = periodo.Valores.Humedad,
                    ProbabilidadLluvia =
                        periodo.ProbabilidadPrecipitacion * 100,
                    LluviaMilimetros =
                        (periodo.Lluvia?.TresHoras ?? 0)
                        + (periodo.Nieve?.TresHoras ?? 0),
                    Viento = ConvertirViento(
                        periodo.Viento.Velocidad, unidades)
                };
            })
            .ToList()
            .AsReadOnly();
    }

    private static IReadOnlyList<PrevisionDiaria> ConvertirDias(
        IEnumerable<PeriodoDto> periodos,
        int desfase,
        Unidades unidades)
    {
        return periodos
            .Select(periodo => new
            {
                Periodo = periodo,
                FechaLocal = ConvertirFecha(periodo.FechaUnix, desfase)
            })
            .GroupBy(item => DateOnly.FromDateTime(item.FechaLocal.DateTime))
            .Take(5)
            .Select(grupo =>
            {
                // El intervalo más próximo al mediodía representa visualmente el día.
                var representativo = grupo
                    .OrderBy(item => Math.Abs(item.FechaLocal.Hour - 12))
                    .First();

                EstadoCieloDto estado =
                    representativo.Periodo.Estados.FirstOrDefault()
                    ?? new EstadoCieloDto();

                return new PrevisionDiaria
                {
                    Fecha = grupo.Key,
                    Descripcion = UtilidadesMeteorologicas.PrimeraMayuscula(
                        estado.Descripcion),
                    Icono = estado.Icono,
                    Minima = grupo.Min(item => item.Periodo.Valores.Minima),
                    Maxima = grupo.Max(item => item.Periodo.Valores.Maxima),
                    Humedad = (int)Math.Round(
                        grupo.Average(item => item.Periodo.Valores.Humedad)),
                    ProbabilidadLluvia =
                        grupo.Max(item =>
                            item.Periodo.ProbabilidadPrecipitacion) * 100,
                    LluviaMilimetros = grupo.Sum(item =>
                        (item.Periodo.Lluvia?.TresHoras ?? 0)
                        + (item.Periodo.Nieve?.TresHoras ?? 0)),
                    VientoMaximo = grupo.Max(item =>
                        ConvertirViento(
                            item.Periodo.Viento.Velocidad, unidades))
                };
            })
            .ToList()
            .AsReadOnly();
    }

    private static CalidadAire? ConvertirAire(
        CalidadAireRespuestaDto? respuesta,
        int desfase)
    {
        CalidadAirePeriodoDto? periodo = respuesta?.Periodos.FirstOrDefault();

        if (periodo is null)
        {
            return null;
        }

        ComponentesAireDto componentes = periodo.Componentes;

        return new CalidadAire
        {
            Indice = periodo.Indice.Valor,
            Fecha = ConvertirFecha(periodo.FechaUnix, desfase),
            MonoxidoCarbono = componentes.Co,
            MonoxidoNitrogeno = componentes.No,
            DioxidoNitrogeno = componentes.No2,
            Ozono = componentes.O3,
            DioxidoAzufre = componentes.So2,
            Pm25 = componentes.Pm25,
            Pm10 = componentes.Pm10,
            Amoniaco = componentes.Nh3
        };
    }

    private static DateTimeOffset ConvertirFecha(long segundos, int desfase)
    {
        return DateTimeOffset
            .FromUnixTimeSeconds(segundos)
            .ToOffset(TimeSpan.FromSeconds(desfase));
    }

    private static double ConvertirViento(double velocidad, Unidades unidades)
    {
        // OpenWeather devuelve m/s en métrico y mph en imperial.
        return unidades == Unidades.Metrico ? velocidad * 3.6 : velocidad;
    }
}

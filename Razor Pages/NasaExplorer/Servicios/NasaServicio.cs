using System.Globalization;
using System.Text.Json;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using NasaExplorer.Configuracion;
using NasaExplorer.DTOs;

namespace NasaExplorer.Servicios;

// Consume varias APIs de NASA desde el servidor para no revelar la clave.
public class NasaServicio(
    IHttpClientFactory fabricaHttp,
    IOptions<NasaOpciones> opciones,
    IMemoryCache cache,
    ILogger<NasaServicio> logger) : INasaServicio
{
    private readonly NasaOpciones _opciones = opciones.Value;

    // Las opciones web aceptan los nombres de propiedades que devuelven las APIs.
    private static readonly JsonSerializerOptions JsonOpciones = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true
    };

    public async Task<ApodDto> ObtenerApodAsync(DateOnly fecha)
    {
        ComprobarClave();

        Dictionary<string, string?> parametros = new()
        {
            ["api_key"] = _opciones.ApiKey,
            ["date"] = fecha.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
            ["thumbs"] = "true"
        };

        string url = QueryHelpers.AddQueryString("planetary/apod", parametros);
        return await ObtenerJsonAsync<ApodDto>("nasa", url, $"apod-{fecha:yyyy-MM-dd}");
    }

    public async Task<MediaRespuestaDto> BuscarMultimediaAsync(
        string busqueda,
        string tipo,
        int? anioDesde,
        int? anioHasta,
        int pagina)
    {
        // La API exige al menos un filtro; la página propone "moon" inicialmente.
        Dictionary<string, string?> parametros = new()
        {
            ["q"] = busqueda,
            ["media_type"] = tipo == "todos" ? "image,video,audio" : tipo,
            ["year_start"] = anioDesde?.ToString(CultureInfo.InvariantCulture),
            ["year_end"] = anioHasta?.ToString(CultureInfo.InvariantCulture),
            ["page"] = Math.Max(1, pagina).ToString(CultureInfo.InvariantCulture),
            ["page_size"] = "24"
        };

        string url = QueryHelpers.AddQueryString("search", LimpiarParametros(parametros));
        string claveCache = $"media-{busqueda}-{tipo}-{anioDesde}-{anioHasta}-{pagina}";
        return await ObtenerJsonAsync<MediaRespuestaDto>("imagenes", url, claveCache);
    }

    public async Task<MediaItemDto?> ObtenerMultimediaAsync(string nasaId)
    {
        // Buscar por identificador devuelve los metadatos completos de la pieza.
        Dictionary<string, string?> parametros = new()
        {
            ["nasa_id"] = nasaId
        };

        string url = QueryHelpers.AddQueryString("search", parametros);
        MediaRespuestaDto respuesta = await ObtenerJsonAsync<MediaRespuestaDto>(
            "imagenes",
            url,
            $"media-detalle-{nasaId}");

        return respuesta.Coleccion.Elementos.FirstOrDefault();
    }

    public async Task<List<string>> ObtenerArchivosMultimediaAsync(string nasaId)
    {
        string idSeguro = Uri.EscapeDataString(nasaId);
        MediaManifiestoDto manifiesto = await ObtenerJsonAsync<MediaManifiestoDto>(
            "imagenes",
            $"asset/{idSeguro}",
            $"media-archivos-{nasaId}");

        return manifiesto.Coleccion.Elementos
            .Select(elemento => elemento.Href)
            .Where(url => !url.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    public async Task<List<EpicImagenDto>> ObtenerEpicAsync(
        string coleccion,
        DateOnly? fecha)
    {
        string coleccionSegura = coleccion switch
        {
            "enhanced" => "enhanced",
            "aerosol" => "aerosol",
            "cloud" => "cloud",
            _ => "natural"
        };

        string ruta = fecha is null
            ? $"api/{coleccionSegura}"
            : $"api/{coleccionSegura}/date/{fecha:yyyy-MM-dd}";

        List<EpicImagenDto> imagenes = await ObtenerJsonAsync<List<EpicImagenDto>>(
            "epic",
            ruta,
            $"epic-{coleccionSegura}-{fecha}");

        // Las URLs se forman con la fecha y el nombre que entrega la API.
        foreach (EpicImagenDto imagen in imagenes)
        {
            string carpeta = imagen.Fecha.ToString("yyyy/MM/dd", CultureInfo.InvariantCulture);
            imagen.ImagenUrl =
                $"https://epic.gsfc.nasa.gov/archive/{coleccionSegura}/{carpeta}/png/{imagen.Imagen}.png";
            imagen.MiniaturaUrl =
                $"https://epic.gsfc.nasa.gov/archive/{coleccionSegura}/{carpeta}/thumbs/{imagen.Imagen}.jpg";
        }

        return imagenes;
    }

    public async Task<List<EonetEventoDto>> ObtenerEventosNaturalesAsync(
        string estado,
        string? categoria,
        int dias)
    {
        Dictionary<string, string?> parametros = new()
        {
            ["status"] = estado is "closed" or "all" ? estado : "open",
            ["category"] = categoria,
            ["days"] = Math.Clamp(dias, 1, 365).ToString(CultureInfo.InvariantCulture),
            ["limit"] = "100"
        };

        string url = QueryHelpers.AddQueryString("events", LimpiarParametros(parametros));
        EonetRespuestaDto respuesta = await ObtenerJsonAsync<EonetRespuestaDto>(
            "eonet",
            url,
            $"eonet-{estado}-{categoria}-{dias}");

        return respuesta.Eventos;
    }

    public async Task<AsteroidesResultado> ObtenerAsteroidesAsync(
        DateOnly fechaInicio,
        DateOnly fechaFin)
    {
        ComprobarClave();

        // NeoWs admite intervalos de siete días como máximo.
        if (fechaFin < fechaInicio || fechaFin.DayNumber - fechaInicio.DayNumber > 7)
        {
            throw new ApiExternaExcepcion(
                "El intervalo de asteroides debe estar entre uno y siete días.");
        }

        Dictionary<string, string?> parametros = new()
        {
            ["start_date"] = fechaInicio.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
            ["end_date"] = fechaFin.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
            ["api_key"] = _opciones.ApiKey
        };

        string url = QueryHelpers.AddQueryString("neo/rest/v1/feed", parametros);
        JsonDocument documento = await ObtenerJsonAsync<JsonDocument>(
            "nasa",
            url,
            $"neos-{fechaInicio}-{fechaFin}");

        List<AsteroideVista> asteroides = [];
        JsonElement raiz = documento.RootElement;

        if (raiz.TryGetProperty("near_earth_objects", out JsonElement dias))
        {
            foreach (JsonProperty dia in dias.EnumerateObject())
            {
                foreach (JsonElement elemento in dia.Value.EnumerateArray())
                {
                    asteroides.Add(ConvertirAsteroide(elemento));
                }
            }
        }

        return new AsteroidesResultado
        {
            FechaInicio = fechaInicio.ToDateTime(TimeOnly.MinValue),
            FechaFin = fechaFin.ToDateTime(TimeOnly.MinValue),
            Total = raiz.TryGetProperty("element_count", out JsonElement total)
                ? total.GetInt32()
                : asteroides.Count,
            Asteroides = asteroides
                .OrderByDescending(asteroide => asteroide.Peligroso)
                .ThenBy(asteroide => asteroide.FechaAproximacion)
                .ToList()
        };
    }

    public async Task<List<DonkiEventoVista>> ObtenerClimaEspacialAsync(
        string tipo,
        DateOnly fechaInicio,
        DateOnly fechaFin)
    {
        string tipoSeguro = tipo is "CME" or "GST" or "FLR" or "IPS" ? tipo : "CME";
        Dictionary<string, string?> parametros = new()
        {
            ["startDate"] = fechaInicio.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
            ["endDate"] = fechaFin.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)
        };

        string url = QueryHelpers.AddQueryString(tipoSeguro, parametros);
        JsonDocument documento = await ObtenerJsonAsync<JsonDocument>(
            "donki",
            url,
            $"donki-{tipoSeguro}-{fechaInicio}-{fechaFin}");

        List<DonkiEventoVista> eventos = [];
        foreach (JsonElement elemento in documento.RootElement.EnumerateArray())
        {
            eventos.Add(ConvertirDonki(elemento, tipoSeguro));
        }

        return eventos.OrderByDescending(evento => evento.Fecha).ToList();
    }

    public async Task<List<ExoplanetaDto>> BuscarExoplanetasAsync(
        string? busqueda,
        string? metodo,
        int? anioDesde,
        int limite)
    {
        int limiteSeguro = Math.Clamp(limite, 10, 100);
        List<string> condiciones = ["pl_name is not null"];

        // Se duplican comillas para que el texto no pueda alterar la consulta ADQL.
        if (!string.IsNullOrWhiteSpace(busqueda))
        {
            string texto = busqueda.Trim().Replace("'", "''");
            condiciones.Add(
                $"(upper(pl_name) like upper('%{texto}%') or upper(hostname) like upper('%{texto}%'))");
        }

        if (!string.IsNullOrWhiteSpace(metodo))
        {
            string metodoSeguro = metodo.Trim().Replace("'", "''");
            condiciones.Add($"discoverymethod = '{metodoSeguro}'");
        }

        if (anioDesde is not null)
        {
            condiciones.Add($"disc_year >= {Math.Clamp(anioDesde.Value, 1988, 2100)}");
        }

        string columnas =
            "pl_name,hostname,discoverymethod,disc_year,pl_rade,pl_bmasse,"
            + "pl_orbper,pl_eqt,sy_dist,sy_pnum";
        string consulta =
            $"select top {limiteSeguro} {columnas} from pscomppars "
            + $"where {string.Join(" and ", condiciones)} order by disc_year desc";

        Dictionary<string, string?> parametros = new()
        {
            ["query"] = consulta,
            ["format"] = "json"
        };

        string url = QueryHelpers.AddQueryString("sync", parametros);
        return await ObtenerJsonAsync<List<ExoplanetaDto>>(
            "exoplanetas",
            url,
            $"exoplanetas-{busqueda}-{metodo}-{anioDesde}-{limiteSeguro}");
    }

    private async Task<T> ObtenerJsonAsync<T>(
        string cliente,
        string url,
        string claveCache)
    {
        if (cache.TryGetValue(claveCache, out T? valorCache) && valorCache is not null)
        {
            return valorCache;
        }

        try
        {
            HttpClient http = fabricaHttp.CreateClient(cliente);
            using HttpResponseMessage respuesta = await http.GetAsync(url);

            if (!respuesta.IsSuccessStatusCode)
            {
                logger.LogWarning(
                    "La API {Cliente} respondió con el estado {Estado}.",
                    cliente,
                    respuesta.StatusCode);

                throw new ApiExternaExcepcion(
                    $"El servicio {NombreServicio(cliente)} no está disponible "
                    + $"o ha rechazado la petición ({(int)respuesta.StatusCode}).");
            }

            await using Stream contenido = await respuesta.Content.ReadAsStreamAsync();
            T? resultado = await JsonSerializer.DeserializeAsync<T>(contenido, JsonOpciones);

            if (resultado is null)
            {
                throw new ApiExternaExcepcion(
                    $"El servicio {NombreServicio(cliente)} ha devuelto una respuesta vacía.");
            }

            cache.Set(
                claveCache,
                resultado,
                TimeSpan.FromMinutes(Math.Clamp(_opciones.MinutosCache, 1, 120)));

            return resultado;
        }
        catch (ApiExternaExcepcion)
        {
            throw;
        }
        catch (Exception excepcion)
        {
            logger.LogError(excepcion, "Error al consultar {Cliente}.", cliente);
            throw new ApiExternaExcepcion(
                $"No se ha podido conectar con {NombreServicio(cliente)}. "
                + "Prueba de nuevo dentro de unos minutos.",
                excepcion);
        }
    }

    private void ComprobarClave()
    {
        if (string.IsNullOrWhiteSpace(_opciones.ApiKey))
        {
            throw new ApiExternaExcepcion(
                "Falta la clave de NASA. Ejecuta: "
                + "dotnet user-secrets set \"Nasa:ApiKey\" \"TU_CLAVE\"");
        }
    }

    private static Dictionary<string, string?> LimpiarParametros(
        Dictionary<string, string?> parametros)
    {
        return parametros
            .Where(par => !string.IsNullOrWhiteSpace(par.Value))
            .ToDictionary(par => par.Key, par => par.Value);
    }

    private static string NombreServicio(string cliente) => cliente switch
    {
        "imagenes" => "NASA Image and Video Library",
        "epic" => "DSCOVR EPIC",
        "eonet" => "EONET",
        "donki" => "DONKI",
        "exoplanetas" => "NASA Exoplanet Archive",
        _ => "NASA Open APIs"
    };

    private static AsteroideVista ConvertirAsteroide(JsonElement elemento)
    {
        JsonElement aproximacion = elemento
            .GetProperty("close_approach_data")
            .EnumerateArray()
            .FirstOrDefault();
        JsonElement kilometros = elemento
            .GetProperty("estimated_diameter")
            .GetProperty("kilometers");

        DateTime? fecha = null;
        double velocidad = 0;
        double distancia = 0;

        if (aproximacion.ValueKind == JsonValueKind.Object)
        {
            if (aproximacion.TryGetProperty("close_approach_date", out JsonElement fechaJson)
                && DateTime.TryParse(fechaJson.GetString(), out DateTime fechaLeida))
            {
                fecha = fechaLeida;
            }

            velocidad = LeerDouble(
                aproximacion.GetProperty("relative_velocity"),
                "kilometers_per_hour");
            distancia = LeerDouble(
                aproximacion.GetProperty("miss_distance"),
                "kilometers");
        }

        return new AsteroideVista
        {
            Id = elemento.GetProperty("id").GetString() ?? string.Empty,
            Nombre = elemento.GetProperty("name").GetString() ?? "Sin nombre",
            Peligroso = elemento
                .GetProperty("is_potentially_hazardous_asteroid")
                .GetBoolean(),
            DiametroMinimoKm = kilometros
                .GetProperty("estimated_diameter_min")
                .GetDouble(),
            DiametroMaximoKm = kilometros
                .GetProperty("estimated_diameter_max")
                .GetDouble(),
            FechaAproximacion = fecha,
            VelocidadKmHora = velocidad,
            DistanciaKm = distancia,
            UrlNasa = elemento.GetProperty("nasa_jpl_url").GetString() ?? string.Empty
        };
    }

    private static double LeerDouble(JsonElement padre, string propiedad)
    {
        return padre.TryGetProperty(propiedad, out JsonElement valor)
            && double.TryParse(
                valor.GetString(),
                NumberStyles.Float,
                CultureInfo.InvariantCulture,
                out double numero)
            ? numero
            : 0;
    }

    private static DonkiEventoVista ConvertirDonki(JsonElement elemento, string tipo)
    {
        string? fechaTexto = ObtenerTexto(elemento, "startTime")
            ?? ObtenerTexto(elemento, "eventTime")
            ?? ObtenerTexto(elemento, "beginTime")
            ?? ObtenerTexto(elemento, "peakTime");
        DateTime? fecha = DateTime.TryParse(fechaTexto, out DateTime fechaLeida)
            ? fechaLeida
            : null;
        string id = ObtenerTexto(elemento, "activityID")
            ?? ObtenerTexto(elemento, "gstID")
            ?? ObtenerTexto(elemento, "flrID")
            ?? ObtenerTexto(elemento, "ipsID")
            ?? $"{tipo}-{fechaTexto ?? "sin-fecha"}";

        string instrumentos = string.Empty;
        if (elemento.TryGetProperty("instruments", out JsonElement listaInstrumentos)
            && listaInstrumentos.ValueKind == JsonValueKind.Array)
        {
            instrumentos = string.Join(
                ", ",
                listaInstrumentos.EnumerateArray()
                    .Select(instrumento => ObtenerTexto(instrumento, "displayName"))
                    .Where(nombre => !string.IsNullOrWhiteSpace(nombre)));
        }

        string titulo = tipo switch
        {
            "CME" => "Eyección de masa coronal",
            "GST" => "Tormenta geomagnética",
            "FLR" => $"Llamarada solar {ObtenerTexto(elemento, "classType")}".Trim(),
            "IPS" => $"Choque interplanetario · {ObtenerTexto(elemento, "location")}".Trim(),
            _ => "Suceso espacial"
        };

        return new DonkiEventoVista
        {
            Id = id,
            Tipo = tipo,
            Fecha = fecha,
            Titulo = titulo,
            Detalle = ObtenerTexto(elemento, "note"),
            Instrumentos = instrumentos,
            Url = ObtenerTexto(elemento, "link")
        };
    }

    private static string? ObtenerTexto(JsonElement elemento, string propiedad)
    {
        return elemento.TryGetProperty(propiedad, out JsonElement valor)
            && valor.ValueKind == JsonValueKind.String
            ? valor.GetString()
            : null;
    }
}

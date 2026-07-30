using System.Globalization;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Biblioteca.Configuracion;
using Biblioteca.DTOs;
using Biblioteca.Modelos;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Biblioteca.Servicios;

// Centraliza las peticiones, la caché y la conversión de datos externos.
public class OpenLibraryServicio : IOpenLibraryServicio
{
    private const string CamposBusqueda =
        "key,title,author_name,first_publish_year,cover_i,edition_count,"
        + "ratings_average,ratings_count,isbn,language,subject,"
        + "number_of_pages_median,ebook_access";

    // El limitador es compartido por todas las peticiones de la aplicación.
    private static readonly SemaphoreSlim Limitador = new(1, 1);
    private static DateTime ultimaPeticionUtc = DateTime.MinValue;

    private readonly HttpClient _cliente;
    private readonly IMemoryCache _cache;
    private readonly OpenLibraryOpciones _opciones;

    public OpenLibraryServicio(
        HttpClient cliente,
        IMemoryCache cache,
        IOptions<OpenLibraryOpciones> opciones)
    {
        _cliente = cliente;
        _cache = cache;
        _opciones = opciones.Value;

        _cliente.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));

        // Open Library solicita identificar la aplicación mediante User-Agent.
        string producto = new(
            _opciones.NombreAplicacion
                .Where(caracter =>
                    char.IsLetterOrDigit(caracter)
                    || caracter is '-' or '_' or '.')
                .ToArray());

        producto = string.IsNullOrWhiteSpace(producto)
            ? "BibliotecaRazor"
            : producto;

        string userAgent = _opciones.TieneContactoReal
            ? $"{producto}/1.0 ({_opciones.Contacto})"
            : $"{producto}/1.0";

        _cliente.DefaultRequestHeaders.UserAgent.ParseAdd(userAgent);
    }

    public Task<PaginaLibros> ObtenerListadoAsync(
        TipoListado tipo,
        int pagina = 1,
        CancellationToken cancellationToken = default)
    {
        (string consulta, string? orden) = tipo switch
        {
            TipoListado.MejorValorados =>
                ("language:spa", "rating"),
            TipoListado.Novedades =>
                ("language:spa", "new"),
            TipoListado.Fantasia =>
                ("subject:fantasy", null),
            TipoListado.Misterio =>
                ("subject:mystery", null),
            TipoListado.CienciaFiccion =>
                ("subject:science_fiction", null),
            TipoListado.Romance =>
                ("subject:romance", null),
            TipoListado.Programacion =>
                ("subject:programming", null),
            _ =>
                ("language:spa", "readinglog")
        };

        return BuscarInternoAsync(
            consulta,
            pagina,
            orden,
            $"listado:{tipo}",
            cancellationToken);
    }

    public Task<PaginaLibros> BuscarAsync(
        string texto,
        int pagina = 1,
        CancellationToken cancellationToken = default)
    {
        string consulta = texto.Trim();
        if (consulta.Length < 2)
        {
            return Task.FromResult(new PaginaLibros());
        }

        return BuscarInternoAsync(
            consulta,
            pagina,
            null,
            $"buscar:{consulta.ToLowerInvariant()}",
            cancellationToken);
    }

    public async Task<LibroDetalle> ObtenerDetalleAsync(
        string id,
        CancellationToken cancellationToken = default)
    {
        string obraId = NormalizarId(id);
        string claveCache = $"openlibrary:detalle:{obraId}";

        LibroDetalle? detalle =
            await _cache.GetOrCreateAsync(claveCache, async entrada =>
            {
                entrada.AbsoluteExpirationRelativeToNow =
                    TimeSpan.FromMinutes(
                        Math.Clamp(_opciones.MinutosCache * 2, 10, 240));

                ObraDto obra = await ObtenerJsonAsync<ObraDto>(
                    $"works/{obraId}.json", cancellationToken);

                LibroBusquedaDto? documento =
                    await BuscarObraAsync(obraId, cancellationToken);

                LibroResumen resumen = documento is null
                    ? CrearResumenBasico(obraId, obra)
                    : ConvertirResumen(documento);

                IReadOnlyList<LibroResumen> recomendaciones =
                    await ObtenerRecomendacionesAsync(
                        obraId, obra.Materias, cancellationToken);

                int? primeraPublicacion = resumen.PrimeraPublicacion
                    ?? ExtraerAnio(obra.PrimeraFechaPublicacion);

                return new LibroDetalle
                {
                    Id = obraId,
                    Titulo = string.IsNullOrWhiteSpace(resumen.Titulo)
                        ? obra.Titulo
                        : resumen.Titulo,
                    Autores = resumen.Autores,
                    PrimeraPublicacion = primeraPublicacion,
                    PortadaId = resumen.PortadaId
                        ?? (obra.Portadas.Count > 0
                            ? obra.Portadas[0]
                            : null),
                    NumeroEdiciones = resumen.NumeroEdiciones,
                    Puntuacion = resumen.Puntuacion,
                    NumeroValoraciones = resumen.NumeroValoraciones,
                    Isbn = resumen.Isbn,
                    Idiomas = resumen.Idiomas,
                    NumeroPaginas = resumen.NumeroPaginas,
                    AccesoElectronico = resumen.AccesoElectronico,
                    Descripcion = ExtraerDescripcion(obra.Descripcion),
                    PrimeraFechaPublicacion =
                        obra.PrimeraFechaPublicacion,
                    Materias = obra.Materias
                        .Take(20)
                        .ToList()
                        .AsReadOnly(),
                    Recomendaciones = recomendaciones
                };
            });

        return detalle
            ?? throw new OpenLibraryExcepcion(
                "No se ha podido preparar la ficha del libro.");
    }

    private async Task<PaginaLibros> BuscarInternoAsync(
        string consulta,
        int pagina,
        string? orden,
        string clave,
        CancellationToken cancellationToken)
    {
        pagina = Math.Max(1, pagina);
        int tamano = Math.Clamp(_opciones.TamanoPagina, 6, 50);
        string claveCache = $"openlibrary:{clave}:{orden}:{pagina}:{tamano}";

        PaginaLibros? resultado =
            await _cache.GetOrCreateAsync(claveCache, async entrada =>
            {
                entrada.AbsoluteExpirationRelativeToNow = DuracionCache();

                Dictionary<string, string?> parametros = new()
                {
                    ["q"] = consulta,
                    ["page"] = pagina.ToString(
                        CultureInfo.InvariantCulture),
                    ["limit"] = tamano.ToString(
                        CultureInfo.InvariantCulture),
                    ["lang"] = _opciones.Idioma,
                    ["fields"] = CamposBusqueda
                };

                if (!string.IsNullOrWhiteSpace(orden))
                {
                    parametros["sort"] = orden;
                }

                string ruta = QueryHelpers.AddQueryString(
                    "search.json", parametros);

                BusquedaLibrosDto dto =
                    await ObtenerJsonAsync<BusquedaLibrosDto>(
                        ruta, cancellationToken);

                int totalPaginas = dto.Total == 0
                    ? 0
                    : (int)Math.Ceiling(dto.Total / (double)tamano);

                return new PaginaLibros
                {
                    Pagina = pagina,
                    TotalPaginas = totalPaginas,
                    TotalResultados = dto.Total,
                    Resultados = dto.Documentos
                        // La API puede devolver la clave con o sin /works/.
                        .Where(documento => EsIdObraValido(documento.Clave))
                        .Select(ConvertirResumen)
                        .ToList()
                        .AsReadOnly()
                };
            });

        return resultado ?? new PaginaLibros();
    }

    private async Task<LibroBusquedaDto?> BuscarObraAsync(
        string obraId,
        CancellationToken cancellationToken)
    {
        string ruta = QueryHelpers.AddQueryString(
            "search.json",
            new Dictionary<string, string?>
            {
                ["q"] = $"key:/works/{obraId}",
                ["limit"] = "1",
                ["lang"] = _opciones.Idioma,
                ["fields"] = CamposBusqueda
            });

        BusquedaLibrosDto dto =
            await ObtenerJsonAsync<BusquedaLibrosDto>(
                ruta, cancellationToken);

        return dto.Documentos.FirstOrDefault();
    }

    private async Task<IReadOnlyList<LibroResumen>>
        ObtenerRecomendacionesAsync(
            string obraId,
            IReadOnlyList<string> materias,
            CancellationToken cancellationToken)
    {
        string? materia = materias.FirstOrDefault(
            valor => !string.IsNullOrWhiteSpace(valor));

        if (materia is null)
        {
            return [];
        }

        string materiaLimpia = materia.Replace("\"", "");
        string consulta = $"subject:\"{materiaLimpia}\"";
        PaginaLibros pagina = await BuscarInternoAsync(
            consulta,
            1,
            null,
            $"recomendaciones:{materia.ToLowerInvariant()}",
            cancellationToken);

        return pagina.Resultados
            .Where(libro =>
                !libro.Id.Equals(
                    obraId, StringComparison.OrdinalIgnoreCase))
            .Take(6)
            .ToList()
            .AsReadOnly();
    }

    private async Task<T> ObtenerJsonAsync<T>(
        string ruta,
        CancellationToken cancellationToken)
    {
        await Limitador.WaitAsync(cancellationToken);

        try
        {
            // Sin contacto se respeta el límite conservador de una petición/s.
            int milisegundos = _opciones.TieneContactoReal ? 400 : 1100;
            TimeSpan transcurrido = DateTime.UtcNow - ultimaPeticionUtc;
            TimeSpan espera = TimeSpan.FromMilliseconds(milisegundos)
                - transcurrido;

            if (espera > TimeSpan.Zero)
            {
                await Task.Delay(espera, cancellationToken);
            }

            using HttpResponseMessage respuesta =
                await _cliente.GetAsync(ruta, cancellationToken);

            ultimaPeticionUtc = DateTime.UtcNow;

            if (!respuesta.IsSuccessStatusCode)
            {
                throw CrearExcepcion(respuesta.StatusCode);
            }

            T? datos = await respuesta.Content.ReadFromJsonAsync<T>(
                cancellationToken: cancellationToken);

            return datos
                ?? throw new OpenLibraryExcepcion(
                    "Open Library ha devuelto una respuesta vacía.");
        }
        catch (OperationCanceledException)
            when (!cancellationToken.IsCancellationRequested)
        {
            throw new OpenLibraryExcepcion(
                "Open Library ha tardado demasiado en responder.");
        }
        catch (HttpRequestException)
        {
            throw new OpenLibraryExcepcion(
                "No se ha podido conectar con Open Library.");
        }
        catch (JsonException)
        {
            throw new OpenLibraryExcepcion(
                "Open Library ha devuelto datos con un formato inesperado.");
        }
        finally
        {
            Limitador.Release();
        }
    }

    private static LibroResumen ConvertirResumen(
        LibroBusquedaDto dto)
    {
        return new LibroResumen
        {
            Id = NormalizarId(dto.Clave),
            Titulo = dto.Titulo,
            Autores = dto.Autores.ToList().AsReadOnly(),
            PrimeraPublicacion = dto.PrimeraPublicacion,
            PortadaId = dto.PortadaId,
            NumeroEdiciones = dto.NumeroEdiciones,
            Puntuacion = dto.Puntuacion,
            NumeroValoraciones = dto.NumeroValoraciones,
            Isbn = dto.Isbn.Take(12).ToList().AsReadOnly(),
            Idiomas = dto.Idiomas.Take(12).ToList().AsReadOnly(),
            NumeroPaginas = dto.NumeroPaginas,
            AccesoElectronico = dto.AccesoElectronico
        };
    }

    private static LibroResumen CrearResumenBasico(
        string obraId,
        ObraDto obra)
    {
        return new LibroResumen
        {
            Id = obraId,
            Titulo = obra.Titulo,
            PrimeraPublicacion = ExtraerAnio(
                obra.PrimeraFechaPublicacion),
            PortadaId = obra.Portadas.Count > 0
                ? obra.Portadas[0]
                : null
        };
    }

    private static string ExtraerDescripcion(JsonElement elemento)
    {
        if (elemento.ValueKind == JsonValueKind.String)
        {
            return elemento.GetString() ?? "";
        }

        if (elemento.ValueKind == JsonValueKind.Object
            && elemento.TryGetProperty("value", out JsonElement valor)
            && valor.ValueKind == JsonValueKind.String)
        {
            return valor.GetString() ?? "";
        }

        return "";
    }

    private static int? ExtraerAnio(string? fecha)
    {
        if (string.IsNullOrWhiteSpace(fecha) || fecha.Length < 4)
        {
            return null;
        }

        return int.TryParse(
            fecha[..4],
            NumberStyles.None,
            CultureInfo.InvariantCulture,
            out int anio)
                ? anio
                : null;
    }

    private static string NormalizarId(string id)
    {
        string valor = id.Trim();

        if (valor.StartsWith("/works/", StringComparison.OrdinalIgnoreCase))
        {
            valor = valor[7..];
        }
        else if (valor.StartsWith("works/", StringComparison.OrdinalIgnoreCase))
        {
            valor = valor[6..];
        }

        valor = valor.ToUpperInvariant();

        bool valido = valor.Length is >= 4 and <= 30
            && valor.StartsWith("OL", StringComparison.Ordinal)
            && valor.EndsWith('W')
            && valor.All(caracter =>
                char.IsLetterOrDigit(caracter));

        return valido
            ? valor
            : throw new OpenLibraryExcepcion(
                "El identificador de la obra no es válido.",
                HttpStatusCode.BadRequest);
    }

    private static bool EsIdObraValido(string? id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return false;
        }

        try
        {
            _ = NormalizarId(id);
            return true;
        }
        catch (OpenLibraryExcepcion)
        {
            return false;
        }
    }

    private static OpenLibraryExcepcion CrearExcepcion(
        HttpStatusCode codigo)
    {
        return codigo switch
        {
            HttpStatusCode.NotFound =>
                new OpenLibraryExcepcion(
                    "Open Library no ha encontrado ese libro.", codigo),
            HttpStatusCode.TooManyRequests =>
                new OpenLibraryExcepcion(
                    "Open Library ha limitado temporalmente las peticiones.",
                    codigo),
            _ =>
                new OpenLibraryExcepcion(
                    $"Open Library ha respondido con el código {(int)codigo}.",
                    codigo)
        };
    }

    private TimeSpan DuracionCache()
    {
        int minutos = Math.Clamp(_opciones.MinutosCache, 1, 120);
        return TimeSpan.FromMinutes(minutos);
    }
}

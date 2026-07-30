using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Recetas.Configuracion;
using Recetas.DTOs;
using Recetas.Modelos;

namespace Recetas.Servicios;

// Centraliza peticiones, caché y conversión de datos de TheMealDB.
public class TheMealDbServicio : ITheMealDbServicio
{
    private readonly HttpClient _cliente;
    private readonly IMemoryCache _cache;
    private readonly TheMealDbOpciones _opciones;

    public TheMealDbServicio(
        HttpClient cliente,
        IMemoryCache cache,
        IOptions<TheMealDbOpciones> opciones)
    {
        _cliente = cliente;
        _cache = cache;
        _opciones = opciones.Value;
    }

    public async Task<IReadOnlyList<RecetaResumen>> BuscarAsync(
        string texto,
        CancellationToken cancellationToken = default)
    {
        string busqueda = texto.Trim();
        if (busqueda.Length < 2)
        {
            return [];
        }

        string ruta = QueryHelpers.AddQueryString(
            "search.php",
            "s",
            busqueda);

        return await ObtenerResumenesConCacheAsync(
            ruta,
            $"buscar:{busqueda.ToLowerInvariant()}",
            cancellationToken);
    }

    public async Task<RecetaDetalle> ObtenerDetalleAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        if (id <= 0)
        {
            throw new TheMealDbExcepcion(
                "El identificador de la receta no es válido.",
                HttpStatusCode.BadRequest);
        }

        string clave = $"themealdb:detalle:{id}";
        RecetaDetalle? detalle =
            await _cache.GetOrCreateAsync(clave, async entrada =>
            {
                entrada.AbsoluteExpirationRelativeToNow =
                    TimeSpan.FromMinutes(
                        Math.Clamp(_opciones.MinutosCache * 2, 10, 240));

                string ruta = QueryHelpers.AddQueryString(
                    "lookup.php",
                    "i",
                    id.ToString());

                RespuestaRecetasDto respuesta =
                    await ObtenerJsonAsync<RespuestaRecetasDto>(
                        ruta,
                        cancellationToken);

                RecetaDto? dto = respuesta.Recetas?.FirstOrDefault();
                return dto is null ? null : ConvertirDetalle(dto);
            });

        return detalle
            ?? throw new TheMealDbExcepcion(
                "TheMealDB no ha encontrado esa receta.",
                HttpStatusCode.NotFound);
    }

    public async Task<RecetaDetalle> ObtenerAleatoriaAsync(
        CancellationToken cancellationToken = default)
    {
        // La receta cambia en cada petición, por eso no se guarda en caché.
        RespuestaRecetasDto respuesta =
            await ObtenerJsonAsync<RespuestaRecetasDto>(
                "random.php",
                cancellationToken);

        RecetaDto? dto = respuesta.Recetas?.FirstOrDefault();
        return dto is null
            ? throw new TheMealDbExcepcion(
                "No se ha podido obtener una receta aleatoria.")
            : ConvertirDetalle(dto);
    }

    public Task<IReadOnlyList<RecetaResumen>> FiltrarCategoriaAsync(
        string categoria,
        CancellationToken cancellationToken = default)
    {
        string valor = categoria.Trim();
        string ruta = QueryHelpers.AddQueryString(
            "filter.php",
            "c",
            valor);

        return ObtenerResumenesConCacheAsync(
            ruta,
            $"categoria:{valor.ToLowerInvariant()}",
            cancellationToken);
    }

    public Task<IReadOnlyList<RecetaResumen>> FiltrarAreaAsync(
        string area,
        CancellationToken cancellationToken = default)
    {
        string valor = area.Trim();
        string ruta = QueryHelpers.AddQueryString(
            "filter.php",
            "a",
            valor);

        return ObtenerResumenesConCacheAsync(
            ruta,
            $"area:{valor.ToLowerInvariant()}",
            cancellationToken);
    }

    public async Task<IReadOnlyList<CategoriaReceta>>
        ObtenerCategoriasAsync(
            CancellationToken cancellationToken = default)
    {
        const string clave = "themealdb:categorias";

        IReadOnlyList<CategoriaReceta>? categorias =
            await _cache.GetOrCreateAsync(clave, async entrada =>
            {
                entrada.AbsoluteExpirationRelativeToNow = DuracionCache();

                RespuestaCategoriasDto respuesta =
                    await ObtenerJsonAsync<RespuestaCategoriasDto>(
                        "categories.php",
                        cancellationToken);

                return (IReadOnlyList<CategoriaReceta>)
                    (respuesta.Categorias ?? [])
                    .Select(categoria => new CategoriaReceta
                    {
                        Nombre = categoria.Nombre,
                        ImagenUrl = categoria.ImagenUrl,
                        Descripcion = categoria.Descripcion
                    })
                    .ToList()
                    .AsReadOnly();
            });

        return categorias ?? [];
    }

    public async Task<IReadOnlyList<string>> ObtenerAreasAsync(
        CancellationToken cancellationToken = default)
    {
        const string clave = "themealdb:areas";

        IReadOnlyList<string>? areas =
            await _cache.GetOrCreateAsync(clave, async entrada =>
            {
                entrada.AbsoluteExpirationRelativeToNow = DuracionCache();

                RespuestaAreasDto respuesta =
                    await ObtenerJsonAsync<RespuestaAreasDto>(
                        "list.php?a=list",
                        cancellationToken);

                return (IReadOnlyList<string>)(respuesta.Areas ?? [])
                    .Select(area => area.Nombre)
                    .Where(nombre => !string.IsNullOrWhiteSpace(nombre))
                    .OrderBy(nombre => nombre)
                    .ToList()
                    .AsReadOnly();
            });

        return areas ?? [];
    }

    private async Task<IReadOnlyList<RecetaResumen>>
        ObtenerResumenesConCacheAsync(
            string ruta,
            string parteClave,
            CancellationToken cancellationToken)
    {
        string clave = $"themealdb:{parteClave}";

        IReadOnlyList<RecetaResumen>? recetas =
            await _cache.GetOrCreateAsync(clave, async entrada =>
            {
                entrada.AbsoluteExpirationRelativeToNow = DuracionCache();

                RespuestaRecetasDto respuesta =
                    await ObtenerJsonAsync<RespuestaRecetasDto>(
                        ruta,
                        cancellationToken);

                return (IReadOnlyList<RecetaResumen>)
                    (respuesta.Recetas ?? [])
                    .Select(ConvertirResumen)
                    .Where(receta => receta.Id > 0)
                    .ToList()
                    .AsReadOnly();
            });

        return recetas ?? [];
    }

    private async Task<T> ObtenerJsonAsync<T>(
        string ruta,
        CancellationToken cancellationToken)
    {
        string rutaCompleta =
            $"{_opciones.ApiKeyValida}/{ruta}";

        try
        {
            using HttpResponseMessage respuesta =
                await _cliente.GetAsync(rutaCompleta, cancellationToken);

            if (!respuesta.IsSuccessStatusCode)
            {
                throw CrearExcepcion(respuesta.StatusCode);
            }

            T? datos = await respuesta.Content.ReadFromJsonAsync<T>(
                cancellationToken: cancellationToken);

            return datos
                ?? throw new TheMealDbExcepcion(
                    "TheMealDB ha devuelto una respuesta vacía.");
        }
        catch (OperationCanceledException)
            when (!cancellationToken.IsCancellationRequested)
        {
            throw new TheMealDbExcepcion(
                "TheMealDB ha tardado demasiado en responder.");
        }
        catch (HttpRequestException)
        {
            throw new TheMealDbExcepcion(
                "No se ha podido conectar con TheMealDB.");
        }
        catch (JsonException)
        {
            throw new TheMealDbExcepcion(
                "TheMealDB ha devuelto datos con un formato inesperado.");
        }
    }

    private static RecetaResumen ConvertirResumen(RecetaDto dto)
    {
        return new RecetaResumen
        {
            Id = int.TryParse(dto.Id, out int id) ? id : 0,
            Nombre = dto.Nombre,
            ImagenUrl = NormalizarUrl(dto.ImagenUrl),
            Categoria = dto.Categoria,
            Area = dto.Area
        };
    }

    private static RecetaDetalle ConvertirDetalle(RecetaDto dto)
    {
        RecetaResumen resumen = ConvertirResumen(dto);
        string youtube = NormalizarUrl(dto.UrlYoutube);

        return new RecetaDetalle
        {
            Id = resumen.Id,
            Nombre = resumen.Nombre,
            ImagenUrl = resumen.ImagenUrl,
            Categoria = resumen.Categoria,
            Area = resumen.Area,
            Instrucciones = dto.Instrucciones,
            Etiquetas = dto.Etiquetas ?? "",
            UrlYoutube = youtube,
            UrlYoutubeEmbed = CrearUrlYoutubeEmbed(youtube),
            Fuente = NormalizarUrl(dto.Fuente),
            Ingredientes = ExtraerIngredientes(dto)
        };
    }

    private static IReadOnlyList<IngredienteReceta> ExtraerIngredientes(
        RecetaDto dto)
    {
        List<IngredienteReceta> ingredientes = [];

        for (int indice = 1; indice <= 20; indice++)
        {
            string nombre = ObtenerTexto(
                dto.OtrosCampos,
                $"strIngredient{indice}");

            if (string.IsNullOrWhiteSpace(nombre))
            {
                continue;
            }

            string medida = ObtenerTexto(
                dto.OtrosCampos,
                $"strMeasure{indice}");

            ingredientes.Add(new IngredienteReceta
            {
                Nombre = nombre.Trim(),
                Medida = medida.Trim()
            });
        }

        return ingredientes.AsReadOnly();
    }

    private static string ObtenerTexto(
        Dictionary<string, JsonElement> campos,
        string nombre)
    {
        return campos.TryGetValue(nombre, out JsonElement elemento)
            && elemento.ValueKind == JsonValueKind.String
                ? elemento.GetString() ?? ""
                : "";
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

    private static string CrearUrlYoutubeEmbed(string url)
    {
        if (!Uri.TryCreate(url, UriKind.Absolute, out Uri? uri))
        {
            return "";
        }

        string id = "";
        if (uri.Host.Contains("youtu.be", StringComparison.OrdinalIgnoreCase))
        {
            id = uri.AbsolutePath.Trim('/');
        }
        else if (uri.Host.Contains(
            "youtube.com",
            StringComparison.OrdinalIgnoreCase))
        {
            id = QueryHelpers.ParseQuery(uri.Query)["v"].ToString();
        }

        bool idValido = id.Length is >= 6 and <= 20
            && id.All(caracter =>
                char.IsLetterOrDigit(caracter)
                || caracter is '-' or '_');

        return idValido
            ? $"https://www.youtube-nocookie.com/embed/{id}"
            : "";
    }

    private static TheMealDbExcepcion CrearExcepcion(HttpStatusCode codigo)
    {
        return codigo switch
        {
            HttpStatusCode.NotFound =>
                new TheMealDbExcepcion(
                    "TheMealDB no ha encontrado ese recurso.",
                    codigo),
            HttpStatusCode.TooManyRequests =>
                new TheMealDbExcepcion(
                    "TheMealDB ha limitado temporalmente las peticiones.",
                    codigo),
            _ =>
                new TheMealDbExcepcion(
                    $"TheMealDB ha respondido con el código {(int)codigo}.",
                    codigo)
        };
    }

    private TimeSpan DuracionCache()
    {
        int minutos = Math.Clamp(_opciones.MinutosCache, 1, 120);
        return TimeSpan.FromMinutes(minutos);
    }
}

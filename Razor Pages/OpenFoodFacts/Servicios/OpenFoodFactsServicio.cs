using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using OpenFoodFacts.Configuracion;
using OpenFoodFacts.DTOs;
using OpenFoodFacts.Modelos;

namespace OpenFoodFacts.Servicios;

// Centraliza las peticiones, la caché y la conversión de datos de la API.
public class OpenFoodFactsServicio : IOpenFoodFactsServicio
{
    private const string CamposResumen =
        "code,product_name,brands,quantity,image_front_small_url,"
        + "image_front_url,nutrition_grades,nutriscore_grade,"
        + "nova_group,ecoscore_grade,environmental_score_grade";

    private const string CamposDetalle =
        CamposResumen
        + ",generic_name,ingredients_text,allergens,traces,categories,"
        + "countries,labels,packaging,serving_size,additives_n,"
        + "additives_tags,nutriments";

    private readonly HttpClient _cliente;
    private readonly IMemoryCache _cache;
    private readonly OpenFoodFactsOpciones _opciones;

    public OpenFoodFactsServicio(
        HttpClient cliente,
        IMemoryCache cache,
        IOptions<OpenFoodFactsOpciones> opciones)
    {
        _cliente = cliente;
        _cache = cache;
        _opciones = opciones.Value;
    }

    public Task<ResultadoProductos> ObtenerDestacadosAsync(
        int pagina = 1,
        CancellationToken cancellationToken = default)
    {
        Dictionary<string, string?> parametros = ParametrosListado(pagina);
        parametros["countries_tags_en"] = "Spain";
        parametros["sort_by"] = "unique_scans_n";

        return ObtenerBusquedaConCacheAsync(
            "api/v2/search",
            parametros,
            $"destacados:{PaginaValida(pagina)}",
            cancellationToken);
    }

    public Task<ResultadoProductos> BuscarAsync(
        string texto,
        int pagina = 1,
        CancellationToken cancellationToken = default)
    {
        string busqueda = texto.Trim();
        if (busqueda.Length < 2)
        {
            return Task.FromResult(new ResultadoProductos());
        }

        // La búsqueda de texto completo todavía se ofrece en este endpoint.
        Dictionary<string, string?> parametros = ParametrosListado(pagina);
        parametros["search_terms"] = busqueda;
        parametros["search_simple"] = "1";
        parametros["action"] = "process";
        parametros["json"] = "1";
        parametros["lc"] = "es";

        return ObtenerBusquedaConCacheAsync(
            "cgi/search.pl",
            parametros,
            $"buscar:{busqueda.ToLowerInvariant()}:{PaginaValida(pagina)}",
            cancellationToken);
    }

    public Task<ResultadoProductos> FiltrarCategoriaAsync(
        string categoria,
        int pagina = 1,
        CancellationToken cancellationToken = default)
    {
        string filtro = categoria.Trim();
        Dictionary<string, string?> parametros = ParametrosListado(pagina);
        parametros["categories_tags_en"] = filtro;
        parametros["countries_tags_en"] = "Spain";
        parametros["sort_by"] = "unique_scans_n";

        return ObtenerBusquedaConCacheAsync(
            "api/v2/search",
            parametros,
            $"categoria:{filtro.ToLowerInvariant()}:{PaginaValida(pagina)}",
            cancellationToken);
    }

    public Task<ResultadoProductos> FiltrarNutriScoreAsync(
        string puntuacion,
        int pagina = 1,
        CancellationToken cancellationToken = default)
    {
        string nota = puntuacion.Trim().ToLowerInvariant();
        if (nota is not ("a" or "b" or "c" or "d" or "e"))
        {
            return Task.FromResult(new ResultadoProductos());
        }

        Dictionary<string, string?> parametros = ParametrosListado(pagina);
        parametros["nutrition_grades_tags"] = nota;
        parametros["countries_tags_en"] = "Spain";
        parametros["sort_by"] = "unique_scans_n";

        return ObtenerBusquedaConCacheAsync(
            "api/v2/search",
            parametros,
            $"nutriscore:{nota}:{PaginaValida(pagina)}",
            cancellationToken);
    }

    public async Task<ProductoDetalle> ObtenerProductoAsync(
        string codigo,
        CancellationToken cancellationToken = default)
    {
        string codigoLimpio = NormalizarCodigo(codigo);
        string clave = $"off:producto:{codigoLimpio}";

        ProductoDetalle? producto =
            await _cache.GetOrCreateAsync(clave, async entrada =>
            {
                entrada.AbsoluteExpirationRelativeToNow =
                    TimeSpan.FromMinutes(
                        Math.Clamp(_opciones.MinutosCache * 2, 10, 240));

                string ruta = QueryHelpers.AddQueryString(
                    $"api/v3.6/product/{codigoLimpio}.json",
                    "fields",
                    CamposDetalle);

                RespuestaProductoDto respuesta =
                    await ObtenerJsonAsync<RespuestaProductoDto>(
                        ruta,
                        cancellationToken);

                if (respuesta.Producto is null)
                {
                    return null;
                }

                if (string.IsNullOrWhiteSpace(respuesta.Producto.Codigo))
                {
                    respuesta.Producto.Codigo = respuesta.Codigo;
                }

                return ConvertirDetalle(respuesta.Producto);
            });

        return producto
            ?? throw new OpenFoodFactsExcepcion(
                "Open Food Facts no ha encontrado ese producto.",
                HttpStatusCode.NotFound);
    }

    private async Task<ResultadoProductos> ObtenerBusquedaConCacheAsync(
        string endpoint,
        Dictionary<string, string?> parametros,
        string parteClave,
        CancellationToken cancellationToken)
    {
        string clave = $"off:{parteClave}";

        ResultadoProductos? resultado =
            await _cache.GetOrCreateAsync(clave, async entrada =>
            {
                entrada.AbsoluteExpirationRelativeToNow = DuracionCache();

                string ruta = QueryHelpers.AddQueryString(
                    endpoint,
                    parametros);

                RespuestaBusquedaDto respuesta =
                    await ObtenerJsonAsync<RespuestaBusquedaDto>(
                        ruta,
                        cancellationToken);

                List<ProductoResumen> productos =
                    (respuesta.Productos ?? [])
                    .Select(ConvertirResumen)
                    .Where(producto =>
                        !string.IsNullOrWhiteSpace(producto.Codigo))
                    .ToList();

                return new ResultadoProductos
                {
                    Productos = productos.AsReadOnly(),
                    Total = respuesta.Total,
                    Pagina = respuesta.Pagina > 0
                        ? respuesta.Pagina
                        : PaginaValida(parametros["page"]),
                    TamanoPagina = respuesta.TamanoPagina > 0
                        ? respuesta.TamanoPagina
                        : _opciones.TamanoPaginaValido
                };
            });

        return resultado ?? new ResultadoProductos();
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
                ?? throw new OpenFoodFactsExcepcion(
                    "Open Food Facts ha devuelto una respuesta vacía.");
        }
        catch (OperationCanceledException)
            when (!cancellationToken.IsCancellationRequested)
        {
            throw new OpenFoodFactsExcepcion(
                "Open Food Facts ha tardado demasiado en responder.");
        }
        catch (HttpRequestException)
        {
            throw new OpenFoodFactsExcepcion(
                "No se ha podido conectar con Open Food Facts.");
        }
        catch (JsonException)
        {
            throw new OpenFoodFactsExcepcion(
                "Open Food Facts ha devuelto datos con un formato inesperado.");
        }
    }

    private static ProductoResumen ConvertirResumen(ProductoDto dto)
    {
        string nombre = string.IsNullOrWhiteSpace(dto.Nombre)
            ? "Producto sin nombre"
            : dto.Nombre.Trim();

        return new ProductoResumen
        {
            Codigo = dto.Codigo.Trim(),
            Nombre = nombre,
            Marca = dto.Marcas?.Trim() ?? "",
            Cantidad = dto.Cantidad?.Trim() ?? "",
            ImagenUrl = NormalizarUrl(
                dto.ImagenPequenaUrl ?? dto.ImagenUrl),
            NutriScore = NormalizarPuntuacion(
                dto.NutriScoreActual ?? dto.NutriScore),
            GrupoNova = ObtenerEntero(dto.GrupoNova),
            GreenScore = NormalizarPuntuacion(
                dto.GreenScore ?? dto.EcoScore)
        };
    }

    private static ProductoDetalle ConvertirDetalle(ProductoDto dto)
    {
        ProductoResumen resumen = ConvertirResumen(dto);
        NutrimentosDto nutrientes = dto.Nutrimentos ?? new();

        return new ProductoDetalle
        {
            Codigo = resumen.Codigo,
            Nombre = resumen.Nombre,
            Marca = resumen.Marca,
            Cantidad = resumen.Cantidad,
            ImagenUrl = NormalizarUrl(dto.ImagenUrl)
                ?? resumen.ImagenUrl,
            NutriScore = resumen.NutriScore,
            GrupoNova = resumen.GrupoNova,
            GreenScore = resumen.GreenScore,
            NombreGenerico = dto.NombreGenerico?.Trim() ?? "",
            Ingredientes = dto.Ingredientes?.Trim() ?? "",
            Alergenos = LimpiarEtiquetas(dto.Alergenos),
            Trazas = LimpiarEtiquetas(dto.Trazas),
            Categorias = dto.Categorias?.Trim() ?? "",
            Paises = dto.Paises?.Trim() ?? "",
            Etiquetas = dto.Etiquetas?.Trim() ?? "",
            Envase = dto.Envase?.Trim() ?? "",
            TamanoRacion = dto.TamanoRacion?.Trim() ?? "",
            NumeroAditivos = dto.NumeroAditivos,
            Aditivos = (dto.Aditivos ?? [])
                .Select(LimpiarEtiqueta)
                .Where(valor => valor.Length > 0)
                .ToList()
                .AsReadOnly(),
            Nutrientes = CrearNutrientes(nutrientes)
        };
    }

    private static IReadOnlyList<NutrienteProducto> CrearNutrientes(
        NutrimentosDto dto)
    {
        return new List<NutrienteProducto>
        {
            CrearNutriente("Energía", dto.EnergiaKcal100g, "kcal"),
            CrearNutriente("Grasas", dto.Grasas100g),
            CrearNutriente(
                "Grasas saturadas",
                dto.GrasasSaturadas100g),
            CrearNutriente("Hidratos de carbono", dto.Hidratos100g),
            CrearNutriente("Azúcares", dto.Azucares100g),
            CrearNutriente("Fibra", dto.Fibra100g),
            CrearNutriente("Proteínas", dto.Proteinas100g),
            CrearNutriente("Sal", dto.Sal100g),
            CrearNutriente("Sodio", dto.Sodio100g)
        }.AsReadOnly();
    }

    private static NutrienteProducto CrearNutriente(
        string nombre,
        double? cantidad,
        string unidad = "g")
    {
        return new NutrienteProducto
        {
            Nombre = nombre,
            Cantidad = cantidad,
            Unidad = unidad
        };
    }

    private Dictionary<string, string?> ParametrosListado(int pagina)
    {
        return new Dictionary<string, string?>
        {
            ["page"] = PaginaValida(pagina).ToString(),
            ["page_size"] = _opciones.TamanoPaginaValido.ToString(),
            ["fields"] = CamposResumen
        };
    }

    private static int PaginaValida(int pagina) =>
        Math.Clamp(pagina, 1, 1000);

    private static int PaginaValida(string? pagina) =>
        int.TryParse(pagina, out int valor)
            ? PaginaValida(valor)
            : 1;

    private static string NormalizarCodigo(string codigo)
    {
        string valor = codigo.Trim();
        bool valido = valor.Length is >= 4 and <= 24
            && valor.All(char.IsDigit);

        return valido
            ? valor
            : throw new OpenFoodFactsExcepcion(
                "El código de barras debe contener entre 4 y 24 números.",
                HttpStatusCode.BadRequest);
    }

    private static int? ObtenerEntero(JsonElement elemento)
    {
        if (elemento.ValueKind == JsonValueKind.Number
            && elemento.TryGetInt32(out int numero))
        {
            return numero;
        }

        if (elemento.ValueKind == JsonValueKind.String
            && int.TryParse(elemento.GetString(), out numero))
        {
            return numero;
        }

        return null;
    }

    private static string NormalizarPuntuacion(string? valor)
    {
        string puntuacion = valor?.Trim().ToLowerInvariant() ?? "";
        return puntuacion is "a" or "b" or "c" or "d" or "e"
            ? puntuacion
            : "";
    }

    private static string? NormalizarUrl(string? url)
    {
        if (!Uri.TryCreate(url, UriKind.Absolute, out Uri? resultado))
        {
            return null;
        }

        return resultado.Scheme is "http" or "https"
            ? resultado.ToString()
            : null;
    }

    private static string LimpiarEtiquetas(string? texto)
    {
        return string.Join(
            ", ",
            (texto ?? "")
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(LimpiarEtiqueta)
                .Where(valor => valor.Length > 0));
    }

    private static string LimpiarEtiqueta(string texto)
    {
        string valor = texto.Trim();
        int separadorIdioma = valor.IndexOf(':');
        if (separadorIdioma >= 0)
        {
            valor = valor[(separadorIdioma + 1)..];
        }

        return valor.Replace('-', ' ');
    }

    private static OpenFoodFactsExcepcion CrearExcepcion(
        HttpStatusCode codigo)
    {
        return codigo switch
        {
            HttpStatusCode.NotFound =>
                new OpenFoodFactsExcepcion(
                    "Open Food Facts no ha encontrado ese producto.",
                    codigo),
            HttpStatusCode.TooManyRequests =>
                new OpenFoodFactsExcepcion(
                    "Se ha alcanzado temporalmente el límite de consultas. "
                    + "Espera un minuto antes de volver a intentarlo.",
                    codigo),
            HttpStatusCode.ServiceUnavailable =>
                new OpenFoodFactsExcepcion(
                    "Open Food Facts está saturado temporalmente.",
                    codigo),
            _ =>
                new OpenFoodFactsExcepcion(
                    $"Open Food Facts ha respondido con el código {(int)codigo}.",
                    codigo)
        };
    }

    private TimeSpan DuracionCache()
    {
        int minutos = Math.Clamp(_opciones.MinutosCache, 1, 180);
        return TimeSpan.FromMinutes(minutos);
    }
}

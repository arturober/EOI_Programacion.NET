using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using PokeApiRazor.Models;

namespace PokeApiRazor.Services;

// Esta clase concentra todas las peticiones a PokeAPI.
// Así, las páginas Razor no necesitan saber cómo se descarga o transforma el JSON.
public class PokeApiService
{
    // Clave utilizada para guardar la lista completa en la memoria del servidor.
    private const string ClaveListaCompleta = "pokeapi-lista-completa";

    // HttpClient realiza las peticiones y la caché evita repeticiones innecesarias.
    private readonly HttpClient _cliente;
    private readonly IMemoryCache _cache;

    // Estas opciones permiten convertir el JSON en objetos de C#.
    private static readonly JsonSerializerOptions OpcionesJson = new()
    {
        PropertyNameCaseInsensitive = true
    };

    // Recibimos las dependencias mediante inyección de dependencias.
    public PokeApiService(HttpClient cliente, IMemoryCache cache)
    {
        _cliente = cliente;
        _cache = cache;
    }

    // Obtiene una página del listado y aplica una búsqueda parcial por nombre.
    public async Task<ResultadoPokemon> ObtenerListadoAsync(
        string? busqueda,
        int pagina,
        int elementosPorPagina = 24)
    {
        // Descargamos la lista completa una vez porque contiene únicamente nombre y URL.
        ListaPokemonApi listaCompleta =
            await ObtenerListaCompletaAsync();

        IEnumerable<RecursoApi> consulta = listaCompleta.Resultados;

        // Si hay texto, conservamos los nombres que lo contienen.
        if (!string.IsNullOrWhiteSpace(busqueda))
        {
            string textoBuscado = busqueda.Trim();
            consulta = consulta.Where(pokemon =>
                pokemon.Nombre.Contains(
                    textoBuscado,
                    StringComparison.OrdinalIgnoreCase));
        }

        // Materializamos la consulta para no recorrerla varias veces.
        List<RecursoApi> encontrados = consulta.ToList();
        int totalPaginas = Math.Max(
            1,
            (int)Math.Ceiling(encontrados.Count / (double)elementosPorPagina));

        // Corregimos una página inexistente, por ejemplo después de una búsqueda.
        int paginaSegura = Math.Clamp(pagina, 1, totalPaginas);

        // Skip omite páginas anteriores y Take se queda con la página actual.
        List<PokemonResumen> pokemons = encontrados
            .Skip((paginaSegura - 1) * elementosPorPagina)
            .Take(elementosPorPagina)
            .Select(CrearResumen)
            .ToList();

        return new ResultadoPokemon
        {
            Pokemons = pokemons,
            Pagina = paginaSegura,
            TotalPaginas = totalPaginas,
            TotalResultados = encontrados.Count
        };
    }

    // Obtiene el detalle combinando varios recursos de PokeAPI.
    public async Task<PokemonDetalle?> ObtenerDetalleAsync(string nombreOId)
    {
        string identificador = nombreOId.Trim().ToLowerInvariant();

        // La primera petición contiene los datos básicos, imágenes y sonidos.
        using HttpResponseMessage respuestaPokemon =
            await _cliente.GetAsync($"pokemon/{Uri.EscapeDataString(identificador)}");

        // Un 404 significa simplemente que ese Pokémon no existe.
        if (respuestaPokemon.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        // Cualquier otro error se enviará a la página para mostrar un aviso.
        respuestaPokemon.EnsureSuccessStatusCode();
        string jsonPokemon = await respuestaPokemon.Content.ReadAsStringAsync();

        PokemonApi pokemon =
            JsonSerializer.Deserialize<PokemonApi>(jsonPokemon, OpcionesJson)
            ?? throw new JsonException("PokeAPI devolvió un Pokémon vacío.");

        // La especie aporta textos en español y datos biológicos.
        EspecieApi especie =
            await _cliente.GetFromJsonAsync<EspecieApi>(pokemon.Especie.Url)
            ?? throw new JsonException("PokeAPI devolvió una especie vacía.");

        // El endpoint de encuentros devuelve una lista, que a veces está vacía.
        List<EncuentroApi> encuentros =
            await _cliente.GetFromJsonAsync<List<EncuentroApi>>(
                pokemon.UrlEncuentros)
            ?? new List<EncuentroApi>();

        // La cadena evolutiva tiene forma de árbol, por eso la leemos como JSON.
        List<string> cadenaEvolucion =
            await ObtenerCadenaEvolucionAsync(especie.CadenaEvolucion.Url);

        // Conservamos también el JSON original, bien indentado, con todos sus campos.
        using JsonDocument documentoPokemon = JsonDocument.Parse(jsonPokemon);
        string jsonFormateado = JsonSerializer.Serialize(
            documentoPokemon.RootElement,
            new JsonSerializerOptions { WriteIndented = true });

        // Transformamos las respuestas de la API en un modelo cómodo para la vista.
        return new PokemonDetalle
        {
            Id = pokemon.Id,
            Nombre = pokemon.Nombre,
            NombreEspanol = ObtenerNombreEspanol(especie, pokemon.Nombre),
            ExperienciaBase = pokemon.ExperienciaBase,
            AlturaMetros = pokemon.Altura / 10m,
            PesoKilos = pokemon.Peso / 10m,
            Orden = pokemon.Orden,
            EsVariedadPrincipal = pokemon.EsPrincipal,
            Genero = ObtenerGenero(especie),
            Descripcion = ObtenerDescripcionEspanola(especie),
            Generacion = TextoPokemon.Formatear(especie.Generacion.Nombre),
            Habitat = TextoPokemon.Formatear(especie.Habitat?.Nombre),
            Color = TextoPokemon.Formatear(especie.Color.Nombre),
            Forma = TextoPokemon.Formatear(especie.Forma.Nombre),
            Crecimiento = TextoPokemon.Formatear(especie.Crecimiento.Nombre),
            RatioCaptura = especie.RatioCaptura,
            FelicidadBase = especie.FelicidadBase,
            PasosEclosionAproximados = (especie.ContadorEclosion + 1) * 255,
            DistribucionSexo = ObtenerDistribucionSexo(especie.RatioSexo),
            EsBebe = especie.EsBebe,
            EsLegendario = especie.EsLegendario,
            EsMitico = especie.EsMitico,
            TieneDiferenciasDeSexo = especie.TieneDiferenciasDeSexo,
            Tipos = pokemon.Tipos
                .Select(tipo => TraducirTipo(tipo.Tipo.Nombre))
                .ToList(),
            Habilidades = pokemon.Habilidades
                .Select(habilidad => new HabilidadPokemon
                {
                    Nombre = TextoPokemon.Formatear(habilidad.Habilidad.Nombre),
                    EsOculta = habilidad.EsOculta
                })
                .ToList(),
            Estadisticas = pokemon.Estadisticas
                .Select(estadistica => new EstadisticaPokemon
                {
                    Nombre = TraducirEstadistica(
                        estadistica.Estadistica.Nombre),
                    Valor = estadistica.Valor,
                    Esfuerzo = estadistica.Esfuerzo
                })
                .ToList(),
            Movimientos = pokemon.Movimientos
                .Select(movimiento =>
                    TextoPokemon.Formatear(movimiento.Movimiento.Nombre))
                .Distinct()
                .OrderBy(nombre => nombre)
                .ToList(),
            Formas = pokemon.Formas
                .Select(forma => TextoPokemon.Formatear(forma.Nombre))
                .ToList(),
            Versiones = pokemon.Juegos
                .Select(juego => TextoPokemon.Formatear(juego.Version.Nombre))
                .Distinct()
                .ToList(),
            GruposHuevo = especie.GruposHuevo
                .Select(grupo => TextoPokemon.Formatear(grupo.Nombre))
                .ToList(),
            Variedades = especie.Variedades
                .Select(variedad =>
                    TextoPokemon.Formatear(variedad.Pokemon.Nombre))
                .ToList(),
            CadenaEvolucion = cadenaEvolucion,
            Objetos = CrearObjetos(pokemon.Objetos),
            Encuentros = CrearEncuentros(encuentros),
            Imagenes = ExtraerImagenes(pokemon.Sprites),
            SonidoActual = pokemon.Sonidos.Actual,
            SonidoClasico = pokemon.Sonidos.Clasico,
            JsonOriginal = jsonFormateado
        };
    }

    // Obtiene la lista completa y la guarda durante treinta minutos.
    private async Task<ListaPokemonApi> ObtenerListaCompletaAsync()
    {
        ListaPokemonApi? lista = await _cache.GetOrCreateAsync(
            ClaveListaCompleta,
            async entrada =>
            {
                entrada.AbsoluteExpirationRelativeToNow =
                    TimeSpan.FromMinutes(30);

                // Un límite amplio permite incluir también las distintas variedades.
                return await _cliente.GetFromJsonAsync<ListaPokemonApi>(
                    "pokemon?limit=100000&offset=0");
            });

        return lista ?? new ListaPokemonApi();
    }

    // Crea la tarjeta de listado y obtiene el ID a partir de la URL del recurso.
    private static PokemonResumen CrearResumen(RecursoApi recurso)
    {
        string ultimoFragmento = recurso.Url
            .TrimEnd('/')
            .Split('/')
            .Last();

        int.TryParse(ultimoFragmento, out int id);

        return new PokemonResumen
        {
            Id = id,
            Nombre = recurso.Nombre,
            Imagen =
                "https://raw.githubusercontent.com/PokeAPI/sprites/" +
                $"master/sprites/pokemon/other/official-artwork/{id}.png"
        };
    }

    // Busca el nombre oficial en español y usa el inglés como alternativa.
    private static string ObtenerNombreEspanol(
        EspecieApi especie,
        string nombreAlternativo)
    {
        return especie.Nombres
            .FirstOrDefault(nombre => nombre.Idioma.Nombre == "es")
            ?.Nombre
            ?? TextoPokemon.Formatear(nombreAlternativo);
    }

    // Busca el género o categoría, por ejemplo "Pokémon Ratón".
    private static string ObtenerGenero(EspecieApi especie)
    {
        return especie.Generos
            .FirstOrDefault(genero => genero.Idioma.Nombre == "es")
            ?.Nombre
            ?? "No disponible";
    }

    // Limpia los saltos especiales que aparecen en algunos textos de los juegos.
    private static string ObtenerDescripcionEspanola(EspecieApi especie)
    {
        string? descripcion = especie.Descripciones
            .FirstOrDefault(texto => texto.Idioma.Nombre == "es")
            ?.Texto;

        if (string.IsNullOrWhiteSpace(descripcion))
        {
            return "PokeAPI no ofrece una descripción en español.";
        }

        return descripcion
            .Replace('\n', ' ')
            .Replace('\r', ' ')
            .Replace('\f', ' ');
    }

    // Convierte el ratio de octavos de PokeAPI en porcentajes fáciles de leer.
    private static string ObtenerDistribucionSexo(int ratioSexo)
    {
        if (ratioSexo < 0)
        {
            return "Sin sexo";
        }

        decimal porcentajeHembra = ratioSexo * 12.5m;
        decimal porcentajeMacho = 100m - porcentajeHembra;
        return $"{porcentajeMacho:0.#}% macho · {porcentajeHembra:0.#}% hembra";
    }

    // Crea una lista más sencilla con los objetos y sus versiones.
    private static List<ObjetoPokemon> CrearObjetos(
        IEnumerable<ObjetoApi> objetos)
    {
        return objetos.Select(objeto => new ObjetoPokemon
        {
            Nombre = TextoPokemon.Formatear(objeto.Objeto.Nombre),
            Versiones = objeto.Versiones
                .Select(version =>
                    TextoPokemon.Formatear(version.Version.Nombre))
                .Distinct()
                .ToList()
        }).ToList();
    }

    // Agrupa zonas repetidas y conserva la probabilidad más alta de cada una.
    private static List<EncuentroPokemon> CrearEncuentros(
        IEnumerable<EncuentroApi> encuentros)
    {
        return encuentros
            .GroupBy(encuentro => encuentro.Zona.Nombre)
            .Select(grupo => new EncuentroPokemon
            {
                Zona = TextoPokemon.Formatear(grupo.Key),
                ProbabilidadMaxima = grupo
                    .SelectMany(encuentro => encuentro.Versiones)
                    .Select(version => version.ProbabilidadMaxima)
                    .DefaultIfEmpty(0)
                    .Max()
            })
            .OrderBy(encuentro => encuentro.Zona)
            .ToList();
    }

    // Recorre de forma recursiva todo el objeto "sprites".
    private static List<ImagenPokemon> ExtraerImagenes(JsonElement sprites)
    {
        List<ImagenPokemon> imagenes = new();
        HashSet<string> direccionesEncontradas =
            new(StringComparer.OrdinalIgnoreCase);

        RecorrerSprites(
            sprites,
            "Sprite",
            imagenes,
            direccionesEncontradas);

        return imagenes;
    }

    // Cada propiedad puede ser otra colección de propiedades o una URL.
    private static void RecorrerSprites(
        JsonElement elemento,
        string ruta,
        List<ImagenPokemon> imagenes,
        HashSet<string> direccionesEncontradas)
    {
        if (elemento.ValueKind == JsonValueKind.Object)
        {
            foreach (JsonProperty propiedad in elemento.EnumerateObject())
            {
                RecorrerSprites(
                    propiedad.Value,
                    $"{ruta} · {TextoPokemon.Formatear(propiedad.Name)}",
                    imagenes,
                    direccionesEncontradas);
            }

            return;
        }

        if (elemento.ValueKind != JsonValueKind.String)
        {
            return;
        }

        string? url = elemento.GetString();

        // Solo añadimos direcciones de imágenes y eliminamos duplicados.
        if (!EsUrlDeImagen(url) || !direccionesEncontradas.Add(url!))
        {
            return;
        }

        imagenes.Add(new ImagenPokemon
        {
            Descripcion = ruta,
            Url = url!
        });
    }

    // Comprueba las extensiones utilizadas en el repositorio de sprites.
    private static bool EsUrlDeImagen(string? texto)
    {
        if (!Uri.TryCreate(texto, UriKind.Absolute, out Uri? url))
        {
            return false;
        }

        string ruta = url.AbsolutePath;
        return ruta.EndsWith(".png", StringComparison.OrdinalIgnoreCase)
            || ruta.EndsWith(".gif", StringComparison.OrdinalIgnoreCase)
            || ruta.EndsWith(".svg", StringComparison.OrdinalIgnoreCase)
            || ruta.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase)
            || ruta.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase);
    }

    // Descarga la cadena evolutiva y aplana su estructura de árbol.
    private async Task<List<string>> ObtenerCadenaEvolucionAsync(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            return new List<string>();
        }

        string json = await _cliente.GetStringAsync(url);
        using JsonDocument documento = JsonDocument.Parse(json);
        List<string> nombres = new();

        if (documento.RootElement.TryGetProperty(
            "chain",
            out JsonElement cadena))
        {
            RecorrerCadenaEvolucion(cadena, nombres);
        }

        return nombres;
    }

    // Añade la especie actual y después visita todas sus posibles evoluciones.
    private static void RecorrerCadenaEvolucion(
        JsonElement eslabon,
        List<string> nombres)
    {
        if (eslabon.TryGetProperty("species", out JsonElement especie)
            && especie.TryGetProperty("name", out JsonElement nombre))
        {
            string? texto = nombre.GetString();

            if (!string.IsNullOrWhiteSpace(texto))
            {
                nombres.Add(TextoPokemon.Formatear(texto));
            }
        }

        if (!eslabon.TryGetProperty(
            "evolves_to",
            out JsonElement evoluciones))
        {
            return;
        }

        foreach (JsonElement evolucion in evoluciones.EnumerateArray())
        {
            RecorrerCadenaEvolucion(evolucion, nombres);
        }
    }

    // Traduce los nombres de tipos más habituales.
    private static string TraducirTipo(string tipo)
    {
        Dictionary<string, string> traducciones = new()
        {
            ["normal"] = "Normal",
            ["fire"] = "Fuego",
            ["water"] = "Agua",
            ["electric"] = "Eléctrico",
            ["grass"] = "Planta",
            ["ice"] = "Hielo",
            ["fighting"] = "Lucha",
            ["poison"] = "Veneno",
            ["ground"] = "Tierra",
            ["flying"] = "Volador",
            ["psychic"] = "Psíquico",
            ["bug"] = "Bicho",
            ["rock"] = "Roca",
            ["ghost"] = "Fantasma",
            ["dragon"] = "Dragón",
            ["dark"] = "Siniestro",
            ["steel"] = "Acero",
            ["fairy"] = "Hada",
            ["stellar"] = "Estelar",
            ["unknown"] = "Desconocido"
        };

        return traducciones.GetValueOrDefault(
            tipo,
            TextoPokemon.Formatear(tipo));
    }

    // Traduce las seis estadísticas principales.
    private static string TraducirEstadistica(string estadistica)
    {
        Dictionary<string, string> traducciones = new()
        {
            ["hp"] = "Puntos de salud",
            ["attack"] = "Ataque",
            ["defense"] = "Defensa",
            ["special-attack"] = "Ataque especial",
            ["special-defense"] = "Defensa especial",
            ["speed"] = "Velocidad"
        };

        return traducciones.GetValueOrDefault(
            estadistica,
            TextoPokemon.Formatear(estadistica));
    }
}

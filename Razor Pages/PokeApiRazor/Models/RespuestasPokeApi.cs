using System.Text.Json;
using System.Text.Json.Serialization;

namespace PokeApiRazor.Models;

// Las clases de este archivo reproducen solo la forma del JSON que necesitamos.
// JsonPropertyName relaciona cada nombre de PokeAPI con una propiedad de C#.

public class ListaPokemonApi
{
    [JsonPropertyName("count")]
    public int Total { get; set; }

    [JsonPropertyName("results")]
    public List<RecursoApi> Resultados { get; set; } = new();
}

public class RecursoApi
{
    [JsonPropertyName("name")]
    public string Nombre { get; set; } = "";

    [JsonPropertyName("url")]
    public string Url { get; set; } = "";
}

public class PokemonApi
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Nombre { get; set; } = "";

    [JsonPropertyName("base_experience")]
    public int? ExperienciaBase { get; set; }

    [JsonPropertyName("height")]
    public int Altura { get; set; }

    [JsonPropertyName("weight")]
    public int Peso { get; set; }

    [JsonPropertyName("order")]
    public int Orden { get; set; }

    [JsonPropertyName("is_default")]
    public bool EsPrincipal { get; set; }

    [JsonPropertyName("location_area_encounters")]
    public string UrlEncuentros { get; set; } = "";

    [JsonPropertyName("species")]
    public RecursoApi Especie { get; set; } = new();

    [JsonPropertyName("types")]
    public List<TipoApi> Tipos { get; set; } = new();

    [JsonPropertyName("abilities")]
    public List<HabilidadApi> Habilidades { get; set; } = new();

    [JsonPropertyName("stats")]
    public List<EstadisticaApi> Estadisticas { get; set; } = new();

    [JsonPropertyName("moves")]
    public List<MovimientoApi> Movimientos { get; set; } = new();

    [JsonPropertyName("forms")]
    public List<RecursoApi> Formas { get; set; } = new();

    [JsonPropertyName("held_items")]
    public List<ObjetoApi> Objetos { get; set; } = new();

    [JsonPropertyName("game_indices")]
    public List<IndiceJuegoApi> Juegos { get; set; } = new();

    // JsonElement permite recorrer también los sprites anidados de juegos antiguos.
    [JsonPropertyName("sprites")]
    public JsonElement Sprites { get; set; }

    [JsonPropertyName("cries")]
    public SonidosApi Sonidos { get; set; } = new();
}

public class TipoApi
{
    [JsonPropertyName("type")]
    public RecursoApi Tipo { get; set; } = new();
}

public class HabilidadApi
{
    [JsonPropertyName("is_hidden")]
    public bool EsOculta { get; set; }

    [JsonPropertyName("ability")]
    public RecursoApi Habilidad { get; set; } = new();
}

public class EstadisticaApi
{
    [JsonPropertyName("base_stat")]
    public int Valor { get; set; }

    [JsonPropertyName("effort")]
    public int Esfuerzo { get; set; }

    [JsonPropertyName("stat")]
    public RecursoApi Estadistica { get; set; } = new();
}

public class MovimientoApi
{
    [JsonPropertyName("move")]
    public RecursoApi Movimiento { get; set; } = new();
}

public class ObjetoApi
{
    [JsonPropertyName("item")]
    public RecursoApi Objeto { get; set; } = new();

    [JsonPropertyName("version_details")]
    public List<ObjetoVersionApi> Versiones { get; set; } = new();
}

public class ObjetoVersionApi
{
    [JsonPropertyName("version")]
    public RecursoApi Version { get; set; } = new();
}

public class IndiceJuegoApi
{
    [JsonPropertyName("version")]
    public RecursoApi Version { get; set; } = new();
}

public class SonidosApi
{
    [JsonPropertyName("latest")]
    public string? Actual { get; set; }

    [JsonPropertyName("legacy")]
    public string? Clasico { get; set; }
}

public class EspecieApi
{
    [JsonPropertyName("names")]
    public List<NombreTraducidoApi> Nombres { get; set; } = new();

    [JsonPropertyName("flavor_text_entries")]
    public List<DescripcionApi> Descripciones { get; set; } = new();

    [JsonPropertyName("genera")]
    public List<GeneroApi> Generos { get; set; } = new();

    [JsonPropertyName("generation")]
    public RecursoApi Generacion { get; set; } = new();

    [JsonPropertyName("habitat")]
    public RecursoApi? Habitat { get; set; }

    [JsonPropertyName("color")]
    public RecursoApi Color { get; set; } = new();

    [JsonPropertyName("shape")]
    public RecursoApi Forma { get; set; } = new();

    [JsonPropertyName("growth_rate")]
    public RecursoApi Crecimiento { get; set; } = new();

    [JsonPropertyName("capture_rate")]
    public int RatioCaptura { get; set; }

    [JsonPropertyName("base_happiness")]
    public int FelicidadBase { get; set; }

    [JsonPropertyName("hatch_counter")]
    public int ContadorEclosion { get; set; }

    [JsonPropertyName("gender_rate")]
    public int RatioSexo { get; set; }

    [JsonPropertyName("is_baby")]
    public bool EsBebe { get; set; }

    [JsonPropertyName("is_legendary")]
    public bool EsLegendario { get; set; }

    [JsonPropertyName("is_mythical")]
    public bool EsMitico { get; set; }

    [JsonPropertyName("has_gender_differences")]
    public bool TieneDiferenciasDeSexo { get; set; }

    [JsonPropertyName("egg_groups")]
    public List<RecursoApi> GruposHuevo { get; set; } = new();

    [JsonPropertyName("varieties")]
    public List<VariedadApi> Variedades { get; set; } = new();

    [JsonPropertyName("evolution_chain")]
    public UrlApi CadenaEvolucion { get; set; } = new();
}

public class NombreTraducidoApi
{
    [JsonPropertyName("name")]
    public string Nombre { get; set; } = "";

    [JsonPropertyName("language")]
    public RecursoApi Idioma { get; set; } = new();
}

public class DescripcionApi
{
    [JsonPropertyName("flavor_text")]
    public string Texto { get; set; } = "";

    [JsonPropertyName("language")]
    public RecursoApi Idioma { get; set; } = new();
}

public class GeneroApi
{
    [JsonPropertyName("genus")]
    public string Nombre { get; set; } = "";

    [JsonPropertyName("language")]
    public RecursoApi Idioma { get; set; } = new();
}

public class VariedadApi
{
    [JsonPropertyName("pokemon")]
    public RecursoApi Pokemon { get; set; } = new();
}

public class UrlApi
{
    [JsonPropertyName("url")]
    public string Url { get; set; } = "";
}

public class EncuentroApi
{
    [JsonPropertyName("location_area")]
    public RecursoApi Zona { get; set; } = new();

    [JsonPropertyName("version_details")]
    public List<EncuentroVersionApi> Versiones { get; set; } = new();
}

public class EncuentroVersionApi
{
    [JsonPropertyName("max_chance")]
    public int ProbabilidadMaxima { get; set; }
}

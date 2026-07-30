using System.Text.Json.Serialization;

namespace RickAndMorty.DTOs;

// Representa la información de paginación enviada por la API.
public class InformacionPaginaDto
{
    [JsonPropertyName("count")]
    public int TotalResultados { get; set; }

    [JsonPropertyName("pages")]
    public int TotalPaginas { get; set; }

    [JsonPropertyName("next")]
    public string? Siguiente { get; set; }

    [JsonPropertyName("prev")]
    public string? Anterior { get; set; }
}

// La API utiliza la misma envoltura para sus tres listados.
public class PaginaApiDto<T>
{
    [JsonPropertyName("info")]
    public InformacionPaginaDto Informacion { get; set; } = new();

    [JsonPropertyName("results")]
    public List<T> Resultados { get; set; } = [];
}

// Las referencias relacionan personajes, episodios y localizaciones.
public class ReferenciaDto
{
    [JsonPropertyName("name")]
    public string Nombre { get; set; } = "";

    [JsonPropertyName("url")]
    public string Url { get; set; } = "";
}

// Contiene todos los datos del recurso Character que utiliza la aplicación.
public class PersonajeDto
{
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Nombre { get; set; } = "";

    [JsonPropertyName("status")]
    public string Estado { get; set; } = "";

    [JsonPropertyName("species")]
    public string Especie { get; set; } = "";

    [JsonPropertyName("type")]
    public string Tipo { get; set; } = "";

    [JsonPropertyName("gender")]
    public string Genero { get; set; } = "";

    [JsonPropertyName("origin")]
    public ReferenciaDto Origen { get; set; } = new();

    [JsonPropertyName("location")]
    public ReferenciaDto Localizacion { get; set; } = new();

    [JsonPropertyName("image")]
    public string Imagen { get; set; } = "";

    [JsonPropertyName("episode")]
    public List<string> Episodios { get; set; } = [];

    [JsonPropertyName("url")]
    public string Url { get; set; } = "";

    [JsonPropertyName("created")]
    public DateTimeOffset Creado { get; set; }
}

// Representa un episodio y las URL de sus personajes.
public class EpisodioDto
{
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Nombre { get; set; } = "";

    [JsonPropertyName("air_date")]
    public string FechaEmision { get; set; } = "";

    [JsonPropertyName("episode")]
    public string Codigo { get; set; } = "";

    [JsonPropertyName("characters")]
    public List<string> Personajes { get; set; } = [];

    [JsonPropertyName("url")]
    public string Url { get; set; } = "";

    [JsonPropertyName("created")]
    public DateTimeOffset Creado { get; set; }
}

// Representa una localización y las URL de sus residentes.
public class LocalizacionDto
{
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Nombre { get; set; } = "";

    [JsonPropertyName("type")]
    public string Tipo { get; set; } = "";

    [JsonPropertyName("dimension")]
    public string Dimension { get; set; } = "";

    [JsonPropertyName("residents")]
    public List<string> Residentes { get; set; } = [];

    [JsonPropertyName("url")]
    public string Url { get; set; } = "";

    [JsonPropertyName("created")]
    public DateTimeOffset Creado { get; set; }
}

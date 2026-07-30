using System.Text.Json;
using System.Text.Json.Serialization;

namespace Biblioteca.DTOs;

// Los DTO reproducen solo los campos utilizados de las respuestas JSON.
public class BusquedaLibrosDto
{
    [JsonPropertyName("numFound")]
    public int NumEncontrados { get; set; }

    // Algunas versiones antiguas del endpoint utilizaban este nombre.
    [JsonPropertyName("num_found")]
    public int NumEncontradosAlternativo { get; set; }

    [JsonPropertyName("start")]
    public int Inicio { get; set; }

    [JsonPropertyName("docs")]
    public List<LibroBusquedaDto> Documentos { get; set; } = [];

    public int Total =>
        Math.Max(NumEncontrados, NumEncontradosAlternativo);
}

public class LibroBusquedaDto
{
    [JsonPropertyName("key")]
    public string Clave { get; set; } = "";

    [JsonPropertyName("title")]
    public string Titulo { get; set; } = "";

    [JsonPropertyName("author_name")]
    public List<string> Autores { get; set; } = [];

    [JsonPropertyName("first_publish_year")]
    public int? PrimeraPublicacion { get; set; }

    [JsonPropertyName("cover_i")]
    public long? PortadaId { get; set; }

    [JsonPropertyName("edition_count")]
    public int NumeroEdiciones { get; set; }

    [JsonPropertyName("ratings_average")]
    public double? Puntuacion { get; set; }

    [JsonPropertyName("ratings_count")]
    public int NumeroValoraciones { get; set; }

    [JsonPropertyName("isbn")]
    public List<string> Isbn { get; set; } = [];

    [JsonPropertyName("language")]
    public List<string> Idiomas { get; set; } = [];

    [JsonPropertyName("subject")]
    public List<string> Materias { get; set; } = [];

    [JsonPropertyName("number_of_pages_median")]
    public int? NumeroPaginas { get; set; }

    [JsonPropertyName("ebook_access")]
    public string AccesoElectronico { get; set; } = "";
}

public class ObraDto
{
    [JsonPropertyName("key")]
    public string Clave { get; set; } = "";

    [JsonPropertyName("title")]
    public string Titulo { get; set; } = "";

    // Puede ser una cadena o un objeto con una propiedad value.
    [JsonPropertyName("description")]
    public JsonElement Descripcion { get; set; }

    [JsonPropertyName("covers")]
    public List<long> Portadas { get; set; } = [];

    [JsonPropertyName("subjects")]
    public List<string> Materias { get; set; } = [];

    [JsonPropertyName("first_publish_date")]
    public string? PrimeraFechaPublicacion { get; set; }
}

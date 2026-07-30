using System.Text.Json.Serialization;

namespace Videojuegos.DTOs;

// RAWG utiliza esta estructura para todos sus listados paginados.
public class PaginaRawgDto<T>
{
    [JsonPropertyName("count")]
    public int Total { get; set; }

    [JsonPropertyName("results")]
    public List<T> Resultados { get; set; } = [];
}

// Reutilizamos la misma clase en géneros, empresas, tiendas y plataformas.
public class NombreRawgDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Nombre { get; set; } = "";

    [JsonPropertyName("slug")]
    public string Slug { get; set; } = "";
}

public class PlataformaRawgDto
{
    [JsonPropertyName("platform")]
    public NombreRawgDto Plataforma { get; set; } = new();
}

public class TiendaRawgDto
{
    [JsonPropertyName("store")]
    public NombreRawgDto Tienda { get; set; } = new();
}

public class CapturaRawgDto
{
    [JsonPropertyName("image")]
    public string Imagen { get; set; } = "";
}

// Contiene los campos compartidos por listados y fichas.
public class VideojuegoRawgDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("slug")]
    public string Slug { get; set; } = "";

    [JsonPropertyName("name")]
    public string Nombre { get; set; } = "";

    [JsonPropertyName("released")]
    public DateTime? FechaLanzamiento { get; set; }

    [JsonPropertyName("background_image")]
    public string? ImagenUrl { get; set; }

    [JsonPropertyName("rating")]
    public double? Puntuacion { get; set; }

    [JsonPropertyName("ratings_count")]
    public int NumeroValoraciones { get; set; }

    [JsonPropertyName("metacritic")]
    public int? Metacritic { get; set; }

    [JsonPropertyName("playtime")]
    public int TiempoJuego { get; set; }

    [JsonPropertyName("genres")]
    public List<NombreRawgDto> Generos { get; set; } = [];

    [JsonPropertyName("parent_platforms")]
    public List<PlataformaRawgDto> PlataformasPadre { get; set; } = [];

    [JsonPropertyName("platforms")]
    public List<PlataformaRawgDto> Plataformas { get; set; } = [];

    [JsonPropertyName("stores")]
    public List<TiendaRawgDto> Tiendas { get; set; } = [];

    [JsonPropertyName("developers")]
    public List<NombreRawgDto> Desarrolladores { get; set; } = [];

    [JsonPropertyName("publishers")]
    public List<NombreRawgDto> Editores { get; set; } = [];

    [JsonPropertyName("description_raw")]
    public string Descripcion { get; set; } = "";

    [JsonPropertyName("website")]
    public string SitioWeb { get; set; } = "";

    [JsonPropertyName("esrb_rating")]
    public NombreRawgDto? ClasificacionEdad { get; set; }
}

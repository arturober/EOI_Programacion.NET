using System.Text.Json.Serialization;

namespace Peliculas.DTOs;

// Los DTO reproducen solamente los campos del JSON que utiliza la aplicación.
public class PaginaPeliculasDto
{
    [JsonPropertyName("page")]
    public int Pagina { get; set; }

    [JsonPropertyName("total_pages")]
    public int TotalPaginas { get; set; }

    [JsonPropertyName("total_results")]
    public int TotalResultados { get; set; }

    [JsonPropertyName("results")]
    public List<PeliculaDto> Resultados { get; set; } = [];
}

public class PeliculaDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("title")]
    public string Titulo { get; set; } = "";

    [JsonPropertyName("original_title")]
    public string TituloOriginal { get; set; } = "";

    [JsonPropertyName("overview")]
    public string Sinopsis { get; set; } = "";

    [JsonPropertyName("poster_path")]
    public string? RutaPoster { get; set; }

    [JsonPropertyName("backdrop_path")]
    public string? RutaFondo { get; set; }

    [JsonPropertyName("release_date")]
    public string? FechaEstreno { get; set; }

    [JsonPropertyName("vote_average")]
    public double Puntuacion { get; set; }

    [JsonPropertyName("vote_count")]
    public int NumeroVotos { get; set; }
}

public class PeliculaDetalleDto : PeliculaDto
{
    [JsonPropertyName("tagline")]
    public string Eslogan { get; set; } = "";

    [JsonPropertyName("runtime")]
    public int? Duracion { get; set; }

    [JsonPropertyName("status")]
    public string Estado { get; set; } = "";

    [JsonPropertyName("homepage")]
    public string? PaginaOficial { get; set; }

    [JsonPropertyName("imdb_id")]
    public string? ImdbId { get; set; }

    [JsonPropertyName("genres")]
    public List<GeneroDto> Generos { get; set; } = [];

    [JsonPropertyName("production_countries")]
    public List<PaisDto> Paises { get; set; } = [];

    [JsonPropertyName("credits")]
    public CreditosDto Creditos { get; set; } = new();

    [JsonPropertyName("videos")]
    public VideosRespuestaDto Videos { get; set; } = new();

    [JsonPropertyName("recommendations")]
    public PaginaPeliculasDto Recomendaciones { get; set; } = new();

    [JsonPropertyName("watch/providers")]
    public ProveedoresRespuestaDto Proveedores { get; set; } = new();
}

public class GeneroDto
{
    [JsonPropertyName("name")]
    public string Nombre { get; set; } = "";
}

public class PaisDto
{
    [JsonPropertyName("name")]
    public string Nombre { get; set; } = "";
}

public class CreditosDto
{
    [JsonPropertyName("cast")]
    public List<RepartoDto> Reparto { get; set; } = [];

    [JsonPropertyName("crew")]
    public List<EquipoDto> Equipo { get; set; } = [];
}

public class RepartoDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Nombre { get; set; } = "";

    [JsonPropertyName("character")]
    public string Personaje { get; set; } = "";

    [JsonPropertyName("profile_path")]
    public string? RutaFoto { get; set; }

    [JsonPropertyName("order")]
    public int Orden { get; set; }
}

public class EquipoDto
{
    [JsonPropertyName("name")]
    public string Nombre { get; set; } = "";

    [JsonPropertyName("job")]
    public string Trabajo { get; set; } = "";
}

public class VideosRespuestaDto
{
    [JsonPropertyName("results")]
    public List<VideoDto> Resultados { get; set; } = [];
}

public class VideoDto
{
    [JsonPropertyName("name")]
    public string Nombre { get; set; } = "";

    [JsonPropertyName("key")]
    public string Clave { get; set; } = "";

    [JsonPropertyName("site")]
    public string Sitio { get; set; } = "";

    [JsonPropertyName("type")]
    public string Tipo { get; set; } = "";

    [JsonPropertyName("official")]
    public bool Oficial { get; set; }
}

public class ProveedoresRespuestaDto
{
    [JsonPropertyName("results")]
    public Dictionary<string, ProveedoresRegionDto> Regiones { get; set; } = [];
}

public class ProveedoresRegionDto
{
    [JsonPropertyName("link")]
    public string? Enlace { get; set; }

    [JsonPropertyName("flatrate")]
    public List<ProveedorDto> Suscripcion { get; set; } = [];

    [JsonPropertyName("rent")]
    public List<ProveedorDto> Alquiler { get; set; } = [];

    [JsonPropertyName("buy")]
    public List<ProveedorDto> Compra { get; set; } = [];
}

public class ProveedorDto
{
    [JsonPropertyName("provider_id")]
    public int Id { get; set; }

    [JsonPropertyName("provider_name")]
    public string Nombre { get; set; } = "";

    [JsonPropertyName("logo_path")]
    public string? RutaLogo { get; set; }

    [JsonPropertyName("display_priority")]
    public int Prioridad { get; set; }
}

using System.Text.Json;
using System.Text.Json.Serialization;

namespace NasaExplorer.DTOs;

// Datos de Astronomy Picture of the Day.
public class ApodDto
{
    [JsonPropertyName("date")]
    public string Fecha { get; set; } = string.Empty;

    [JsonPropertyName("title")]
    public string Titulo { get; set; } = string.Empty;

    [JsonPropertyName("explanation")]
    public string Explicacion { get; set; } = string.Empty;

    [JsonPropertyName("media_type")]
    public string TipoMedio { get; set; } = string.Empty;

    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;

    [JsonPropertyName("hdurl")]
    public string? UrlAltaResolucion { get; set; }

    [JsonPropertyName("thumbnail_url")]
    public string? MiniaturaUrl { get; set; }

    [JsonPropertyName("copyright")]
    public string? Copyright { get; set; }
}

// Respuesta Collection+JSON de la biblioteca multimedia.
public class MediaRespuestaDto
{
    [JsonPropertyName("collection")]
    public MediaColeccionDto Coleccion { get; set; } = new();
}

public class MediaColeccionDto
{
    [JsonPropertyName("items")]
    public List<MediaItemDto> Elementos { get; set; } = [];

    [JsonPropertyName("metadata")]
    public MediaMetadatosDto Metadatos { get; set; } = new();
}

public class MediaMetadatosDto
{
    [JsonPropertyName("total_hits")]
    public int Total { get; set; }
}

public class MediaItemDto
{
    [JsonPropertyName("data")]
    public List<MediaDatosDto> Datos { get; set; } = [];

    [JsonPropertyName("links")]
    public List<MediaEnlaceDto> Enlaces { get; set; } = [];

    // Facilita el uso desde las vistas sin repetir FirstOrDefault.
    [JsonIgnore]
    public MediaDatosDto DatosPrincipales => Datos.FirstOrDefault() ?? new();

    [JsonIgnore]
    public string? Miniatura => Enlaces.FirstOrDefault()?.Href;
}

public class MediaDatosDto
{
    [JsonPropertyName("nasa_id")]
    public string NasaId { get; set; } = string.Empty;

    [JsonPropertyName("title")]
    public string Titulo { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string? Descripcion { get; set; }

    [JsonPropertyName("media_type")]
    public string TipoMedio { get; set; } = string.Empty;

    [JsonPropertyName("date_created")]
    public DateTime? FechaCreacion { get; set; }

    [JsonPropertyName("center")]
    public string? Centro { get; set; }

    [JsonPropertyName("keywords")]
    public List<string> PalabrasClave { get; set; } = [];
}

public class MediaEnlaceDto
{
    [JsonPropertyName("href")]
    public string Href { get; set; } = string.Empty;
}

// Manifiesto con los archivos disponibles para una pieza multimedia.
public class MediaManifiestoDto
{
    [JsonPropertyName("collection")]
    public MediaManifiestoColeccionDto Coleccion { get; set; } = new();
}

public class MediaManifiestoColeccionDto
{
    [JsonPropertyName("items")]
    public List<MediaManifiestoItemDto> Elementos { get; set; } = [];
}

public class MediaManifiestoItemDto
{
    [JsonPropertyName("href")]
    public string Href { get; set; } = string.Empty;
}

// Datos de una fotografía terrestre de DSCOVR EPIC.
public class EpicImagenDto
{
    [JsonPropertyName("identifier")]
    public string Identificador { get; set; } = string.Empty;

    [JsonPropertyName("image")]
    public string Imagen { get; set; } = string.Empty;

    [JsonPropertyName("caption")]
    public string Leyenda { get; set; } = string.Empty;

    [JsonPropertyName("date")]
    public DateTime Fecha { get; set; }

    [JsonPropertyName("centroid_coordinates")]
    public EpicCoordenadasDto Coordenadas { get; set; } = new();

    // Se construye en el servicio con el esquema oficial del archivo EPIC.
    [JsonIgnore]
    public string ImagenUrl { get; set; } = string.Empty;

    [JsonIgnore]
    public string MiniaturaUrl { get; set; } = string.Empty;
}

public class EpicCoordenadasDto
{
    [JsonPropertyName("lat")]
    public double Latitud { get; set; }

    [JsonPropertyName("lon")]
    public double Longitud { get; set; }
}

// Respuesta de eventos naturales EONET v3.
public class EonetRespuestaDto
{
    [JsonPropertyName("events")]
    public List<EonetEventoDto> Eventos { get; set; } = [];
}

public class EonetEventoDto
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("title")]
    public string Titulo { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string? Descripcion { get; set; }

    [JsonPropertyName("closed")]
    public DateTime? Cerrado { get; set; }

    [JsonPropertyName("categories")]
    public List<EonetCategoriaDto> Categorias { get; set; } = [];

    [JsonPropertyName("sources")]
    public List<EonetFuenteDto> Fuentes { get; set; } = [];

    [JsonPropertyName("geometry")]
    public List<EonetGeometriaDto> Geometrias { get; set; } = [];
}

public class EonetCategoriaDto
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("title")]
    public string Titulo { get; set; } = string.Empty;
}

public class EonetFuenteDto
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;
}

public class EonetGeometriaDto
{
    [JsonPropertyName("date")]
    public DateTime Fecha { get; set; }

    [JsonPropertyName("type")]
    public string Tipo { get; set; } = string.Empty;

    // JsonElement permite admitir tanto puntos como polígonos GeoJSON.
    [JsonPropertyName("coordinates")]
    public JsonElement Coordenadas { get; set; }
}

// Respuesta simplificada para un día de aproximaciones de asteroides.
public class AsteroidesResultado
{
    public int Total { get; set; }
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public List<AsteroideVista> Asteroides { get; set; } = [];
}

public class AsteroideVista
{
    public string Id { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public bool Peligroso { get; set; }
    public double DiametroMinimoKm { get; set; }
    public double DiametroMaximoKm { get; set; }
    public DateTime? FechaAproximacion { get; set; }
    public double VelocidadKmHora { get; set; }
    public double DistanciaKm { get; set; }
    public string UrlNasa { get; set; } = string.Empty;
}

// Modelo común para varios tipos de sucesos de meteorología espacial DONKI.
public class DonkiEventoVista
{
    public string Id { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public DateTime? Fecha { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string? Detalle { get; set; }
    public string? Instrumentos { get; set; }
    public string? Url { get; set; }
}

// Parámetros seleccionados de la tabla pscomppars del archivo de exoplanetas.
public class ExoplanetaDto
{
    [JsonPropertyName("pl_name")]
    public string Nombre { get; set; } = string.Empty;

    [JsonPropertyName("hostname")]
    public string Estrella { get; set; } = string.Empty;

    [JsonPropertyName("discoverymethod")]
    public string? MetodoDescubrimiento { get; set; }

    [JsonPropertyName("disc_year")]
    public int? AnioDescubrimiento { get; set; }

    [JsonPropertyName("pl_rade")]
    public double? RadiosTierra { get; set; }

    [JsonPropertyName("pl_bmasse")]
    public double? MasasTierra { get; set; }

    [JsonPropertyName("pl_orbper")]
    public double? PeriodoOrbitalDias { get; set; }

    [JsonPropertyName("pl_eqt")]
    public double? TemperaturaEquilibrio { get; set; }

    [JsonPropertyName("sy_dist")]
    public double? DistanciaParsecs { get; set; }

    [JsonPropertyName("sy_pnum")]
    public int? PlanetasSistema { get; set; }
}

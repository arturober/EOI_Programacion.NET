using System.Text.Json.Serialization;

namespace OpenWeather.DTOs;

// Estas clases reproducen únicamente los campos del JSON que utiliza la aplicación.
// Separarlas de los modelos de pantalla evita que el diseño dependa de la API externa.

public class LugarDto
{
    [JsonPropertyName("name")]
    public string Nombre { get; set; } = "";

    [JsonPropertyName("local_names")]
    public Dictionary<string, string>? NombresLocales { get; set; }

    [JsonPropertyName("lat")]
    public double Latitud { get; set; }

    [JsonPropertyName("lon")]
    public double Longitud { get; set; }

    [JsonPropertyName("country")]
    public string Pais { get; set; } = "";

    [JsonPropertyName("state")]
    public string? Region { get; set; }
}

public class TiempoActualDto
{
    [JsonPropertyName("coord")]
    public CoordenadasDto Coordenadas { get; set; } = new();

    [JsonPropertyName("weather")]
    public List<EstadoCieloDto> Estados { get; set; } = [];

    [JsonPropertyName("main")]
    public ValoresPrincipalesDto Valores { get; set; } = new();

    [JsonPropertyName("visibility")]
    public int Visibilidad { get; set; }

    [JsonPropertyName("wind")]
    public VientoDto Viento { get; set; } = new();

    [JsonPropertyName("clouds")]
    public NubesDto Nubes { get; set; } = new();

    [JsonPropertyName("dt")]
    public long FechaUnix { get; set; }

    [JsonPropertyName("sys")]
    public SistemaDto Sistema { get; set; } = new();

    [JsonPropertyName("timezone")]
    public int DesfaseHorario { get; set; }

    [JsonPropertyName("name")]
    public string Nombre { get; set; } = "";
}

public class PrevisionDto
{
    [JsonPropertyName("list")]
    public List<PeriodoDto> Periodos { get; set; } = [];

    [JsonPropertyName("city")]
    public CiudadDto Ciudad { get; set; } = new();
}

public class PeriodoDto
{
    [JsonPropertyName("dt")]
    public long FechaUnix { get; set; }

    [JsonPropertyName("main")]
    public ValoresPrincipalesDto Valores { get; set; } = new();

    [JsonPropertyName("weather")]
    public List<EstadoCieloDto> Estados { get; set; } = [];

    [JsonPropertyName("wind")]
    public VientoDto Viento { get; set; } = new();

    [JsonPropertyName("pop")]
    public double ProbabilidadPrecipitacion { get; set; }

    [JsonPropertyName("rain")]
    public PrecipitacionDto? Lluvia { get; set; }

    [JsonPropertyName("snow")]
    public PrecipitacionDto? Nieve { get; set; }
}

public class CalidadAireRespuestaDto
{
    [JsonPropertyName("list")]
    public List<CalidadAirePeriodoDto> Periodos { get; set; } = [];
}

public class CalidadAirePeriodoDto
{
    [JsonPropertyName("main")]
    public IndiceAireDto Indice { get; set; } = new();

    [JsonPropertyName("components")]
    public ComponentesAireDto Componentes { get; set; } = new();

    [JsonPropertyName("dt")]
    public long FechaUnix { get; set; }
}

public class CoordenadasDto
{
    [JsonPropertyName("lon")]
    public double Longitud { get; set; }

    [JsonPropertyName("lat")]
    public double Latitud { get; set; }
}

public class EstadoCieloDto
{
    [JsonPropertyName("description")]
    public string Descripcion { get; set; } = "";

    [JsonPropertyName("icon")]
    public string Icono { get; set; } = "01d";
}

public class ValoresPrincipalesDto
{
    [JsonPropertyName("temp")]
    public double Temperatura { get; set; }

    [JsonPropertyName("feels_like")]
    public double Sensacion { get; set; }

    [JsonPropertyName("temp_min")]
    public double Minima { get; set; }

    [JsonPropertyName("temp_max")]
    public double Maxima { get; set; }

    [JsonPropertyName("pressure")]
    public int Presion { get; set; }

    [JsonPropertyName("humidity")]
    public int Humedad { get; set; }
}

public class VientoDto
{
    [JsonPropertyName("speed")]
    public double Velocidad { get; set; }

    [JsonPropertyName("deg")]
    public int Direccion { get; set; }

    [JsonPropertyName("gust")]
    public double? Racha { get; set; }
}

public class NubesDto
{
    [JsonPropertyName("all")]
    public int Porcentaje { get; set; }
}

public class SistemaDto
{
    [JsonPropertyName("country")]
    public string Pais { get; set; } = "";

    [JsonPropertyName("sunrise")]
    public long AmanecerUnix { get; set; }

    [JsonPropertyName("sunset")]
    public long AtardecerUnix { get; set; }
}

public class CiudadDto
{
    [JsonPropertyName("name")]
    public string Nombre { get; set; } = "";

    [JsonPropertyName("country")]
    public string Pais { get; set; } = "";

    [JsonPropertyName("timezone")]
    public int DesfaseHorario { get; set; }
}

public class PrecipitacionDto
{
    [JsonPropertyName("3h")]
    public double TresHoras { get; set; }
}

public class IndiceAireDto
{
    [JsonPropertyName("aqi")]
    public int Valor { get; set; }
}

public class ComponentesAireDto
{
    [JsonPropertyName("co")]
    public double Co { get; set; }

    [JsonPropertyName("no")]
    public double No { get; set; }

    [JsonPropertyName("no2")]
    public double No2 { get; set; }

    [JsonPropertyName("o3")]
    public double O3 { get; set; }

    [JsonPropertyName("so2")]
    public double So2 { get; set; }

    [JsonPropertyName("pm2_5")]
    public double Pm25 { get; set; }

    [JsonPropertyName("pm10")]
    public double Pm10 { get; set; }

    [JsonPropertyName("nh3")]
    public double Nh3 { get; set; }
}

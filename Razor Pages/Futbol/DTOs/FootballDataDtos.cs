using System.Text.Json.Serialization;

namespace Futbol.DTOs;

// Los DTO reproducen únicamente los campos de la API que utiliza la interfaz.
public class CompeticionesRespuestaDto
{
    [JsonPropertyName("competitions")]
    public List<CompeticionDto> Competiciones { get; set; } = [];
}

public class CompeticionDto
{
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Nombre { get; set; } = "";

    [JsonPropertyName("code")]
    public string? Codigo { get; set; }

    [JsonPropertyName("type")]
    public string? Tipo { get; set; }

    [JsonPropertyName("emblem")]
    public string? Emblema { get; set; }

    [JsonPropertyName("area")]
    public AreaDto? Area { get; set; }

    [JsonPropertyName("currentSeason")]
    public TemporadaDto? TemporadaActual { get; set; }
}

public class AreaDto
{
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Nombre { get; set; } = "";

    [JsonPropertyName("code")]
    public string? Codigo { get; set; }

    [JsonPropertyName("flag")]
    public string? Bandera { get; set; }
}

public class TemporadaDto
{
    public int Id { get; set; }

    [JsonPropertyName("startDate")]
    public DateOnly? FechaInicio { get; set; }

    [JsonPropertyName("endDate")]
    public DateOnly? FechaFin { get; set; }

    [JsonPropertyName("currentMatchday")]
    public int? JornadaActual { get; set; }
}

public class PartidosRespuestaDto
{
    [JsonPropertyName("matches")]
    public List<PartidoDto> Partidos { get; set; } = [];
}

public class PartidoDto
{
    public int Id { get; set; }

    [JsonPropertyName("utcDate")]
    public DateTimeOffset FechaUtc { get; set; }

    [JsonPropertyName("status")]
    public string Estado { get; set; } = "";

    [JsonPropertyName("matchday")]
    public int? Jornada { get; set; }

    [JsonPropertyName("stage")]
    public string? Fase { get; set; }

    [JsonPropertyName("group")]
    public string? Grupo { get; set; }

    [JsonPropertyName("competition")]
    public CompeticionDto? Competicion { get; set; }

    [JsonPropertyName("homeTeam")]
    public EquipoResumenDto Local { get; set; } = new();

    [JsonPropertyName("awayTeam")]
    public EquipoResumenDto Visitante { get; set; } = new();

    [JsonPropertyName("score")]
    public MarcadorDto Marcador { get; set; } = new();
}

public class EquipoResumenDto
{
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Nombre { get; set; } = "";

    [JsonPropertyName("shortName")]
    public string? NombreCorto { get; set; }

    [JsonPropertyName("tla")]
    public string? Siglas { get; set; }

    [JsonPropertyName("crest")]
    public string? Escudo { get; set; }
}

public class MarcadorDto
{
    [JsonPropertyName("winner")]
    public string? Ganador { get; set; }

    [JsonPropertyName("duration")]
    public string? Duracion { get; set; }

    [JsonPropertyName("fullTime")]
    public GolesDto Final { get; set; } = new();

    [JsonPropertyName("halfTime")]
    public GolesDto Descanso { get; set; } = new();
}

public class GolesDto
{
    [JsonPropertyName("home")]
    public int? Local { get; set; }

    [JsonPropertyName("away")]
    public int? Visitante { get; set; }
}

public class ClasificacionRespuestaDto
{
    [JsonPropertyName("competition")]
    public CompeticionDto? Competicion { get; set; }

    [JsonPropertyName("season")]
    public TemporadaDto? Temporada { get; set; }

    [JsonPropertyName("standings")]
    public List<ClasificacionDto> Clasificaciones { get; set; } = [];
}

public class ClasificacionDto
{
    [JsonPropertyName("stage")]
    public string? Fase { get; set; }

    [JsonPropertyName("type")]
    public string? Tipo { get; set; }

    [JsonPropertyName("group")]
    public string? Grupo { get; set; }

    [JsonPropertyName("table")]
    public List<FilaClasificacionDto> Tabla { get; set; } = [];
}

public class FilaClasificacionDto
{
    [JsonPropertyName("position")]
    public int Posicion { get; set; }

    [JsonPropertyName("team")]
    public EquipoResumenDto Equipo { get; set; } = new();

    [JsonPropertyName("playedGames")]
    public int Jugados { get; set; }

    [JsonPropertyName("form")]
    public string? Forma { get; set; }

    [JsonPropertyName("won")]
    public int Ganados { get; set; }

    [JsonPropertyName("draw")]
    public int Empatados { get; set; }

    [JsonPropertyName("lost")]
    public int Perdidos { get; set; }

    [JsonPropertyName("points")]
    public int Puntos { get; set; }

    [JsonPropertyName("goalsFor")]
    public int GolesFavor { get; set; }

    [JsonPropertyName("goalsAgainst")]
    public int GolesContra { get; set; }

    [JsonPropertyName("goalDifference")]
    public int Diferencia { get; set; }
}

public class GoleadoresRespuestaDto
{
    [JsonPropertyName("competition")]
    public CompeticionDto? Competicion { get; set; }

    [JsonPropertyName("scorers")]
    public List<GoleadorDto> Goleadores { get; set; } = [];
}

public class GoleadorDto
{
    [JsonPropertyName("player")]
    public PersonaDto Jugador { get; set; } = new();

    [JsonPropertyName("team")]
    public EquipoResumenDto Equipo { get; set; } = new();

    [JsonPropertyName("playedMatches")]
    public int? PartidosJugados { get; set; }

    [JsonPropertyName("goals")]
    public int? Goles { get; set; }

    [JsonPropertyName("assists")]
    public int? Asistencias { get; set; }

    [JsonPropertyName("penalties")]
    public int? Penaltis { get; set; }
}

public class EquiposRespuestaDto
{
    [JsonPropertyName("competition")]
    public CompeticionDto? Competicion { get; set; }

    [JsonPropertyName("teams")]
    public List<EquipoResumenDto> Equipos { get; set; } = [];
}

public class EquipoDetalleDto : EquipoResumenDto
{
    [JsonPropertyName("area")]
    public AreaDto? Area { get; set; }

    [JsonPropertyName("address")]
    public string? Direccion { get; set; }

    [JsonPropertyName("website")]
    public string? SitioWeb { get; set; }

    [JsonPropertyName("founded")]
    public int? Fundacion { get; set; }

    [JsonPropertyName("clubColors")]
    public string? Colores { get; set; }

    [JsonPropertyName("venue")]
    public string? Estadio { get; set; }

    [JsonPropertyName("runningCompetitions")]
    public List<CompeticionDto> Competiciones { get; set; } = [];

    [JsonPropertyName("coach")]
    public PersonaDto? Entrenador { get; set; }

    [JsonPropertyName("squad")]
    public List<PersonaDto> Plantilla { get; set; } = [];
}

public class PersonaDto
{
    public int? Id { get; set; }

    [JsonPropertyName("name")]
    public string? Nombre { get; set; }

    [JsonPropertyName("position")]
    public string? Posicion { get; set; }

    [JsonPropertyName("dateOfBirth")]
    public DateOnly? FechaNacimiento { get; set; }

    [JsonPropertyName("nationality")]
    public string? Nacionalidad { get; set; }

    [JsonPropertyName("shirtNumber")]
    public int? Dorsal { get; set; }
}

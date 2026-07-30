namespace Futbol.Servicios;

// Traduce los códigos más habituales de football-data.org.
public static class TextoFutbol
{
    public static string Estado(string? codigo) => codigo switch
    {
        "SCHEDULED" => "Programado",
        "TIMED" => "Con horario",
        "IN_PLAY" => "En juego",
        "PAUSED" => "Descanso",
        "FINISHED" => "Finalizado",
        "POSTPONED" => "Aplazado",
        "SUSPENDED" => "Suspendido",
        "CANCELLED" => "Cancelado",
        _ => codigo ?? "Sin estado"
    };

    public static string Fase(string? codigo) => codigo switch
    {
        "REGULAR_SEASON" => "Liga regular",
        "GROUP_STAGE" => "Fase de grupos",
        "LAST_16" or "ROUND_OF_16" => "Octavos de final",
        "QUARTER_FINALS" => "Cuartos de final",
        "SEMI_FINALS" => "Semifinales",
        "FINAL" => "Final",
        "PLAYOFFS" => "Eliminatorias",
        _ => (codigo ?? "Sin fase").Replace('_', ' ').ToLowerInvariant()
    };

    public static string Posicion(string? codigo) => codigo switch
    {
        "Goalkeeper" => "Portero",
        "Defence" or "Defender" => "Defensa",
        "Midfield" or "Midfielder" => "Centrocampista",
        "Offence" or "Attacker" => "Delantero",
        _ => codigo ?? "Sin especificar"
    };
}

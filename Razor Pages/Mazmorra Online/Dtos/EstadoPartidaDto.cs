using MazmorraOnline.Models;

namespace MazmorraOnline.Dtos;

// Estado reducido que SignalR envía a los navegadores diez veces por segundo.
public class EstadoPartidaDto
{
    // Datos generales de la ronda.
    public EstadoPartida Estado { get; set; }
    public int NumeroRonda { get; set; }
    public string NombreMapa { get; set; } = "";
    public int SegundosRestantes { get; set; }
    public int SegundosParaReiniciar { get; set; }
    public string? Ganador { get; set; }
    // Los muros no aparecen aquí porque Razor ya envió los mapas.
    public List<JugadorEstadoDto> Jugadores { get; set; } = new();
    public List<ProyectilEstadoDto> Proyectiles { get; set; } = new();
    public List<PowerUp> PowerUps { get; set; } = new();
}

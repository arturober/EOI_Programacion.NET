namespace MazmorraOnline.Models;

// Agrupa todos los objetos que forman la partida global.
public class Partida
{
    // Información general de la ronda.
    public EstadoPartida Estado { get; set; } = EstadoPartida.Esperando;
    public float SegundosRestantes { get; set; } = 90;
    public float SegundosParaReiniciar { get; set; }
    public string? Ganador { get; set; }

    // El diccionario permite encontrar un jugador rápidamente mediante su ID.
    public Dictionary<string, Jugador> Jugadores { get; set; } = new();

    // Estas listas cambian mientras se ejecuta la física.
    public List<Proyectil> Proyectiles { get; set; } = new();
    public List<PowerUp> PowerUps { get; set; } = new();
}

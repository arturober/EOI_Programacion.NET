namespace Equipos.Models;

public class Equipo
{
  public int Id { get; set; }

  public required string Nombre { get; set; }

  public ICollection<Jugador> Jugadores { get; set; } = [];
}

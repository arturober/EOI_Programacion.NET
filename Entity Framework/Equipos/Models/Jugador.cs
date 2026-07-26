namespace Equipos.Models;

public class Jugador
{
  public int Id { get; set; }

  public required string Nombre { get; set; }

  public required int EquipoId { get; set; }

  public Equipo? Equipo { get; set; }
}

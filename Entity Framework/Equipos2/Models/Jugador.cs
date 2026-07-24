namespace Equipos.Models;

public class Jugador
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    public required string Nickname { get; set; }
    
    public required string NombreCompleto { get; set; }
    
    public required string Rol { get; set; }
    
    public Guid EquipoId { get; set; }
    
    public Equipo? Equipo { get; set; }
}

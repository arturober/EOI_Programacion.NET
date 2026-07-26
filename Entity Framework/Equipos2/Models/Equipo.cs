namespace Equipos.Models;

public class Equipo
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    public required string Nombre { get; set; }
    
    public required string Juego { get; set; }
    
    public string? LogoUrl { get; set; }
    
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    
    public ICollection<Jugador> Jugadores { get; set; } = [];
}

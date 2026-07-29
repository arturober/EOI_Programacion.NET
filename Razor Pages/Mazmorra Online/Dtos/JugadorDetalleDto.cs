namespace MazmorraOnline.Dtos;

// Información detallada que devuelve GET /api/jugadores/{id}.
public class JugadorDetalleDto
{
    public string Id { get; set; } = "";
    public string Nombre { get; set; } = "";
    public int Vida { get; set; }
    public bool Vivo { get; set; }
    public int Victorias { get; set; }
    public int Eliminaciones { get; set; }
    public bool TieneEscudo { get; set; }
    public bool TieneVelocidad { get; set; }
    public bool TieneDisparoRapido { get; set; }
}

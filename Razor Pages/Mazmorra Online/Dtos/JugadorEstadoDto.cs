namespace MazmorraOnline.Dtos;

// Datos de un jugador necesarios para dibujarlo y mostrar sus estadísticas.
public class JugadorEstadoDto
{
    public string Id { get; set; } = "";
    public string Nombre { get; set; } = "";
    public string Color { get; set; } = "";
    public float X { get; set; }
    public float Y { get; set; }
    public float Angulo { get; set; }
    public int Vida { get; set; }
    public bool Vivo { get; set; }
    public bool TieneEscudo { get; set; }
    public int Victorias { get; set; }
    public int Eliminaciones { get; set; }
}

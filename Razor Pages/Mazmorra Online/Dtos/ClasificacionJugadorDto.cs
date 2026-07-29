namespace MazmorraOnline.Dtos;

// Datos públicos de un jugador necesarios para la clasificación.
public class ClasificacionJugadorDto
{
    public string Id { get; set; } = "";
    public string Nombre { get; set; } = "";
    public int Victorias { get; set; }
    public int Eliminaciones { get; set; }
}

namespace MazmorraOnline.Dtos;

// Guarda los datos que se muestran en el historial de rondas.
public class ResultadoRondaDto
{
    public int Numero { get; set; }
    public string? Ganador { get; set; }
    public int NumeroJugadores { get; set; }
    public string Mapa { get; set; } = "";
    public DateTime Fecha { get; set; }
}

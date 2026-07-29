namespace MazmorraOnline.Models;

// Representa un tablero leído desde uno de los archivos de texto.
public class Mapa
{
    // Filas conserva el dibujo original para enviarlo a Razor y JavaScript.
    public string Nombre { get; set; } = "";
    public List<string> Filas { get; set; } = new();

    // Estas listas facilitan los cálculos de física y colocación.
    public List<Muro> Muros { get; set; } = new();
    public List<(float X, float Y)> PosicionesJugadores { get; set; } = new();
    public List<(float X, float Y)> PosicionesPowerUps { get; set; } = new();
}

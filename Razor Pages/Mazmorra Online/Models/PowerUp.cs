namespace MazmorraOnline.Models;

// Representa una mejora que puede recoger un jugador.
public class PowerUp
{
    // Tipo decide el efecto; X e Y indican dónde se dibuja y se recoge.
    public TipoPowerUp Tipo { get; set; }
    public float X { get; set; }
    public float Y { get; set; }
}

namespace MazmorraOnline.Models;

// Contiene los datos internos que necesita el servidor para un proyectil.
public class Proyectil
{
    // El propietario no puede recibir daño de su propio disparo.
    public string PropietarioId { get; set; } = "";

    // La posición se envía al navegador; las velocidades solo usa el servidor.
    public float X { get; set; }
    public float Y { get; set; }
    public float VelocidadX { get; set; }
    public float VelocidadY { get; set; }
}

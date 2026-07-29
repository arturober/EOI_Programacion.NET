namespace OpenWeather.Modelos;

// Contiene una localidad devuelta por el servicio de geocodificación.
public class Lugar
{
    public string Nombre { get; set; } = "";
    public string? Region { get; set; }
    public string Pais { get; set; } = "";
    public double Latitud { get; set; }
    public double Longitud { get; set; }

    // Une solamente las partes disponibles para formar un texto legible.
    public string NombreCompleto =>
        string.Join(", ", new[] { Nombre, Region, Pais }
            .Where(parte => !string.IsNullOrWhiteSpace(parte)));
}

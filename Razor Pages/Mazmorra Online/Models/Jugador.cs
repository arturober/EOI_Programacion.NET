namespace MazmorraOnline.Models;

// Guarda todos los datos de un jugador mientras la aplicación está abierta.
public class Jugador
{
    // Guid crea un identificador distinto para cada jugador.
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Nombre { get; set; } = "";
    public string Color { get; set; } = "#4dabf7";

    // Posición y dirección actuales dentro del tablero.
    public float X { get; set; }
    public float Y { get; set; }
    public float Angulo { get; set; }

    // Datos que se reinician o acumulan durante las rondas.
    public int Vida { get; set; } = 100;
    public bool Vivo { get; set; } = true;
    public int Victorias { get; set; }
    public int Eliminaciones { get; set; }

    // El servidor conserva la última acción recibida del navegador.
    public AccionJugador Accion { get; set; } = new();

    // SignalR asigna un identificador distinto a cada conexión.
    // Sirve para distinguir una conexión nueva de otra antigua que se cierra.
    public string? ConexionId { get; set; }

    // Si la conexión se pierde, se conserva al jugador unos segundos.
    public DateTime? DesconectadoDesde { get; set; }

    // Solo cambia cuando existe una interacción real con los controles.
    public DateTime UltimaActividad { get; set; } = DateTime.UtcNow;

    // Estas propiedades controlan efectos temporales.
    public float TiempoEscudo { get; set; }
    public float TiempoVelocidad { get; set; }
    public float TiempoDisparoRapido { get; set; }

    // Tiempo que falta para poder volver a disparar.
    public float TiempoRecarga { get; set; }
}

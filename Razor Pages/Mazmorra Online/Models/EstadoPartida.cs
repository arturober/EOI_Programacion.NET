namespace MazmorraOnline.Models;

// Indica en qué fase se encuentra la partida global.
public enum EstadoPartida
{
    // Faltan jugadores para comenzar.
    Esperando,

    // Hay una ronda activa.
    EnJuego,

    // Se está mostrando el ganador antes de reiniciar.
    Finalizada
}

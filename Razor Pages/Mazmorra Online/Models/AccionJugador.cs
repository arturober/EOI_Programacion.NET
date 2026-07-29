namespace MazmorraOnline.Models;

// Contiene las acciones que un jugador está realizando en este momento.
// El navegador envía este objeto al servidor diez veces por segundo.
public class AccionJugador
{
    // Estas propiedades indican las direcciones que están pulsadas.
    public bool Arriba { get; set; }
    public bool Abajo { get; set; }
    public bool Izquierda { get; set; }
    public bool Derecha { get; set; }

    // Disparar permanece a true mientras se mantenga pulsado el botón.
    public bool Disparar { get; set; }

    // El ángulo se expresa en radianes y señala la dirección del disparo.
    public float Angulo { get; set; }
}

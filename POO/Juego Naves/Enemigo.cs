using System.Numerics;
using Raylib_cs;

public class Enemigo(Vector2 posicion, Vector2 tamano, Color color, float velocidad, int vidaMaxima, int danoAlColisionar, int puntosAlMorir, Jugador jugador)
: Personaje(posicion, tamano, color, velocidad, vidaMaxima)
{
  public int DanoAlColisionar { get; protected set; } = danoAlColisionar;
  public int PuntosAlMorir { get; protected set; } = puntosAlMorir;
  protected Jugador Target { get; private set; } = jugador;

  public static Enemigo CrearEnemigoBasico(Vector2 posicionInicial, Jugador jugador)
  {
    return new Enemigo(posicionInicial, new Vector2(30, 30), new Color(230, 40, 80, 255), 120f, 20, 10, 100, jugador);
  }

  public static Enemigo CrearEnemigoRapido(Vector2 posicionInicial, Jugador jugador)
  {
    return new Enemigo(posicionInicial, new Vector2(22, 22), new Color(190, 40, 230, 255), 200f, 10, 5, 200, jugador);
  }

  public override void Actualizar(float deltaTime)
  {
    if (!Activo || !Target.Activo) return;

    // Calcular dirección hacia el jugador
    Vector2 direccion = Target.Posicion - Posicion;

    if (direccion != Vector2.Zero)
    {
      direccion = Vector2.Normalize(direccion);
    }

    // Moverse hacia el jugador
    Posicion += direccion * Velocidad * deltaTime;
  }
}

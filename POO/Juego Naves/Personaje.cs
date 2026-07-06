using System.Numerics;
using Raylib_cs;

public abstract class Personaje(Vector2 posicion, Vector2 tamano, Color color, float velocidad, int vidaMaxima)
: Entidad(posicion, tamano, color)
{
  public float Velocidad { get; set; } = velocidad;
  public int Vida { get; protected set; } = vidaMaxima;
  public int VidaMaxima { get; protected set; } = vidaMaxima;

  public virtual void RecibirDano(int cantidad)
  {
    Vida -= cantidad;
    if (Vida <= 0)
    {
      Vida = 0;
      Activo = false;
    }
  }

  public virtual void Curar(int cantidad)
  {
    Vida += cantidad;
    if (Vida > VidaMaxima)
    {
      Vida = VidaMaxima;
    }
  }
}

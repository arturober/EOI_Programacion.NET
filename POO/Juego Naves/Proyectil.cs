using System.Numerics;
using Raylib_cs;

public class Proyectil(Vector2 posicionInicial, Vector2 direccion, float velocidad)
: Entidad(posicionInicial - new Vector2(4, 4), new Vector2(8, 8), new Color(255, 220, 0, 255))
{
  public Vector2 Direccion { get; private set; } = direccion;
  public float Velocidad { get; private set; } = velocidad;

  public override void Actualizar(float deltaTime)
    {
        if (!Activo) return;

        Posicion += Direccion * Velocidad * deltaTime;

        // Desactivar si sale de los límites de la pantalla (con un pequeño margen de 50px)
        if (Posicion.X < -50 || Posicion.X > 850 || Posicion.Y < -50 || Posicion.Y > 650)
        {
            Activo = false;
        }
    }

    public override void Dibujar()
    {
        if (!Activo) return;

        // Dibujamos el proyectil como un círculo brillante
        Raylib.DrawCircleV(Posicion + Tamano / 2, Tamano.X / 2, Color);
    }
}

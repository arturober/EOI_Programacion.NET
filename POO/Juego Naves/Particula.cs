using System.Numerics;
using Raylib_cs;

public class Particula(Vector2 posicion, Vector2 velocidad, Color color, float duracion)
: Entidad(posicion, new Vector2(4 + Random.Shared.Next(0, 4), 4 + Random.Shared.Next(0, 4)), color)
{
  public Vector2 Velocidad { get; set; } = velocidad;
  public float VidaRestante { get; private set; } = duracion;
  public float VidaTotal { get; private set; } = duracion;
  private byte _alphaInicial = color.A;

  public override void Actualizar(float deltaTime)
    {
        if (!Activo) return;

        Posicion += Velocidad * deltaTime;

        // Simular fricción o desaceleración lenta
        Velocidad *= 0.95f;

        VidaRestante -= deltaTime;
        if (VidaRestante <= 0)
        {
            Activo = false;
        }
    }

    public override void Dibujar()
    {
        if (!Activo) return;

        // Desvanecer la partícula basándonos en el tiempo de vida restante
        float porcentajeVida = Math.Clamp(VidaRestante / VidaTotal, 0f, 1f);
        byte nuevoAlpha = (byte)(_alphaInicial * porcentajeVida);
        var colorConAlpha = new Color(Color.R, Color.G, Color.B, nuevoAlpha);

        Raylib.DrawRectangleV(Posicion, Tamano, colorConAlpha);
    }
}

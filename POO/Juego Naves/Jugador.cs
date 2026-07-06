using System.Numerics;
using Raylib_cs;

public class Jugador(Vector2 posicionInicial, float velocidad) : Personaje(posicionInicial, new Vector2(40, 40), new Color(0, 180, 255, 255), velocidad, 100)
{
    private float _tiempoDesdeUltimoDisparo = 0f;
    private const float CooldownDisparo = 0.2f; // Cooldown de 0.2 segundos (5 disparos por segundo)

  public override void Actualizar(float deltaTime)
    {
        // Actualizar cooldown
        _tiempoDesdeUltimoDisparo += deltaTime;

        // Leer movimiento
        Vector2 direccion = Vector2.Zero;
        if (Raylib.IsKeyDown(KeyboardKey.Up) || Raylib.IsKeyDown(KeyboardKey.W))    direccion.Y -= 1;
        if (Raylib.IsKeyDown(KeyboardKey.Down) || Raylib.IsKeyDown(KeyboardKey.S))  direccion.Y += 1;
        if (Raylib.IsKeyDown(KeyboardKey.Left) || Raylib.IsKeyDown(KeyboardKey.A))  direccion.X -= 1;
        if (Raylib.IsKeyDown(KeyboardKey.Right) || Raylib.IsKeyDown(KeyboardKey.D)) direccion.X += 1;

        if (direccion != Vector2.Zero)
        {
            direccion = Vector2.Normalize(direccion);
        }

        // Mover y limitar a la pantalla (800x600)
        Vector2 nuevaPosicion = Posicion + direccion * Velocidad * deltaTime;
        nuevaPosicion.X = Math.Clamp(nuevaPosicion.X, 0f, 800f - Tamano.X);
        nuevaPosicion.Y = Math.Clamp(nuevaPosicion.Y, 0f, 600f - Tamano.Y);
        Posicion = nuevaPosicion;
    }

    public Proyectil? IntentarDisparar(Vector2 direccionApuntado)
    {
        if (_tiempoDesdeUltimoDisparo >= CooldownDisparo)
        {
            _tiempoDesdeUltimoDisparo = 0f;
            Vector2 centroJugador = Posicion + Tamano / 2;

            // Spawnear proyectil desde el centro del jugador
            return new Proyectil(centroJugador, direccionApuntado, 500f);
        }
        return null;
    }
}

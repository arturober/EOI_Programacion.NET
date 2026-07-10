using Godot;
using System.Collections.Generic;

public partial class Juego : Node2D
{
    // Posición de la nave.
    private Vector2 nave = new Vector2(400, 550);

    // Posición de la bala.
    private Vector2 bala;

    // Indica si hay una bala en pantalla.
    private bool hayBala = false;

    // Posiciones de los alienígenas.
    private List<Vector2> aliens = new()
    {
        new Vector2(200, 100),
        new Vector2(300, 100),
        new Vector2(400, 100),
        new Vector2(500, 100),
        new Vector2(600, 100)
    };

    public override void _Process(double delta)
    {
        // Mover la nave hacia la izquierda.
        if (Input.IsActionPressed("ui_left"))
        {
            nave.X -= 300 * (float)delta;
        }

        // Mover la nave hacia la derecha.
        if (Input.IsActionPressed("ui_right"))
        {
            nave.X += 300 * (float)delta;
        }

        // Mover la bala hacia arriba.
        if (hayBala)
        {
            bala.Y -= 500 * (float)delta;

            ComprobarChoques();

            // La bala desaparece al salir de la pantalla.
            if (bala.Y < 0)
            {
                hayBala = false;
            }
        }

        // Volver a dibujar la pantalla.
        QueueRedraw();
    }

    public override void _Input(InputEvent evento)
    {
        // Disparar al pulsar la barra espaciadora.
        if (evento is InputEventKey tecla &&
            tecla.Pressed &&
            tecla.Keycode == Key.Space &&
            !hayBala)
        {
            bala = nave;
            hayBala = true;
        }
    }

    private void ComprobarChoques()
    {
        for (int i = aliens.Count - 1; i >= 0; i--)
        {
            // Si la bala está cerca de un alienígena, lo elimina.
            if (bala.DistanceTo(aliens[i]) < 25)
            {
                aliens.RemoveAt(i);
                hayBala = false;
                break;
            }
        }
    }

    public override void _Draw()
    {
        // Dibujar la nave.
        DrawRect(
            new Rect2(nave.X - 20, nave.Y - 10, 40, 20),
            Colors.Blue
        );

        // Dibujar la bala.
        if (hayBala)
        {
            DrawCircle(bala, 5, Colors.Yellow);
        }

        // Dibujar los alienígenas.
        foreach (Vector2 alien in aliens)
        {
            DrawCircle(alien, 20, Colors.Green);
        }

        // Mostrar un mensaje cuando no quedan alienígenas.
        if (aliens.Count == 0)
        {
            DrawString(
                ThemeDB.FallbackFont,
                new Vector2(320, 300),
                "¡HAS GANADO!",
                HorizontalAlignment.Left,
                -1,
                30,
                Colors.White
            );
        }
    }
}
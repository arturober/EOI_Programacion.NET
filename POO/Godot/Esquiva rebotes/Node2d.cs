using Godot;
using System.Collections.Generic;

// ESQUIVA LOS REBOTES
// Muévete con las flechas o con WASD.
// Cada cinco segundos aparece una bola nueva.
public partial class Node2d : Node2D
{
    // Cada bola guarda sus propios datos y sabe moverse.
    private class Bola
    {
        public Vector2 Posicion;
        public Vector2 Velocidad;
        public float Radio = 16;

        public void Mover(float segundos, Vector2 pantalla)
        {
            Posicion += Velocidad * segundos;

            // La bola rebota al tocar los bordes.
            if (Posicion.X < Radio || Posicion.X > pantalla.X - Radio)
                Velocidad.X *= -1;

            if (Posicion.Y < Radio || Posicion.Y > pantalla.Y - Radio)
                Velocidad.Y *= -1;
        }
    }

    private Vector2 jugador;
    private List<Bola> bolas = new();
    private RandomNumberGenerator azar = new();

    private float tiempo;
    private int puntos;
    private bool terminado;

    public override void _Ready()
    {
        azar.Randomize();
        Reiniciar();
    }

    public override void _Process(double delta)
    {
        float segundos = (float)delta;
        Vector2 pantalla = GetViewportRect().Size;

        if (!terminado)
        {
            MoverJugador(segundos, pantalla);

            foreach (Bola bola in bolas)
                bola.Mover(segundos, pantalla);

            // La puntuación representa los segundos sobrevividos.
            tiempo += segundos;
            puntos = (int)tiempo;

            // Cada cinco segundos se añade una bola.
            if (bolas.Count < 1 + puntos / 5)
                CrearBola(pantalla);

            // Comprobamos la distancia entre el jugador y cada bola.
            foreach (Bola bola in bolas)
                if (bola.Posicion.DistanceTo(jugador) < bola.Radio + 16)
                    terminado = true;
        }

        QueueRedraw();
    }

    public override void _Input(InputEvent evento)
    {
        if (evento is InputEventKey tecla && tecla.Pressed &&
            tecla.Keycode == Key.Space && terminado)
            Reiniciar();
    }

    private void MoverJugador(float segundos, Vector2 pantalla)
    {
        Vector2 direccion = Vector2.Zero;

        if (Input.IsPhysicalKeyPressed(Key.Left) ||
            Input.IsPhysicalKeyPressed(Key.A)) direccion.X--;

        if (Input.IsPhysicalKeyPressed(Key.Right) ||
            Input.IsPhysicalKeyPressed(Key.D)) direccion.X++;

        if (Input.IsPhysicalKeyPressed(Key.Up) ||
            Input.IsPhysicalKeyPressed(Key.W)) direccion.Y--;

        if (Input.IsPhysicalKeyPressed(Key.Down) ||
            Input.IsPhysicalKeyPressed(Key.S)) direccion.Y++;

        jugador += direccion.Normalized() * 300 * segundos;

        jugador.X = Mathf.Clamp(jugador.X, 16, pantalla.X - 16);
        jugador.Y = Mathf.Clamp(jugador.Y, 16, pantalla.Y - 16);
    }

    private void CrearBola(Vector2 pantalla)
    {
        Bola bola = new();

        bola.Posicion = new Vector2(
            30,
            azar.RandfRange(30, pantalla.Y - 30));

        bola.Velocidad = new Vector2(
            azar.RandfRange(160, 260),
            azar.RandfRange(160, 260));

        bolas.Add(bola);
    }

    private void Reiniciar()
    {
        jugador = GetViewportRect().Size / 2;
        bolas.Clear();
        tiempo = 0;
        terminado = false;
    }

    public override void _Draw()
    {
        DrawRect(
            new Rect2(Vector2.Zero, GetViewportRect().Size),
            Colors.Black);

        DrawRect(
            new Rect2(jugador - Vector2.One * 16, Vector2.One * 32),
            Colors.Cyan);

        foreach (Bola bola in bolas)
            DrawCircle(bola.Posicion, bola.Radio, Colors.OrangeRed);

        DrawString(
            ThemeDB.FallbackFont,
            new Vector2(15, 30),
            $"Tiempo: {puntos}",
            fontSize: 22);

        if (terminado)
            DrawString(
                ThemeDB.FallbackFont,
                new Vector2(15, 60),
                "Pulsa ESPACIO para reiniciar",
                fontSize: 22);
    }
}
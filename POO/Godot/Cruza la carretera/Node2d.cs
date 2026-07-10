using Godot;

// Cruza la carretera con dos coches.
// Añade este script a un único nodo Node2D.
public partial class Node2d : Node2D
{
    private class Jugador
    {
        public Vector2 Posicion;
        public Vector2 Tamano = new Vector2(35, 35);
        public float Velocidad = 260;

        public Rect2 Rectangulo()
        {
            return new Rect2(Posicion - Tamano / 2, Tamano);
        }
    }

    private class Coche
    {
        public Vector2 Posicion;
        public Vector2 Tamano = new Vector2(110, 45);
        public float Velocidad;

        public Rect2 Rectangulo()
        {
            return new Rect2(Posicion - Tamano / 2, Tamano);
        }
    }

    private Jugador jugador = new Jugador();
    private Coche coche1 = new Coche();
    private Coche coche2 = new Coche();
    private int puntos;

    public override void _Ready()
    {
        ReiniciarJuego();
    }

    public override void _Process(double delta)
    {
        float segundos = (float)delta;
        Vector2 pantalla = GetViewportRect().Size;

        MoverJugador(segundos, pantalla);
        MoverCoche(coche1, segundos, pantalla.X);
        MoverCoche(coche2, segundos, pantalla.X);
        ComprobarJuego();

        QueueRedraw();
    }

    private void MoverJugador(float segundos, Vector2 pantalla)
    {
        Vector2 direccion = Vector2.Zero;

        if (Input.IsPhysicalKeyPressed(Key.Left) ||
            Input.IsPhysicalKeyPressed(Key.A))
            direccion.X = -1;

        if (Input.IsPhysicalKeyPressed(Key.Right) ||
            Input.IsPhysicalKeyPressed(Key.D))
            direccion.X = 1;

        if (Input.IsPhysicalKeyPressed(Key.Up) ||
            Input.IsPhysicalKeyPressed(Key.W))
            direccion.Y = -1;

        if (Input.IsPhysicalKeyPressed(Key.Down) ||
            Input.IsPhysicalKeyPressed(Key.S))
            direccion.Y = 1;

        jugador.Posicion += direccion.Normalized()
            * jugador.Velocidad * segundos;

        jugador.Posicion.X = Mathf.Clamp(
            jugador.Posicion.X, 20, pantalla.X - 20);

        jugador.Posicion.Y = Mathf.Clamp(
            jugador.Posicion.Y, 20, pantalla.Y - 20);
    }

    // El mismo método sirve para mover los dos coches.
    private void MoverCoche(
        Coche coche,
        float segundos,
        float ancho)
    {
        coche.Posicion.X += coche.Velocidad * segundos;

        // Un coche circula hacia la derecha.
        if (coche.Velocidad > 0 &&
            coche.Posicion.X > ancho + 110)
        {
            coche.Posicion.X = -110;
        }

        // El otro coche circula hacia la izquierda.
        if (coche.Velocidad < 0 &&
            coche.Posicion.X < -110)
        {
            coche.Posicion.X = ancho + 110;
        }
    }

    private void ComprobarJuego()
    {
        bool choque =
            jugador.Rectangulo().Intersects(
                coche1.Rectangulo())
            ||
            jugador.Rectangulo().Intersects(
                coche2.Rectangulo());

        if (choque)
            ReiniciarJugador();

        // Al llegar a la meta, los coches aceleran.
        if (jugador.Posicion.Y < 30)
        {
            puntos++;

            coche1.Velocidad += 20;
            coche2.Velocidad -= 20;

            ReiniciarJugador();
        }
    }

    private void ReiniciarJuego()
    {
        Vector2 pantalla = GetViewportRect().Size;

        coche1.Velocidad = 260;
        coche2.Velocidad = -320;

        coche1.Posicion = new Vector2(
            -110,
            pantalla.Y / 2 - 35);

        coche2.Posicion = new Vector2(
            pantalla.X + 110,
            pantalla.Y / 2 + 35);

        ReiniciarJugador();
    }

    private void ReiniciarJugador()
    {
        Vector2 pantalla = GetViewportRect().Size;

        jugador.Posicion = new Vector2(
            pantalla.X / 2,
            pantalla.Y - 40);
    }

    public override void _Draw()
    {
        Vector2 pantalla = GetViewportRect().Size;

        // Fondo.
        DrawRect(
            new Rect2(Vector2.Zero, pantalla),
            new Color(0.1f, 0.45f, 0.18f));

        // Carretera.
        DrawRect(
            new Rect2(
                0,
                pantalla.Y / 2 - 75,
                pantalla.X,
                150),
            Colors.DarkSlateGray);

        // Separación entre los dos carriles.
        DrawLine(
            new Vector2(0, pantalla.Y / 2),
            new Vector2(pantalla.X, pantalla.Y / 2),
            Colors.White,
            4);

        // Personajes.
        DrawRect(
            jugador.Rectangulo(),
            Colors.Yellow);

        DrawRect(
            coche1.Rectangulo(),
            Colors.Red);

        DrawRect(
            coche2.Rectangulo(),
            Colors.Orange);

        // Meta.
        DrawLine(
            new Vector2(0, 25),
            new Vector2(pantalla.X, 25),
            Colors.White,
            5);

        // Marcador.
        DrawString(
            ThemeDB.FallbackFont,
            new Vector2(20, 55),
            $"Puntos: {puntos}",
            fontSize: 24);
    }
}
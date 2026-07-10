using Godot;

// Pong sencillo para Godot 4 con C#.
// Añade este script a un único nodo Node2D.
// No necesita imágenes, escenas adicionales ni acciones en Input Map.

public partial class Node2d : Node2D
{
    // Una paleta solo necesita posición, tamaño y su rectángulo de colisión.
    private class Paleta
    {
        public Vector2 Posicion;
        public Vector2 Tamano = new Vector2(20, 100);

        public Rect2 ObtenerRectangulo()
        {
            return new Rect2(Posicion - Tamano / 2, Tamano);
        }
    }

    // La pelota guarda su posición, velocidad y tamaño.
    private class Pelota
    {
        public Vector2 Posicion;
        public Vector2 Velocidad;
        public float Tamano = 18;

        public Rect2 ObtenerRectangulo()
        {
            Vector2 tamano = new Vector2(Tamano, Tamano);

            return new Rect2(
                Posicion - tamano / 2,
                tamano);
        }
    }

    private Paleta jugador = new Paleta();
    private Paleta cpu = new Paleta();
    private Pelota pelota = new Pelota();

    private int puntosJugador;
    private int puntosCpu;

    // Godot ejecuta _Ready una vez al comenzar.
    public override void _Ready()
    {
        ReiniciarPelota(-1);
    }

    // Godot ejecuta _Process continuamente.
    public override void _Process(double delta)
    {
        float segundos = (float)delta;
        Vector2 pantalla = GetViewportRect().Size;

        // Las paletas permanecen junto a los lados.
        jugador.Posicion.X = 40;
        cpu.Posicion.X = pantalla.X - 40;

        MoverJugador(segundos, pantalla);
        MoverCpu(segundos, pantalla);
        MoverPelota(segundos, pantalla);

        // Solicitamos que se dibujen las nuevas posiciones.
        QueueRedraw();
    }

    private void MoverJugador(float segundos, Vector2 pantalla)
    {
        float direccion = 0;

        if (Input.IsPhysicalKeyPressed(Key.W) ||
            Input.IsPhysicalKeyPressed(Key.Up))
        {
            direccion = -1;
        }

        if (Input.IsPhysicalKeyPressed(Key.S) ||
            Input.IsPhysicalKeyPressed(Key.Down))
        {
            direccion = 1;
        }

        jugador.Posicion.Y += direccion * 500 * segundos;

        LimitarPaleta(jugador, pantalla.Y);
    }

    private void MoverCpu(float segundos, Vector2 pantalla)
    {
        // La CPU intenta acercarse lentamente a la pelota.
        cpu.Posicion.Y = Mathf.MoveToward(
            cpu.Posicion.Y,
            pelota.Posicion.Y,
            280 * segundos);

        LimitarPaleta(cpu, pantalla.Y);
    }

    private void LimitarPaleta(
        Paleta paleta,
        float altoPantalla)
    {
        float mitad = paleta.Tamano.Y / 2;

        paleta.Posicion.Y = Mathf.Clamp(
            paleta.Posicion.Y,
            mitad,
            altoPantalla - mitad);
    }

    private void MoverPelota(
        float segundos,
        Vector2 pantalla)
    {
        pelota.Posicion += pelota.Velocidad * segundos;

        float mitad = pelota.Tamano / 2;

        // Rebote en la parte superior.
        if (pelota.Posicion.Y < mitad)
        {
            pelota.Posicion.Y = mitad;

            pelota.Velocidad.Y =
                Mathf.Abs(pelota.Velocidad.Y);
        }

        // Rebote en la parte inferior.
        if (pelota.Posicion.Y > pantalla.Y - mitad)
        {
            pelota.Posicion.Y = pantalla.Y - mitad;

            pelota.Velocidad.Y =
                -Mathf.Abs(pelota.Velocidad.Y);
        }

        // Rebote en la paleta del jugador.
        if (pelota.Velocidad.X < 0 &&
            pelota.ObtenerRectangulo().Intersects(
                jugador.ObtenerRectangulo()))
        {
            pelota.Posicion.X =
                jugador.Posicion.X
                + jugador.Tamano.X / 2
                + mitad;

            pelota.Velocidad.X =
                Mathf.Abs(pelota.Velocidad.X);
        }

        // Rebote en la paleta de la CPU.
        if (pelota.Velocidad.X > 0 &&
            pelota.ObtenerRectangulo().Intersects(
                cpu.ObtenerRectangulo()))
        {
            pelota.Posicion.X =
                cpu.Posicion.X
                - cpu.Tamano.X / 2
                - mitad;

            pelota.Velocidad.X =
                -Mathf.Abs(pelota.Velocidad.X);
        }

        // La CPU consigue un punto.
        if (pelota.Posicion.X < 0)
        {
            puntosCpu++;
            ReiniciarPelota(-1);
        }
        // El jugador consigue un punto.
        else if (pelota.Posicion.X > pantalla.X)
        {
            puntosJugador++;
            ReiniciarPelota(1);
        }
    }

    private void ReiniciarPelota(int direccion)
    {
        Vector2 pantalla = GetViewportRect().Size;

        jugador.Posicion = new Vector2(
            40,
            pantalla.Y / 2);

        cpu.Posicion = new Vector2(
            pantalla.X - 40,
            pantalla.Y / 2);

        pelota.Posicion = pantalla / 2;

        pelota.Velocidad = new Vector2(
            350 * direccion,
            220);
    }

    // Todos los elementos gráficos se dibujan mediante código.
    public override void _Draw()
    {
        Vector2 pantalla = GetViewportRect().Size;

        // Fondo negro.
        DrawRect(
            new Rect2(Vector2.Zero, pantalla),
            Colors.Black);

        // Línea central.
        DrawLine(
            new Vector2(pantalla.X / 2, 0),
            new Vector2(pantalla.X / 2, pantalla.Y),
            Colors.DimGray,
            4);

        // Paletas.
        DrawRect(
            jugador.ObtenerRectangulo(),
            Colors.White);

        DrawRect(
            cpu.ObtenerRectangulo(),
            Colors.White);

        // Pelota.
        DrawRect(
            pelota.ObtenerRectangulo(),
            Colors.Yellow);

        // Marcador.
        DrawString(
            ThemeDB.FallbackFont,
            new Vector2(pantalla.X / 2 - 45, 40),
            $"{puntosJugador}     {puntosCpu}",
            fontSize: 30);
    }
}
using Godot;
using System.Collections.Generic;

// RECOLECTOR FRENÉTICO
// Recoge monedas y evita enemigos cada vez más rápidos.
public partial class Node2d : Node2D
{
    private class Jugador
    {
        public Vector2 Posicion;
        public float Tamano = 34;
        public float Velocidad = 330;
        public Rect2 Rectangulo() =>
            new Rect2(Posicion - Vector2.One * Tamano / 2, Vector2.One * Tamano);
    }

    private class Enemigo
    {
        public Vector2 Posicion;
        public Vector2 Velocidad;
        public float Tamano = 30;
        public Rect2 Rectangulo() =>
            new Rect2(Posicion - Vector2.One * Tamano / 2, Vector2.One * Tamano);
        public void Mover(float segundos, Vector2 pantalla)
        {
            Posicion += Velocidad * segundos;
            if (Posicion.X < Tamano / 2 || Posicion.X > pantalla.X - Tamano / 2)
                Velocidad.X *= -1;
            if (Posicion.Y < Tamano / 2 || Posicion.Y > pantalla.Y - Tamano / 2)
                Velocidad.Y *= -1;
            Posicion.X = Mathf.Clamp(Posicion.X, Tamano / 2, pantalla.X - Tamano / 2);
            Posicion.Y = Mathf.Clamp(Posicion.Y, Tamano / 2, pantalla.Y - Tamano / 2);
        }
    }

    private Jugador jugador = new Jugador();
    private List<Enemigo> enemigos = new List<Enemigo>();
    private RandomNumberGenerator azar = new RandomNumberGenerator();
    private Vector2 moneda;
    private int puntos;
    private int record;
    private bool terminado;

    public override void _Ready()
    {
        azar.Randomize();
        Reiniciar();
    }
    public override void _Process(double delta)
    {
        if (terminado)
        {
            QueueRedraw();
            return;
        }
        float segundos = (float)delta;
        Vector2 pantalla = GetViewportRect().Size;
        MoverJugador(segundos, pantalla);
        foreach (Enemigo enemigo in enemigos)
            enemigo.Mover(segundos, pantalla);
        ComprobarColisiones();
        QueueRedraw();
    }
    public override void _Input(InputEvent evento)
    {
        if (evento is InputEventKey tecla && tecla.Pressed &&
            !tecla.Echo && tecla.Keycode == Key.Space && terminado)
            Reiniciar();
    }

    private void MoverJugador(float segundos, Vector2 pantalla)
    {
        Vector2 direccion = Vector2.Zero;
        if (Input.IsPhysicalKeyPressed(Key.Left) || Input.IsPhysicalKeyPressed(Key.A))
            direccion.X--;
        if (Input.IsPhysicalKeyPressed(Key.Right) || Input.IsPhysicalKeyPressed(Key.D))
            direccion.X++;
        if (Input.IsPhysicalKeyPressed(Key.Up) || Input.IsPhysicalKeyPressed(Key.W))
            direccion.Y--;
        if (Input.IsPhysicalKeyPressed(Key.Down) || Input.IsPhysicalKeyPressed(Key.S))
            direccion.Y++;
        jugador.Posicion += direccion.Normalized() * jugador.Velocidad * segundos;
        jugador.Posicion.X = Mathf.Clamp(jugador.Posicion.X, 17, pantalla.X - 17);
        jugador.Posicion.Y = Mathf.Clamp(jugador.Posicion.Y, 17, pantalla.Y - 17);
    }
    private void ComprobarColisiones()
    {
        Rect2 jugadorRect = jugador.Rectangulo();
        Rect2 monedaRect = new Rect2(moneda - Vector2.One * 12, Vector2.One * 24);
        if (jugadorRect.Intersects(monedaRect))
        {
            puntos++;
            if (puntos > record)
                record = puntos;
            ColocarMoneda();
            foreach (Enemigo enemigo in enemigos)
                enemigo.Velocidad *= 1.05f;
            if (puntos % 5 == 0)
                CrearEnemigo();
        }
        foreach (Enemigo enemigo in enemigos)
            if (jugadorRect.Intersects(enemigo.Rectangulo()))
                terminado = true;
    }

    private void Reiniciar()
    {
        puntos = 0;
        terminado = false;
        enemigos.Clear();
        jugador.Posicion = GetViewportRect().Size / 2;
        CrearEnemigo();
        ColocarMoneda();
    }
    private void CrearEnemigo()
    {
        Enemigo enemigo = new Enemigo();
        enemigo.Posicion = new Vector2(40, 40);
        enemigo.Velocidad = new Vector2(
            azar.RandfRange(180, 260),
            azar.RandfRange(180, 260));
        if (azar.RandiRange(0, 1) == 0)
            enemigo.Velocidad.X *= -1;
        enemigos.Add(enemigo);
    }
    private void ColocarMoneda()
    {
        Vector2 pantalla = GetViewportRect().Size;
        moneda = new Vector2(
            azar.RandfRange(35, pantalla.X - 35),
            azar.RandfRange(70, pantalla.Y - 35));
    }

    public override void _Draw()
    {
        Vector2 pantalla = GetViewportRect().Size;
        DrawRect(new Rect2(Vector2.Zero, pantalla), new Color(0.05f, 0.08f, 0.15f));
        DrawCircle(moneda, 12, Colors.Gold);
        DrawRect(jugador.Rectangulo(), Colors.DeepSkyBlue);
        foreach (Enemigo enemigo in enemigos)
            DrawRect(enemigo.Rectangulo(), Colors.OrangeRed);
        DrawString(ThemeDB.FallbackFont, new Vector2(20, 35),
            $"Puntos: {puntos}   Récord: {record}", fontSize: 24);
        if (terminado)
            DrawString(ThemeDB.FallbackFont,
                new Vector2(pantalla.X / 2 - 155, pantalla.Y / 2),
                "Pulsa ESPACIO para volver a jugar", fontSize: 22);
    }
}
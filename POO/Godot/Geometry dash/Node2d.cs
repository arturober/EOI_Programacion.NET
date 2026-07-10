using Godot;

// Geometry Dash sencillo para un único nodo Node2D.
// ESPACIO/W/ARRIBA salta. A/D o flechas mueve.
public partial class Node2d : Node2D
{
    private class Jugador
    {
        public Vector2 Posicion;
        public Vector2 Tamano = new Vector2(42, 42);
        public float VelocidadY;
        public float Angulo;

        public Rect2 Rectangulo()
        {
            Vector2 margen = new Vector2(5, 5);

            return new Rect2(
                Posicion - Tamano / 2 + margen,
                Tamano - margen * 2);
        }
    }

    private class Obstaculo
    {
        public Vector2 Posicion;
        public int Pinchos = 1;

        public Rect2 Rectangulo()
        {
            float ancho = Pinchos * 36;

            return new Rect2(
                Posicion.X - ancho / 2 + 6,
                Posicion.Y - 42,
                ancho - 12,
                37);
        }
    }

    private Jugador jugador = new Jugador();

    private Obstaculo obstaculo1 = new Obstaculo();
    private Obstaculo obstaculo2 = new Obstaculo();

    private int puntos;
    private bool terminado;
    private bool enElSuelo;

    public override void _Ready()
    {
        Reiniciar();
    }

    public override void _Process(double delta)
    {
        float segundos = (float)delta;
        Vector2 pantalla = GetViewportRect().Size;
        float suelo = pantalla.Y - 80;

        if (!terminado)
        {
            MoverJugador(
                segundos,
                pantalla,
                suelo);

            MoverObstaculo(
                obstaculo1,
                obstaculo2,
                segundos,
                pantalla.X,
                suelo);

            MoverObstaculo(
                obstaculo2,
                obstaculo1,
                segundos,
                pantalla.X,
                suelo);

            terminado =
                jugador.Rectangulo().Intersects(
                    obstaculo1.Rectangulo())
                ||
                jugador.Rectangulo().Intersects(
                    obstaculo2.Rectangulo());
        }

        QueueRedraw();
    }

    public override void _Input(InputEvent evento)
    {
        if (evento is not InputEventKey tecla ||
            !tecla.Pressed ||
            tecla.Echo)
        {
            return;
        }

        bool saltar =
            tecla.Keycode == Key.Space ||
            tecla.Keycode == Key.W ||
            tecla.Keycode == Key.Up;

        if (saltar && terminado)
        {
            Reiniciar();
        }
        else if (saltar && enElSuelo)
        {
            jugador.VelocidadY = -650;
            enElSuelo = false;
        }
    }

    private void MoverJugador(
        float segundos,
        Vector2 pantalla,
        float suelo)
    {
        float direccion = 0;

        if (Input.IsPhysicalKeyPressed(Key.A) ||
            Input.IsPhysicalKeyPressed(Key.Left))
        {
            direccion = -1;
        }

        if (Input.IsPhysicalKeyPressed(Key.D) ||
            Input.IsPhysicalKeyPressed(Key.Right))
        {
            direccion = 1;
        }

        jugador.Posicion.X += direccion * 280 * segundos;

        jugador.Posicion.X = Mathf.Clamp(
            jugador.Posicion.X,
            30,
            pantalla.X * 0.55f);

        // Aplicamos gravedad.
        jugador.VelocidadY += 1700 * segundos;

        jugador.Posicion.Y += jugador.VelocidadY * segundos;

        // El cuadrado gira mientras está en el aire.
        if (!enElSuelo)
        {
            jugador.Angulo += 5 * segundos;
        }

        float limite = suelo - jugador.Tamano.Y / 2;

        // Colocamos al jugador sobre el suelo.
        if (jugador.Posicion.Y >= limite)
        {
            jugador.Posicion.Y = limite;
            jugador.VelocidadY = 0;
            jugador.Angulo = 0;
            enElSuelo = true;
        }
    }

    private void MoverObstaculo(
        Obstaculo obstaculo,
        Obstaculo otro,
        float segundos,
        float anchoPantalla,
        float suelo)
    {
        float velocidad = 320 + puntos * 8;

        obstaculo.Posicion.X -= velocidad * segundos;

        obstaculo.Posicion.Y = suelo;

        // Cuando sale, reaparece detrás del otro.
        if (obstaculo.Posicion.X < -100)
        {
            puntos++;

            // Alternamos entre uno y dos pinchos.
            if (obstaculo.Pinchos == 1)
            {
                obstaculo.Pinchos = 2;
            }
            else
            {
                obstaculo.Pinchos = 1;
            }

            obstaculo.Posicion.X = Mathf.Max(
                anchoPantalla + 100,
                otro.Posicion.X + 320);
        }
    }

    private void Reiniciar()
    {
        Vector2 pantalla = GetViewportRect().Size;

        float suelo = pantalla.Y - 80;

        puntos = 0;
        terminado = false;
        enElSuelo = true;

        jugador.Posicion = new Vector2(
            pantalla.X * 0.2f,
            suelo - jugador.Tamano.Y / 2);

        jugador.VelocidadY = 0;
        jugador.Angulo = 0;

        obstaculo1.Posicion = new Vector2(
            pantalla.X + 200,
            suelo);

        obstaculo2.Posicion = new Vector2(
            pantalla.X + 600,
            suelo);

        obstaculo1.Pinchos = 1;
        obstaculo2.Pinchos = 2;
    }

    public override void _Draw()
    {
        Vector2 pantalla = GetViewportRect().Size;

        float suelo = pantalla.Y - 80;

        // Fondo.
        DrawRect(
            new Rect2(Vector2.Zero, pantalla),
            new Color(0.08f, 0.12f, 0.24f));

        // Suelo.
        DrawRect(
            new Rect2(0, suelo, pantalla.X, 80),
            new Color(0.15f, 0.55f, 0.75f));

        // Movemos el origen al jugador y giramos el dibujo.
        DrawSetTransform(
            jugador.Posicion,
            jugador.Angulo,
            Vector2.One);

        DrawRect(
            new Rect2(
                -jugador.Tamano / 2,
                jugador.Tamano),
            terminado
                ? Colors.Red
                : Colors.Yellow);

        // Recuperamos las coordenadas normales.
        DrawSetTransform(Vector2.Zero, 0, Vector2.One);

        DibujarPinchos(obstaculo1, suelo);

        DibujarPinchos(obstaculo2, suelo);

        DrawString(
            ThemeDB.FallbackFont,
            new Vector2(20, 35),
            $"Puntos: {puntos}",
            HorizontalAlignment.Left,
            -1,
            24);
    }

    private void DibujarPinchos(
        Obstaculo obstaculo,
        float suelo)
    {
        for (int i = 0;
            i < obstaculo.Pinchos;
            i++)
        {
            float x = obstaculo.Posicion.X + (i - (obstaculo.Pinchos - 1) / 2f) * 36;

            Vector2[] triangulo =
            {
                new Vector2(x, suelo - 50),

                new Vector2(x - 18, suelo),

                new Vector2(x + 18, suelo)
            };

            DrawColoredPolygon(triangulo, Colors.Red);
        }
    }
}
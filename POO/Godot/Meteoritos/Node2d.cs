using Godot;
using System.Collections.Generic;

// ============================================================================
// MINIJUEGO: ESQUIVA METEORITOS CON DISPAROS
// ============================================================================
//
// Controles:
// - Flechas izquierda/derecha o teclas A/D: mover la nave.
// - Espacio: disparar.
// - Espacio después de perder: volver a jugar.
// - F11: cambiar entre ventana y pantalla completa.
// - Escape: cerrar el juego.
//
// Todo el juego está en este único fichero y todos los dibujos se crean
// mediante código. No necesita imágenes, sonidos ni otros recursos externos.
// ============================================================================

public partial class Node2d : Node2D
{
    // El juego utiliza siempre estas coordenadas internas.
    // Después, el dibujo se escala para adaptarlo a la ventana real.
    private const float AnchoJuego = 1280f;
    private const float AltoJuego = 720f;

    // ------------------------------------------------------------------------
    // CLASE JUGADOR
    // ------------------------------------------------------------------------
    // Guarda los datos y el comportamiento básico de la nave.
    private class Jugador
    {
        public Vector2 Posicion;
        public float Ancho = 74f;
        public float Alto = 34f;
        public float Velocidad = 620f;

        // Devuelve el rectángulo que ocupa la nave.
        public Rect2 ObtenerRectangulo()
        {
            return new Rect2(
                Posicion.X - Ancho / 2f,
                Posicion.Y - Alto / 2f,
                Ancho,
                Alto);
        }

        // Mueve la nave y evita que salga de la pantalla.
        public void Mover(float direccion, float segundos)
        {
            Posicion.X += direccion * Velocidad * segundos;

            float limiteIzquierdo = Ancho / 2f;
            float limiteDerecho = AnchoJuego - Ancho / 2f;

            Posicion.X = Mathf.Clamp(Posicion.X, limiteIzquierdo, limiteDerecho);
        }
    }

    // ------------------------------------------------------------------------
    // CLASE METEORITO
    // ------------------------------------------------------------------------
    // Cada meteorito tiene una posición, un tamaño y una velocidad propios.
    private class Meteorito
    {
        public Vector2 Posicion;
        public float Radio;
        public float Velocidad;

        public void Mover(float segundos)
        {
            Posicion.Y += Velocidad * segundos;
        }
    }

    // ------------------------------------------------------------------------
    // CLASE BALA
    // ------------------------------------------------------------------------
    // La bala es solamente un pequeño rectángulo que se mueve hacia arriba.
    private class Bala
    {
        public Vector2 Posicion;
        public float Ancho = 6f;
        public float Alto = 20f;
        public float Velocidad = 850f;

        public void Mover(float segundos)
        {
            Posicion.Y -= Velocidad * segundos;
        }

        // Devuelve el rectángulo ocupado por la bala.
        public Rect2 ObtenerRectangulo()
        {
            return new Rect2(
                Posicion.X - Ancho / 2f,
                Posicion.Y - Alto / 2f,
                Ancho,
                Alto);
        }
    }

    // Objetos principales del juego.
    private Jugador jugador = new Jugador();
    private List<Meteorito> meteoritos = new List<Meteorito>();
    private List<Bala> balas = new List<Bala>();
    private RandomNumberGenerator azar = new RandomNumberGenerator();

    // Variables que controlan la partida.
    private float tiempoHastaSiguienteMeteorito;
    private float tiempoInvulnerable;
    private int puntos;
    private int record;
    private int vidas;
    private bool juegoTerminado;

    public override void _Ready()
    {
        azar.Randomize();
        ReiniciarJuego();
    }

    public override void _Process(double delta)
    {
        float segundos = (float)delta;

        // Si la partida ha terminado, solo necesitamos redibujar la pantalla.
        if (juegoTerminado)
        {
            QueueRedraw();
            return;
        }

        MoverJugador(segundos);
        CrearMeteoritos(segundos);
        MoverMeteoritos(segundos);
        MoverBalas(segundos);
        ComprobarImpactosDeBalas();
        ComprobarGolpesAlJugador();

        // Reduce poco a poco el tiempo de invulnerabilidad después de un golpe.
        if (tiempoInvulnerable > 0f)
        {
            tiempoInvulnerable -= segundos;
        }

        QueueRedraw();
    }

    public override void _Input(InputEvent evento)
    {
        InputEventKey tecla = evento as InputEventKey;

        // Ignoramos los eventos que no sean pulsaciones de teclado.
        if (tecla == null || !tecla.Pressed || tecla.IsEcho())
        {
            return;
        }

        if (tecla.Keycode == Key.Space)
        {
            if (juegoTerminado)
            {
                ReiniciarJuego();
            }
            else
            {
                CrearBala();
            }
        }
        else if (tecla.Keycode == Key.F11)
        {
            CambiarPantallaCompleta();
        }
        else if (tecla.Keycode == Key.Escape)
        {
            GetTree().Quit();
        }
    }

    // Lee el teclado y mueve al jugador.
    private void MoverJugador(float segundos)
    {
        float direccion = 0f;

        if (Input.IsPhysicalKeyPressed(Key.Left) ||
            Input.IsPhysicalKeyPressed(Key.A))
        {
            direccion -= 1f;
        }

        if (Input.IsPhysicalKeyPressed(Key.Right) ||
            Input.IsPhysicalKeyPressed(Key.D))
        {
            direccion += 1f;
        }

        jugador.Mover(direccion, segundos);
    }

    // Crea una bala justo encima de la nave.
    private void CrearBala()
    {
        Bala nuevaBala = new Bala();

        nuevaBala.Posicion = jugador.Posicion + new Vector2(0f, -30f);

        balas.Add(nuevaBala);
    }

    // Crea meteoritos periódicamente.
    private void CrearMeteoritos(float segundos)
    {
        tiempoHastaSiguienteMeteorito -= segundos;

        if (tiempoHastaSiguienteMeteorito > 0f)
        {
            return;
        }

        float radio = azar.RandfRange(18f, 36f);

        Meteorito nuevoMeteorito = new Meteorito();

        nuevoMeteorito.Radio = radio;
        nuevoMeteorito.Posicion = new Vector2(
            azar.RandfRange(radio, AnchoJuego - radio),
            -radio);
        nuevoMeteorito.Velocidad =
            azar.RandfRange(240f, 360f) + puntos * 3f;

        meteoritos.Add(nuevoMeteorito);

        // Los meteoritos aparecen cada vez más deprisa.
        tiempoHastaSiguienteMeteorito = Mathf.Max(0.24f, 0.85f - puntos * 0.015f);
    }

    // Mueve los meteoritos y elimina los que salen de la pantalla.
    private void MoverMeteoritos(float segundos)
    {
        // Recorremos la lista de atrás hacia delante porque podemos borrar elementos.
        for (int i = meteoritos.Count - 1; i >= 0; i--)
        {
            Meteorito meteorito = meteoritos[i];
            meteorito.Mover(segundos);

            if (meteorito.Posicion.Y - meteorito.Radio > AltoJuego)
            {
                meteoritos.RemoveAt(i);
                SumarPunto();
            }
        }
    }

    // Mueve las balas y elimina las que salen por la parte superior.
    private void MoverBalas(float segundos)
    {
        for (int i = balas.Count - 1; i >= 0; i--)
        {
            Bala bala = balas[i];
            bala.Mover(segundos);

            if (bala.Posicion.Y + bala.Alto / 2f < 0f)
            {
                balas.RemoveAt(i);
            }
        }
    }

    // Comprueba si alguna bala ha tocado un meteorito.
    private void ComprobarImpactosDeBalas()
    {
        // Recorremos primero todas las balas.
        for (int i = balas.Count - 1; i >= 0; i--)
        {
            Bala bala = balas[i];
            Rect2 rectanguloBala = bala.ObtenerRectangulo();

            // Para cada bala, comprobamos todos los meteoritos.
            for (int j = meteoritos.Count - 1; j >= 0; j--)
            {
                Meteorito meteorito = meteoritos[j];

                if (CirculoTocaRectangulo(
                    meteorito.Posicion,
                    meteorito.Radio,
                    rectanguloBala))
                {
                    // Al impactar desaparecen tanto la bala como el meteorito.
                    balas.RemoveAt(i);
                    meteoritos.RemoveAt(j);
                    SumarPunto();

                    // Esta bala ya ha desaparecido, así que dejamos de buscar.
                    break;
                }
            }
        }
    }

    // Comprueba si algún meteorito toca la nave.
    private void ComprobarGolpesAlJugador()
    {
        // Durante un instante después de recibir un golpe no puede recibir otro.
        if (tiempoInvulnerable > 0f)
        {
            return;
        }

        Rect2 rectanguloJugador = jugador.ObtenerRectangulo();

        for (int i = meteoritos.Count - 1; i >= 0; i--)
        {
            Meteorito meteorito = meteoritos[i];

            if (CirculoTocaRectangulo(
                meteorito.Posicion,
                meteorito.Radio,
                rectanguloJugador))
            {
                meteoritos.RemoveAt(i);
                vidas--;
                tiempoInvulnerable = 1f;

                if (vidas <= 0)
                {
                    juegoTerminado = true;
                }

                // Solo procesamos un golpe en este fotograma.
                return;
            }
        }
    }

    // Añade un punto y actualiza el récord cuando sea necesario.
    private void SumarPunto()
    {
        puntos++;

        if (puntos > record)
        {
            record = puntos;
        }
    }

    // Colisión sencilla entre un círculo y un rectángulo.
    // Se utiliza tanto para la nave como para las balas.
    private static bool CirculoTocaRectangulo(
        Vector2 centro,
        float radio,
        Rect2 rectangulo)
    {
        float izquierda = rectangulo.Position.X;
        float derecha = rectangulo.Position.X + rectangulo.Size.X;
        float arriba = rectangulo.Position.Y;
        float abajo = rectangulo.Position.Y + rectangulo.Size.Y;

        // Busca el punto del rectángulo que queda más cerca del círculo.
        float puntoX = Mathf.Clamp(centro.X, izquierda, derecha);
        float puntoY = Mathf.Clamp(centro.Y, arriba, abajo);

        Vector2 puntoMasCercano = new Vector2(puntoX, puntoY);
        float distanciaAlCuadrado = centro.DistanceSquaredTo(puntoMasCercano);

        return distanciaAlCuadrado <= radio * radio;
    }

    // Coloca todas las variables en su estado inicial.
    private void ReiniciarJuego()
    {
        meteoritos.Clear();
        balas.Clear();

        jugador.Posicion = new Vector2(AnchoJuego / 2f, AltoJuego - 70f);

        puntos = 0;
        vidas = 3;
        tiempoInvulnerable = 0f;
        tiempoHastaSiguienteMeteorito = 0.4f;
        juegoTerminado = false;

        QueueRedraw();
    }

    // Cambia entre modo ventana y pantalla completa al pulsar F11.
    private static void CambiarPantallaCompleta()
    {
        bool estaEnPantallaCompleta =
            DisplayServer.WindowGetMode() == DisplayServer.WindowMode.ExclusiveFullscreen;

        if (estaEnPantallaCompleta)
        {
            DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
        }
        else
        {
            DisplayServer.WindowSetMode(
                DisplayServer.WindowMode.ExclusiveFullscreen);
        }
    }

    // ------------------------------------------------------------------------
    // DIBUJO
    // ------------------------------------------------------------------------
    public override void _Draw()
    {
        Vector2 tamanoPantalla = GetViewportRect().Size;

        // Pinta de negro toda la ventana, incluidas las posibles bandas laterales.
        DrawRect(
            new Rect2(Vector2.Zero, tamanoPantalla),
            Colors.Black);

        // Calcula una escala uniforme para conservar la proporción 16:9.
        float escala = Mathf.Min(
            tamanoPantalla.X / AnchoJuego,
            tamanoPantalla.Y / AltoJuego);

        Vector2 tamanoEscalado = new Vector2(AnchoJuego, AltoJuego) * escala;
        Vector2 margen = (tamanoPantalla - tamanoEscalado) / 2f;

        // Todo lo que se dibuje después utilizará las coordenadas internas
        // de 1280 x 720 y se adaptará automáticamente a la ventana real.
        DrawSetTransform(margen, 0f, Vector2.One * escala);

        DibujarFondo();

        foreach (Bala bala in balas)
        {
            DibujarBala(bala);
        }

        foreach (Meteorito meteorito in meteoritos)
        {
            DibujarMeteorito(meteorito);
        }

        // Durante la invulnerabilidad hacemos parpadear la nave.
        bool dibujarNave = true;

        if (tiempoInvulnerable > 0f && Time.GetTicksMsec() % 200 >= 100)
        {
            dibujarNave = false;
        }

        if (dibujarNave)
        {
            DibujarNave();
        }

        DibujarMarcador();

        if (juegoTerminado)
        {
            DibujarPantallaFinal();
        }

        // Recupera el sistema normal de coordenadas.
        DrawSetTransform(Vector2.Zero, 0f, Vector2.One);
    }

    private void DibujarFondo()
    {
        DrawRect(
            new Rect2(0f, 0f, AnchoJuego, AltoJuego),
            new Color(0.03f, 0.05f, 0.12f));

        // Crea un pequeño campo de estrellas mediante una fórmula sencilla.
        for (int i = 0; i < 55; i++)
        {
            float x = (i * 97f + 43f) % AnchoJuego;
            float y = (i * 53f + 29f) % AltoJuego;
            float radio = 1f;

            if (i % 3 == 0)
            {
                radio = 2f;
            }

            DrawCircle(new Vector2(x, y), radio, new Color(0.75f, 0.82f, 1f));
        }
    }

    private void DibujarNave()
    {
        Vector2 centro = jugador.Posicion;
        Color colorNave = new Color(0.2f, 0.8f, 1f);

        // Cuerpo de la nave.
        DrawRect(
            new Rect2(centro.X - 24f, centro.Y - 15f, 48f, 30f),
            colorNave);

        // Alas.
        DrawRect(
            new Rect2(centro.X - 37f, centro.Y + 5f, 20f, 10f),
            colorNave);
        DrawRect(
            new Rect2(centro.X + 17f, centro.Y + 5f, 20f, 10f),
            colorNave);

        // Cabina.
        DrawCircle(
            centro + new Vector2(0f, -8f),
            9f,
            new Color(0.85f, 0.95f, 1f));

        // Llamas de los motores.
        DrawLine(
            centro + new Vector2(-12f, 16f),
            centro + new Vector2(-12f, 28f),
            Colors.Orange,
            5f);
        DrawLine(
            centro + new Vector2(12f, 16f),
            centro + new Vector2(12f, 28f),
            Colors.Orange,
            5f);
    }

    // La bala se dibuja como un rectángulo amarillo muy sencillo.
    private void DibujarBala(Bala bala)
    {
        DrawRect(
            bala.ObtenerRectangulo(),
            Colors.Yellow);
    }

    private void DibujarMeteorito(Meteorito meteorito)
    {
        DrawCircle(
            meteorito.Posicion,
            meteorito.Radio,
            new Color(0.55f, 0.34f, 0.2f));

        // Dos cráteres sencillos para que el meteorito resulte reconocible.
        DrawCircle(
            meteorito.Posicion + new Vector2(
                -meteorito.Radio * 0.3f,
                -meteorito.Radio * 0.2f),
            meteorito.Radio * 0.22f,
            new Color(0.35f, 0.2f, 0.12f));

        DrawCircle(
            meteorito.Posicion + new Vector2(
                meteorito.Radio * 0.25f,
                meteorito.Radio * 0.25f),
            meteorito.Radio * 0.15f,
            new Color(0.35f, 0.2f, 0.12f));
    }

    private void DibujarMarcador()
    {
        DrawString(
            ThemeDB.FallbackFont,
            new Vector2(28f, 42f),
            $"Puntos: {puntos}    Vidas: {vidas}    Récord: {record}",
            HorizontalAlignment.Left,
            -1f,
            26,
            Colors.White);

        DrawString(
            ThemeDB.FallbackFont,
            new Vector2(28f, 78f),
            "Mover: flechas o A/D    Disparar: ESPACIO    F11: pantalla completa",
            HorizontalAlignment.Left,
            -1f,
            18,
            new Color(0.75f, 0.82f, 0.95f));
    }

    private void DibujarPantallaFinal()
    {
        DrawRect(
            new Rect2(270f, 220f, 740f, 260f),
            new Color(0f, 0f, 0f, 0.82f));

        DibujarTextoCentrado("FIN DE LA PARTIDA", 300f, 48, Colors.OrangeRed);
        DibujarTextoCentrado($"Puntuación: {puntos}", 365f, 32, Colors.White);
        DibujarTextoCentrado("Pulsa ESPACIO para volver a jugar", 430f, 25, Colors.White);
    }

    private void DibujarTextoCentrado(string texto, float y, int tamano, Color color)
    {
        DrawString(
            ThemeDB.FallbackFont,
            new Vector2(0f, y),
            texto,
            HorizontalAlignment.Center,
            AnchoJuego,
            tamano,
            color);
    }
}

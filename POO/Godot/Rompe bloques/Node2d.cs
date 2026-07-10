using Godot;
using System.Collections.Generic;

// ============================================================================
// MINIJUEGO 2: ROMPEBLOQUES
// ============================================================================
//
// Instrucciones:
// - Flechas izquierda/derecha o teclas A/D: mover la pala.
// - Espacio: volver a jugar después de ganar o perder.
// - F11: cambiar entre ventana y pantalla completa.
// - Escape: cerrar el juego.
//
// Todo el juego está en este único fichero y se dibuja mediante código.
// No utiliza imágenes, sonidos ni escenas adicionales.
// ============================================================================

public partial class Node2d : Node2D
{
    // Resolución lógica interna. El dibujo se escala después para conservar
    // correctamente su proporción en cualquier tamaño de ventana.
    private const float AnchoJuego = 1280f;
    private const float AltoJuego = 720f;

    // ------------------------------------------------------------------------
    // CLASE PALA
    // ------------------------------------------------------------------------
    private class Pala
    {
        public Vector2 Posicion;
        public float Ancho = 180f;
        public float Alto = 24f;
        public float Velocidad = 760f;

        public Rect2 ObtenerRectangulo()
        {
            return new Rect2(
                Posicion.X - Ancho / 2f,
                Posicion.Y - Alto / 2f,
                Ancho,
                Alto);
        }

        public void Mover(float direccion, float segundos)
        {
            Posicion.X += direccion * Velocidad * segundos;

            float limiteIzquierdo = Ancho / 2f;
            float limiteDerecho = AnchoJuego - Ancho / 2f;

            Posicion.X = Mathf.Clamp(Posicion.X, limiteIzquierdo, limiteDerecho);
        }
    }

    // ------------------------------------------------------------------------
    // CLASE PELOTA
    // ------------------------------------------------------------------------
    private class Pelota
    {
        public Vector2 Posicion;
        public Vector2 Velocidad;
        public float Radio = 13f;

        public void Mover(float segundos)
        {
            Posicion += Velocidad * segundos;
        }
    }

    // ------------------------------------------------------------------------
    // CLASE BLOQUE
    // ------------------------------------------------------------------------
    private class Bloque
    {
        public Rect2 Rectangulo;
        public Color Color;
    }

    private Pala pala = new Pala();
    private Pelota pelota = new Pelota();
    private List<Bloque> bloques = new List<Bloque>();
    private RandomNumberGenerator azar = new RandomNumberGenerator();

    private int puntos;
    private int vidas;
    private bool partidaTerminada;
    private bool victoria;
    private float tiempoAntesDelSaque;

    public override void _Ready()
    {
        azar.Randomize();
        ReiniciarPartida();
    }

    public override void _Process(double delta)
    {
        float segundos = (float)delta;

        if (partidaTerminada)
        {
            QueueRedraw();
            return;
        }

        MoverPala(segundos);

        // Después de perder una vida, dejamos una pequeña pausa antes del saque.
        if (tiempoAntesDelSaque > 0f)
        {
            tiempoAntesDelSaque -= segundos;
            pelota.Posicion = pala.Posicion + new Vector2(0f, -35f);
        }
        else
        {
            pelota.Mover(segundos);
            ComprobarParedes();
            ComprobarChoqueConPala();
            ComprobarChoqueConBloques();
            ComprobarPelotaPerdida();
        }

        QueueRedraw();
    }

    public override void _Input(InputEvent evento)
    {
        InputEventKey tecla = evento as InputEventKey;

        if (tecla == null || !tecla.Pressed || tecla.IsEcho())
        {
            return;
        }

        if (tecla.Keycode == Key.Space && partidaTerminada)
        {
            ReiniciarPartida();
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

    private void MoverPala(float segundos)
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

        pala.Mover(direccion, segundos);
    }

    private void ComprobarParedes()
    {
        // Pared izquierda.
        if (pelota.Posicion.X - pelota.Radio <= 0f)
        {
            pelota.Posicion.X = pelota.Radio;
            pelota.Velocidad.X = Mathf.Abs(pelota.Velocidad.X);
        }

        // Pared derecha.
        if (pelota.Posicion.X + pelota.Radio >= AnchoJuego)
        {
            pelota.Posicion.X = AnchoJuego - pelota.Radio;
            pelota.Velocidad.X = -Mathf.Abs(pelota.Velocidad.X);
        }

        // Techo.
        if (pelota.Posicion.Y - pelota.Radio <= 0f)
        {
            pelota.Posicion.Y = pelota.Radio;
            pelota.Velocidad.Y = Mathf.Abs(pelota.Velocidad.Y);
        }
    }

    private void ComprobarChoqueConPala()
    {
        Rect2 rectanguloPala = pala.ObtenerRectangulo();

        // Solo comprobamos el choque cuando la pelota baja.
        if (pelota.Velocidad.Y <= 0f ||
            !CirculoTocaRectangulo(pelota.Posicion, pelota.Radio, rectanguloPala))
        {
            return;
        }

        // Coloca la pelota justo encima de la pala para evitar que quede atrapada.
        pelota.Posicion.Y = rectanguloPala.Position.Y - pelota.Radio;

        // El punto de impacto modifica la dirección del rebote.
        // Golpear cerca de los extremos produce un rebote más diagonal.
        float distanciaAlCentro = pelota.Posicion.X - pala.Posicion.X;
        float golpe = distanciaAlCentro / (pala.Ancho / 2f);

        float rapidez = pelota.Velocidad.Length();
        Vector2 nuevaDireccion = new Vector2(golpe, -1f).Normalized();

        pelota.Velocidad = nuevaDireccion * rapidez;
    }

    private void ComprobarChoqueConBloques()
    {
        for (int i = 0; i < bloques.Count; i++)
        {
            Bloque bloque = bloques[i];

            if (!CirculoTocaRectangulo(
                pelota.Posicion,
                pelota.Radio,
                bloque.Rectangulo))
            {
                continue;
            }

            bloques.RemoveAt(i);
            puntos += 10;

            // Rebote sencillo y fácil de entender.
            pelota.Velocidad.Y *= -1f;

            // La pelota aumenta ligeramente su velocidad con cada bloque.
            float nuevaRapidez = Mathf.Min(760f, pelota.Velocidad.Length() + 7f);
            pelota.Velocidad = pelota.Velocidad.Normalized() * nuevaRapidez;

            if (bloques.Count == 0)
            {
                partidaTerminada = true;
                victoria = true;
            }

            // Solo rompemos un bloque en cada fotograma.
            return;
        }
    }

    private void ComprobarPelotaPerdida()
    {
        if (pelota.Posicion.Y - pelota.Radio <= AltoJuego)
        {
            return;
        }

        vidas--;

        if (vidas <= 0)
        {
            partidaTerminada = true;
            victoria = false;
        }
        else
        {
            PrepararNuevoSaque();
        }
    }

    private static bool CirculoTocaRectangulo(
        Vector2 centro,
        float radio,
        Rect2 rectangulo)
    {
        float izquierda = rectangulo.Position.X;
        float derecha = rectangulo.Position.X + rectangulo.Size.X;
        float arriba = rectangulo.Position.Y;
        float abajo = rectangulo.Position.Y + rectangulo.Size.Y;

        float puntoX = Mathf.Clamp(centro.X, izquierda, derecha);
        float puntoY = Mathf.Clamp(centro.Y, arriba, abajo);

        Vector2 puntoMasCercano = new Vector2(puntoX, puntoY);

        return centro.DistanceSquaredTo(puntoMasCercano) <= radio * radio;
    }

    private void ReiniciarPartida()
    {
        puntos = 0;
        vidas = 3;
        partidaTerminada = false;
        victoria = false;

        pala.Posicion = new Vector2(AnchoJuego / 2f, AltoJuego - 48f);

        CrearBloques();
        PrepararNuevoSaque();

        QueueRedraw();
    }

    private void PrepararNuevoSaque()
    {
        pala.Posicion = new Vector2(AnchoJuego / 2f, AltoJuego - 48f);
        pelota.Posicion = pala.Posicion + new Vector2(0f, -35f);

        // El signo aleatorio hace que unas veces salga hacia la izquierda
        // y otras hacia la derecha.
        float direccionX;

        if (azar.RandiRange(0, 1) == 0)
        {
            direccionX = -1f;
        }
        else
        {
            direccionX = 1f;
        }

        pelota.Velocidad = new Vector2(330f * direccionX, -430f);

        tiempoAntesDelSaque = 0.8f;
    }

    private void CrearBloques()
    {
        bloques.Clear();

        const int filas = 6;
        const int columnas = 10;
        const float anchoBloque = 104f;
        const float altoBloque = 34f;
        const float separacion = 10f;
        const float inicioX = 75f;
        const float inicioY = 105f;

        Color[] colores =
        {
            new Color(0.95f, 0.3f, 0.3f),
            new Color(1f, 0.55f, 0.2f),
            new Color(1f, 0.82f, 0.2f),
            new Color(0.35f, 0.85f, 0.4f),
            new Color(0.25f, 0.65f, 1f),
            new Color(0.7f, 0.4f, 1f)
        };

        for (int fila = 0; fila < filas; fila++)
        {
            for (int columna = 0; columna < columnas; columna++)
            {
                float x = inicioX + columna * (anchoBloque + separacion);
                float y = inicioY + fila * (altoBloque + separacion);

                Bloque nuevoBloque = new Bloque();
                nuevoBloque.Rectangulo =
                    new Rect2(x, y, anchoBloque, altoBloque);
                nuevoBloque.Color = colores[fila];

                bloques.Add(nuevoBloque);
            }
        }
    }

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

        DrawRect(
            new Rect2(Vector2.Zero, tamanoPantalla),
            Colors.Black);

        float escala = Mathf.Min(
            tamanoPantalla.X / AnchoJuego,
            tamanoPantalla.Y / AltoJuego);

        Vector2 tamanoEscalado = new Vector2(AnchoJuego, AltoJuego) * escala;
        Vector2 margen = (tamanoPantalla - tamanoEscalado) / 2f;

        DrawSetTransform(margen, 0f, Vector2.One * escala);

        DibujarFondo();
        DibujarBloques();
        DibujarPala();
        DibujarPelota();
        DibujarMarcador();

        if (partidaTerminada)
        {
            DibujarPantallaFinal();
        }

        DrawSetTransform(Vector2.Zero, 0f, Vector2.One);
    }

    private void DibujarFondo()
    {
        DrawRect(
            new Rect2(0f, 0f, AnchoJuego, AltoJuego),
            new Color(0.035f, 0.045f, 0.075f));

        // Líneas decorativas sencillas del campo de juego.
        for (int y = 90; y < 600; y += 60)
        {
            DrawLine(
                new Vector2(25f, y),
                new Vector2(AnchoJuego - 25f, y),
                new Color(0.08f, 0.1f, 0.16f),
                2f);
        }
    }

    private void DibujarBloques()
    {
        foreach (Bloque bloque in bloques)
        {
            DrawRect(bloque.Rectangulo, bloque.Color);

            // Pequeña línea clara en la parte superior para dar sensación de volumen.
            DrawLine(
                bloque.Rectangulo.Position + new Vector2(4f, 4f),
                bloque.Rectangulo.Position + new Vector2(bloque.Rectangulo.Size.X - 4f, 4f),
                new Color(1f, 1f, 1f, 0.45f),
                3f);
        }
    }

    private void DibujarPala()
    {
        Rect2 rectangulo = pala.ObtenerRectangulo();

        float radioExtremo = pala.Alto / 2f;
        Color colorPala = new Color(0.2f, 0.8f, 1f);

        // El rectángulo central y los dos círculos forman una pala redondeada.
        DrawRect(
            new Rect2(
                rectangulo.Position.X + radioExtremo,
                rectangulo.Position.Y,
                rectangulo.Size.X - radioExtremo * 2f,
                rectangulo.Size.Y),
            colorPala);

        DrawCircle(
            new Vector2(rectangulo.Position.X + radioExtremo, pala.Posicion.Y),
            radioExtremo,
            colorPala);

        DrawCircle(
            new Vector2(
                rectangulo.Position.X + rectangulo.Size.X - radioExtremo,
                pala.Posicion.Y),
            radioExtremo,
            colorPala);
    }

    private void DibujarPelota()
    {
        DrawCircle(pelota.Posicion, pelota.Radio, Colors.White);
        DrawCircle(
            pelota.Posicion + new Vector2(-4f, -4f),
            4f,
            new Color(0.65f, 0.9f, 1f));
    }

    private void DibujarMarcador()
    {
        DrawString(
            ThemeDB.FallbackFont,
            new Vector2(28f, 43f),
            $"Puntos: {puntos}    Vidas: {vidas}    Bloques: {bloques.Count}",
            HorizontalAlignment.Left,
            -1f,
            26,
            Colors.White);

        DrawString(
            ThemeDB.FallbackFont,
            new Vector2(28f, 77f),
            "Mover: flechas o A/D    F11: pantalla completa",
            HorizontalAlignment.Left,
            -1f,
            18,
            new Color(0.75f, 0.82f, 0.95f));
    }

    private void DibujarPantallaFinal()
    {
        DrawRect(
            new Rect2(260f, 220f, 760f, 265f),
            new Color(0f, 0f, 0f, 0.86f));

        string titulo;
        Color colorTitulo;

        if (victoria)
        {
            titulo = "¡HAS GANADO!";
            colorTitulo = Colors.LimeGreen;
        }
        else
        {
            titulo = "FIN DE LA PARTIDA";
            colorTitulo = Colors.OrangeRed;
        }

        DibujarTextoCentrado(titulo, 305f, 48, colorTitulo);
        DibujarTextoCentrado($"Puntuación: {puntos}", 370f, 32, Colors.White);
        DibujarTextoCentrado("Pulsa ESPACIO para volver a jugar", 435f, 25, Colors.White);
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

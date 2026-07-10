using Godot;

// PULSO PERFECTO
// Pulsa ESPACIO cuando la marca esté dentro de la zona verde.
public partial class Node2d : Node2D
{
    // El marcador sabe moverse y rebotar en los extremos.
    private class Marcador
    {
        public float Posicion;
        public float Velocidad = 280;

        public void Mover(float segundos, float ancho)
        {
            Posicion += Velocidad * segundos;

            if (Posicion < 0 || Posicion > ancho)
                Velocidad *= -1;

            Posicion = Mathf.Clamp(Posicion, 0, ancho);
        }
    }

    private Marcador marcador = new();
    private RandomNumberGenerator azar = new();

    private float objetivo;
    private int puntos;
    private bool terminado;

    public override void _Ready()
    {
        azar.Randomize();
        Reiniciar();
    }

    public override void _Process(double delta)
    {
        if (!terminado)
        {
            float ancho = GetViewportRect().Size.X - 100;
            marcador.Mover((float)delta, ancho);
        }

        QueueRedraw();
    }

    public override void _Input(InputEvent evento)
    {
        if (evento is not InputEventKey tecla ||
            !tecla.Pressed ||
            tecla.Keycode != Key.Space)
        {
            return;
        }

        if (terminado)
        {
            Reiniciar();
            return;
        }

        // El jugador acierta si está a menos de 35 píxeles.
        if (Mathf.Abs(marcador.Posicion - objetivo) <= 35)
        {
            puntos++;
            marcador.Velocidad *= 1.12f;
            NuevoObjetivo();
        }
        else
        {
            terminado = true;
        }
    }

    private void NuevoObjetivo()
    {
        float ancho = GetViewportRect().Size.X;

        objetivo = azar.RandfRange(
            50,
            ancho - 150);
    }

    private void Reiniciar()
    {
        marcador.Posicion = 0;
        marcador.Velocidad = 280;
        puntos = 0;
        terminado = false;

        NuevoObjetivo();
    }

    public override void _Draw()
    {
        Vector2 pantalla = GetViewportRect().Size;
        float y = pantalla.Y / 2;

        DrawRect(
            new Rect2(Vector2.Zero, pantalla),
            Colors.Black);

        // Barra gris sobre la que se mueve el marcador.
        DrawRect(
            new Rect2(50, y - 12, pantalla.X - 100, 24),
            Colors.DimGray);

        // Zona en la que debemos pulsar.
        DrawRect(
            new Rect2(50 + objetivo - 35, y - 18, 70, 36),
            Colors.Green);

        DrawCircle(
            new Vector2(50 + marcador.Posicion, y),
            14,
            Colors.Yellow);

        DrawString(
            ThemeDB.FallbackFont,
            new Vector2(20, 35),
            $"Aciertos: {puntos}",
            fontSize: 24);

        if (terminado)
            DrawString(
                ThemeDB.FallbackFont,
                new Vector2(20, 70),
                "Fallaste. Pulsa ESPACIO",
                fontSize: 22);
    }
}
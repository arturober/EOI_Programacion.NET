using Godot;
using System.Collections.Generic;
// SERPIENTE RÁPIDA: flechas o WASD. Espacio reinicia al perder.
public partial class Node2d : Node2D
{
    private class Serpiente
    {
        public List<Vector2I> Cuerpo = new();
        public Vector2I Direccion = Vector2I.Right;
        public void Mover(bool crecer)
        {
            Cuerpo.Insert(0, Cuerpo[0] + Direccion);
            if (!crecer) Cuerpo.RemoveAt(Cuerpo.Count - 1);
        }
    }
    private const int Celda = 30;
    private Serpiente serpiente = new();
    private RandomNumberGenerator azar = new();
    private Vector2I comida;
    private float tiempo, intervalo;
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
            tiempo += (float)delta;
            if (tiempo >= intervalo)
            {
                tiempo = 0;
                Avanzar();
            }
        }
        QueueRedraw();
    }
    public override void _Input(InputEvent evento)
    {
        if (evento is not InputEventKey tecla || !tecla.Pressed) return;
        Vector2I nueva = serpiente.Direccion;
        if (tecla.Keycode == Key.Up || tecla.Keycode == Key.W) nueva = Vector2I.Up;
        if (tecla.Keycode == Key.Down || tecla.Keycode == Key.S) nueva = Vector2I.Down;
        if (tecla.Keycode == Key.Left || tecla.Keycode == Key.A) nueva = Vector2I.Left;
        if (tecla.Keycode == Key.Right || tecla.Keycode == Key.D) nueva = Vector2I.Right;
        if (nueva != -serpiente.Direccion) serpiente.Direccion = nueva;
        if (terminado && tecla.Keycode == Key.Space) Reiniciar();
    }
    private void Avanzar()
    {
        Vector2I cabeza = serpiente.Cuerpo[0] + serpiente.Direccion;
        Vector2I limite = (Vector2I)(GetViewportRect().Size / Celda);
        bool comer = cabeza == comida;
        if (cabeza.X < 0 || cabeza.Y < 0 || cabeza.X >= limite.X ||
            cabeza.Y >= limite.Y || serpiente.Cuerpo.Contains(cabeza))
        {
            terminado = true;
            return;
        }
        serpiente.Mover(comer);
        if (!comer) return;
        puntos++;
        intervalo = Mathf.Max(0.06f, intervalo - 0.005f);
        ColocarComida();
    }
    private void Reiniciar()
    {
        serpiente.Cuerpo = new() { new(8, 8), new(7, 8) };
        serpiente.Direccion = Vector2I.Right;
        puntos = 0;
        tiempo = 0;
        intervalo = 0.14f;
        terminado = false;
        ColocarComida();
    }
    private void ColocarComida()
    {
        Vector2I limite = (Vector2I)(GetViewportRect().Size / Celda);
        do comida = new(azar.RandiRange(0, limite.X - 1),
                        azar.RandiRange(0, limite.Y - 1));
        while (serpiente.Cuerpo.Contains(comida));
    }
    public override void _Draw()
    {
        DrawRect(new Rect2(Vector2.Zero, GetViewportRect().Size), Colors.Black);
        DrawRect(new Rect2(comida * Celda, Vector2.One * (Celda - 2)), Colors.Gold);
        foreach (Vector2I parte in serpiente.Cuerpo)
            DrawRect(new Rect2(parte * Celda, Vector2.One * (Celda - 2)), Colors.LimeGreen);
        DrawString(ThemeDB.FallbackFont, new Vector2(15, 28),
            $"Puntos: {puntos}", fontSize: 22);
        if (terminado)
            DrawString(ThemeDB.FallbackFont, new Vector2(15, 58),
                "Has perdido. Pulsa ESPACIO", fontSize: 22);
    }
}
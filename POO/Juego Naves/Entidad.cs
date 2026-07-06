using System.Numerics;
using Raylib_cs;

public abstract class Entidad
{
    public Vector2 Posicion { get; set; }
    public Vector2 Tamano { get; protected set; }
    public Color Color { get; protected set; }
    public bool Activo { get; set; } = true;

    public Entidad(Vector2 posicion, Vector2 tamano, Color color)
    {
        Posicion = posicion;
        Tamano = tamano;
        Color = color;
    }

    public abstract void Actualizar(float deltaTime);

    public virtual void Dibujar()
    {
        Raylib.DrawRectangleV(Posicion, Tamano, Color);
    }

    public bool ColisionaCon(Entidad otra)
    {
        if (!Activo || !otra.Activo) return false;
        
        return Raylib.CheckCollisionRecs(
            new Rectangle(Posicion.X, Posicion.Y, Tamano.X, Tamano.Y),
            new Rectangle(otra.Posicion.X, otra.Posicion.Y, otra.Tamano.X, otra.Tamano.Y)
        );
    }
}

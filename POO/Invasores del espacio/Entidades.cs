abstract class Entidad
{
    public int x { get; set; }
    public int y { get; set; } 
    public char Icono { get; set; }

    protected Entidad(int x, int y, char icono)
    {
        this.x = x;
        this.y = y;
        this.Icono = icono;
    }

    public abstract void Actualizar(AccionJugador accion, int anchoTablero);

    public bool EstaDentroDelTablero(int anchoTablero, int altoTablero)
    {
        return x >= 0 && x < anchoTablero && y >= 0 && y < altoTablero;
    }   
}

class Nave : Entidad
{
    private const char IconoNave = 'A';

    public Nave(int x, int y) : base(x, y, IconoNave)
    {
    }

    public override void Actualizar(AccionJugador accion, int anchoTablero)
    {
        if (accion == AccionJugador.Izquierda && x > 0)
        {
            x--;
        }
        else if (accion == AccionJugador.Derecha && x < anchoTablero - 1)
        {
            x++;
        }
    }
}

class Alien : Entidad
{
    private const char IconoAlien = 'V';

    public Alien(int x, int y) : base(x, y, IconoAlien)
    {
    }

    public override void Actualizar(AccionJugador accion, int anchoTablero)
    {
        x++;
    }
}

class Bala : Entidad
{
    private const char IconoBala = '|';

    public Bala(int x, int y) : base(x, y, IconoBala)
    {
    }

    public override void Actualizar(AccionJugador accion, int anchoTablero)
    {
        y--;
    }
}
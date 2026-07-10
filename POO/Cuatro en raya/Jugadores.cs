abstract class Jugador
{
    public string Nombre { get; set; }
    public char Ficha { get; set; }

    public Jugador(string nombre, char ficha)
    {
        Nombre = nombre;
        Ficha = ficha;
    }

    public abstract int HacerMovimiento();
}

class JugadorHumano : Jugador
{
    public JugadorHumano(string nombre, char ficha) : base(nombre, ficha) { }

    public override int HacerMovimiento()
    {
        Console.WriteLine($"{Nombre}, es tu turno. Ingresa la columna (0-6):");
        int columna;
        while (!int.TryParse(Console.ReadLine(), out columna) || columna < 0 || columna > 6)
        {
            Console.WriteLine("Entrada inválida. Ingresa un número entre 0 y 6:");
        }
        return columna;
    }
}

class JugadorOrdenador : Jugador
{
    private Random random = new Random();

    public JugadorOrdenador(string nombre, char ficha) : base(nombre, ficha) { }

    public override int HacerMovimiento()
    {
        int columna = random.Next(0, 7);
        Console.WriteLine($"{Nombre} ha elegido la columna {columna}.");
        return columna;
    }
}
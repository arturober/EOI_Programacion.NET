abstract class Jugador
{
    public string Nombre { get; }
    public char Ficha { get; }

    public Jugador(string nombre, char ficha)
    {
        Nombre = nombre;
        Ficha = ficha;
    }

    public abstract int ElegirColumna(Tablero tablero);
}

class JugadorHumano : Jugador
{
    public JugadorHumano(string nombre, char ficha) : base(nombre, ficha)
    {
    }

    public override int ElegirColumna(Tablero tablero)
    {
        while (true)
        {
            Console.Write($"Elige una columna del 1 al {Tablero.Columnas}: ");
            string texto = Console.ReadLine() ?? string.Empty;

            if (!int.TryParse(texto, out int columnaElegida)) {
                Console.WriteLine("Entrada incorrecta. Por favor, introduce un número.");
                continue;
            }

            if (columnaElegida < 1 || columnaElegida > Tablero.Columnas)
            {
                Console.WriteLine($"Entrada incorrecta. Por favor, introduce un número entre 1 y {Tablero.Columnas}.");
                continue;
            }

            int columna = columnaElegida - 1; // Restamos 1 para convertir a índice de matriz

            if (!tablero.ColumnaEsValida(columna))
            {
                Console.WriteLine("La columna está llena. Por favor, elige otra columna.");
                continue;
            }

            return columna; 
        }

    }
}

class JugadorOrdenador : Jugador
{
    public JugadorOrdenador(string nombre, char ficha) : base(nombre, ficha)
    {
    }

    public override int ElegirColumna(Tablero tablero)
    {
        Random random = new Random();
        int columna;

        do
        {
            columna = random.Next(0, Tablero.Columnas);
        } while (!tablero.ColumnaEsValida(columna));

        return columna;
    }
}
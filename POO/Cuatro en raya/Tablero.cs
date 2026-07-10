class Tablero
{
    public const int Filas = 6;
    public const int Columnas = 7;

    private const char Vacio = '.';

    private readonly char[,] casillas;

    public Tablero()
    {
        casillas = new char[Filas, Columnas];
        InicializarTablero();
    }

    public void InicializarTablero()
    {
        for (int fila = 0; fila < Filas; fila++)
        {
            for (int columna = 0; columna < Columnas; columna++)
            {
                casillas[fila, columna] = Vacio;
            }
        }
    }

    public void Mostrar()
    {
        MostrarNumerosdeColumnas();

        for (int fila = 0; fila < Filas; fila++)
        {
            for (int columna = 0; columna < Columnas; columna++)
            {
                MostrarCasilla(casillas[fila, columna]);
            }
            Console.WriteLine("  ");
        }
        Console.ResetColor();
    }

    private void MostrarNumerosdeColumnas()
    {

        for (int columna = 1; columna <= Columnas; columna++)
        {
            Console.Write($"  {columna}");
        }
        Console.WriteLine();
    }

    private void MostrarCasilla(char ficha)
    {
        Console.BackgroundColor = ConsoleColor.Blue;

        Console.Write(" " + ObtenerEmojiDeFicha(ficha));

        Console.ResetColor();
    }

    public string ObtenerEmojiDeFicha(char ficha)
    {
        return ficha switch
        {
            'X' => "🔴", // Emoji para la ficha del jugador humano
            'O' => "🟡", // Emoji para la ficha del jugador ordenador
            _ => "⚪"   // Emoji para casilla vacía
        };
    }
}

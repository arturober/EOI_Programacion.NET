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
            Console.Write(" ");
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
            Console.Write($"   {columna}");
        }
        Console.WriteLine();
    }

    private void MostrarCasilla(char ficha)
    {
        Console.BackgroundColor = ConsoleColor.Blue;

        Console.Write(" " + ObtenerEmojiDeFicha(ficha) + " ");

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

    public bool ColumnaEsValida(int columna)
    {
        return columna >= 0 && columna < Columnas && casillas[0, columna] == Vacio;
    }

    public bool ColocarFicha(int columna, char ficha)
    {
        if (!ColumnaEsValida(columna))
        {
            return false;
        }

        for (int fila = Filas - 1; fila >= 0; fila--)
        {
            if (casillas[fila, columna] == Vacio)
            {
                casillas[fila, columna] = ficha;
                return true;
            }
        }

        return false;
    }

    public bool HayGanador(char ficha)
    {
        // Comprobar filas
        for (int fila = 0; fila < Filas; fila++)
        {
            for (int columna = 0; columna <= Columnas - 4; columna++)
            {
                if (casillas[fila, columna] == ficha &&
                    casillas[fila, columna + 1] == ficha &&
                    casillas[fila, columna + 2] == ficha &&
                    casillas[fila, columna + 3] == ficha)
                {
                    return true;
                }
            }
        }

        // Comprobar columnas
        for (int columna = 0; columna < Columnas; columna++)
        {
            for (int fila = 0; fila <= Filas - 4; fila++)
            {
                if (casillas[fila, columna] == ficha &&
                    casillas[fila + 1, columna] == ficha &&
                    casillas[fila + 2, columna] == ficha &&
                    casillas[fila + 3, columna] == ficha)
                {
                    return true;
                }
            }
        }

        // Comprobar diagonales (de izquierda a derecha)
        for (int fila = 0; fila <= Filas - 4; fila++)
        {
            for (int columna = 0; columna <= Columnas - 4; columna++)
            {
                if (casillas[fila, columna] == ficha &&
                    casillas[fila + 1, columna + 1] == ficha &&
                    casillas[fila + 2, columna + 2] == ficha &&
                    casillas[fila + 3, columna + 3] == ficha)
                {
                    return true;
                }
            }
        }

        // Comprobar diagonales (de derecha a izquierda)
        for (int fila = 0; fila <= Filas - 4; fila++)
        {
            for (int columna = 3; columna < Columnas; columna++)
            {
                if (casillas[fila, columna] == ficha &&
                    casillas[fila + 1, columna - 1] == ficha &&
                    casillas[fila + 2, columna - 2] == ficha &&
                    casillas[fila + 3, columna - 3] == ficha)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public bool EstaLleno()
    {
        for (int columna = 0; columna < Columnas; columna++)
        {
            if (casillas[0, columna] == Vacio)
            {
                return false;
            }
        }
        return true;
    }
}

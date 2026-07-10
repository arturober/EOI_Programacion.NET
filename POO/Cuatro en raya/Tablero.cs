// Tablero almacena las casillas y aplica todas las operaciones relacionadas con
// ellas: reiniciar, mostrar, colocar o retirar fichas y comprobar el resultado.
//
// No decide turnos, no pide datos por teclado y no anuncia ganadores. Esas tareas
// pertenecen a Juego o a los distintos tipos de Jugador.
class Tablero
{
    // Un tablero clásico de 4 en raya tiene seis filas y siete columnas.
    // Son constantes públicas porque otras clases necesitan conocer sus límites.
    public const int Filas = 6;
    public const int Columnas = 7;

    // El punto representa internamente una casilla que todavía no contiene ficha.
    // Es privado porque el resto del programa no necesita conocer ese detalle.
    private const char CasillaVacia = '.';

    // El array se crea una sola vez. Su contenido cambiará durante la partida, pero
    // la variable casillas siempre seguirá apuntando al mismo array.
    private readonly char[,] casillas;

    public Tablero()
    {
        casillas = new char[Filas, Columnas];
        Reiniciar();
    }

    public void Reiniciar()
    {
        // Recorremos todas las posiciones para eliminar cualquier ficha de una
        // partida anterior y dejar cada casilla con el carácter de casilla vacía.
        for (int fila = 0; fila < Filas; fila++)
        {
            for (int columna = 0; columna < Columnas; columna++)
            {
                casillas[fila, columna] = CasillaVacia;
            }
        }
    }

    public void Mostrar()
    {
        // Tablero no limpia la consola. Solo dibuja su contenido. Esto evita que
        // una clase de datos tome decisiones sobre toda la interfaz del programa.
        MostrarNumerosDeColumnas();

        for (int fila = 0; fila < Filas; fila++)
        {
            Console.Write("  ");

            for (int columna = 0; columna < Columnas; columna++)
            {
                MostrarCasilla(casillas[fila, columna]);
            }

            Console.WriteLine();
        }

        // Nos aseguramos de que la consola recupere sus colores normales después
        // de terminar el dibujo, incluso aunque ya se restablezcan por casilla.
        Console.ResetColor();
    }

    private void MostrarNumerosDeColumnas()
    {
        Console.Write(" ");

        // En pantalla se muestran columnas del 1 al 7, aunque internamente el array
        // utilice índices del 0 al 6.
        for (int columna = 1; columna <= Columnas; columna++)
        {
            Console.Write($"   {columna}");
        }

        Console.WriteLine();
    }

    private void MostrarCasilla(char ficha)
    {
        // El fondo azul representa la estructura vertical del tablero.
        Console.BackgroundColor = ConsoleColor.Blue;

        Console.Write(" ");
        Console.Write(ObtenerEmojiDeFicha(ficha));
        Console.Write(" ");

        // El color se restablece tras cada casilla para que los mensajes del juego
        // no se impriman accidentalmente sobre un fondo azul.
        Console.ResetColor();
    }

    public string ObtenerEmojiDeFicha(char ficha)
    {
        // X representa la ficha roja y O representa la amarilla. Cualquier otro
        // valor se muestra como una casilla blanca vacía.
        if (ficha == 'X')
        {
            return "🔴";
        }

        if (ficha == 'O')
        {
            return "🟡";
        }

        return "⚪";
    }

    public bool ColumnaEsValida(int columna)
    {
        // El operador && utiliza evaluación de cortocircuito. La casilla superior
        // solo se consulta si el índice está dentro del intervalo permitido, por lo
        // que nunca se intenta acceder a una posición inexistente del array.
        return columna >= 0 &&
               columna < Columnas &&
               casillas[0, columna] == CasillaVacia;
    }

    public bool ColocarFicha(int columna, char ficha)
    {
        // Una columna fuera del tablero o completamente llena no admite la ficha.
        if (!ColumnaEsValida(columna))
        {
            return false;
        }

        // Las fichas caen por gravedad. Por ello se busca desde la última fila hacia
        // arriba y se utiliza la primera casilla libre encontrada.
        int filaLibre = ObtenerFilaLibre(columna);
        casillas[filaLibre, columna] = ficha;

        return true;
    }

    public void DeshacerUltimaFicha(int columna)
    {
        // La última ficha colocada en una columna siempre es la ficha ocupada que
        // está más arriba. Se recorre de arriba abajo y se elimina la primera.
        //
        // Este método se utiliza para las simulaciones de la IA: permite probar un
        // movimiento y restaurar inmediatamente el estado anterior del tablero.
        for (int fila = 0; fila < Filas; fila++)
        {
            if (casillas[fila, columna] != CasillaVacia)
            {
                casillas[fila, columna] = CasillaVacia;
                return;
            }
        }
    }

    private int ObtenerFilaLibre(int columna)
    {
        // Empezamos por la parte inferior para reproducir la caída de una ficha.
        for (int fila = Filas - 1; fila >= 0; fila--)
        {
            if (casillas[fila, columna] == CasillaVacia)
            {
                return fila;
            }
        }

        // ColocarFicha llama a este método únicamente después de validar la
        // columna, así que en una ejecución correcta nunca se alcanza este punto.
        return -1;
    }

    public bool EstaLleno()
    {
        // Si al menos una columna sigue siendo válida, todavía queda espacio.
        for (int columna = 0; columna < Columnas; columna++)
        {
            if (ColumnaEsValida(columna))
            {
                return false;
            }
        }

        return true;
    }

    public bool HayGanador(char ficha)
    {
        // Se toma cada ficha del jugador como posible comienzo de una línea de
        // cuatro. Desde ella se comprueban las cuatro direcciones necesarias.
        for (int fila = 0; fila < Filas; fila++)
        {
            for (int columna = 0; columna < Columnas; columna++)
            {
                if (casillas[fila, columna] == ficha &&
                    HayCuatroDesde(fila, columna, ficha))
                {
                    return true;
                }
            }
        }

        return false;
    }

    private bool HayCuatroDesde(int fila, int columna, char ficha)
    {
        // Solo es necesario comprobar estas cuatro direcciones:
        // - horizontal hacia la derecha;
        // - vertical hacia abajo;
        // - diagonal hacia abajo y a la derecha;
        // - diagonal hacia abajo y a la izquierda.
        //
        // No se comprueban las direcciones opuestas porque esas mismas líneas se
        // detectarán al comenzar el recorrido desde su extremo contrario.
        return HayCuatroEnDireccion(fila, columna, 0, 1, ficha) ||
               HayCuatroEnDireccion(fila, columna, 1, 0, ficha) ||
               HayCuatroEnDireccion(fila, columna, 1, 1, ficha) ||
               HayCuatroEnDireccion(fila, columna, 1, -1, ficha);
    }

    private bool HayCuatroEnDireccion(
        int filaInicial,
        int columnaInicial,
        int movimientoFila,
        int movimientoColumna,
        char ficha)
    {
        // Se comprueban exactamente cuatro posiciones: la inicial y las tres que
        // se obtienen al aplicar sucesivamente el movimiento indicado.
        for (int posicion = 0; posicion < 4; posicion++)
        {
            int fila = filaInicial + posicion * movimientoFila;
            int columna = columnaInicial + posicion * movimientoColumna;

            // En cuanto una posición sale del tablero o contiene otra ficha, ya no
            // puede existir una línea de cuatro en esta dirección.
            if (!EstaDentroDelTablero(fila, columna) ||
                casillas[fila, columna] != ficha)
            {
                return false;
            }
        }

        // Si las cuatro comprobaciones han sido correctas, existe una línea ganadora.
        return true;
    }

    private bool EstaDentroDelTablero(int fila, int columna)
    {
        return fila >= 0 &&
               fila < Filas &&
               columna >= 0 &&
               columna < Columnas;
    }
}
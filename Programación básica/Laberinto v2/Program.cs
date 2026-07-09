bool jugando = true;
bool victoriaFinal = false;

int nivelActual = 1;
int nivelesTotales = 3;

DateTime tiempoInicio = DateTime.Now;

int jugadorX = 1;
int jugadorY = 1;

int tesorosRecolectados = 0;
int tesorosTotales = 0;

char[,] mapa = new char[0, 0];

bool cargarNuevoNivel = true;

while(jugando)
{
    if (cargarNuevoNivel)
    {
        tesorosRecolectados = 0;
        jugadorX = 1;
        jugadorY = 1;

        if (nivelActual == 1)
        {
            tesorosTotales = 4;
            mapa = new char[,] {
                { '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#' },
                { '#', ' ', ' ', ' ', '#', '*', ' ', ' ', ' ', ' ', '#' },
                { '#', ' ', '#', ' ', '#', ' ', '#', '#', '#', ' ', '#' },
                { '#', '*', '#', ' ', ' ', ' ', ' ', ' ', '#', ' ', '#' },
                { '#', '#', '#', '#', '#', ' ', '#', ' ', '#', ' ', '#' },
                { '#', ' ', ' ', ' ', ' ', ' ', '#', ' ', ' ', '*', '#' },
                { '#', ' ', '#', '#', '#', '#', '#', '#', '#', ' ', '#' },
                { '#', ' ', ' ', ' ', '*', ' ', ' ', ' ', ' ', ' ', '#' },
                { '#', '#', '#', '#', '#', '#', '#', '#', '#', 'S', '#' }
            };
        }
        else if (nivelActual == 2)
        {
            tesorosTotales = 5;
            mapa = new char[,] {
                { '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#' },
                { '#', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '*', '#' },
                { '#', ' ', '#', '#', '#', '#', '#', '#', '#', '#', '#', ' ', '#' },
                { '#', ' ', '#', '*', ' ', ' ', ' ', ' ', ' ', ' ', '#', ' ', '#' },
                { '#', ' ', '#', ' ', '#', '#', '#', '#', '#', ' ', '#', ' ', '#' },
                { '#', ' ', '#', ' ', '#', '*', ' ', '*', '#', ' ', '#', ' ', '#' },
                { '#', '*', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '#' },
                { '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', 'S', '#' }
            };
        }
        else if (nivelActual == 3)
        {
            tesorosTotales = 6;
            mapa = new char[,] {
                { '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#' },
                { '#', ' ', ' ', ' ', '#', '*', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '#' },
                { '#', ' ', '#', ' ', '#', ' ', '#', '#', '#', '#', '#', '#', '#', ' ', '#' },
                { '#', ' ', '#', ' ', ' ', ' ', '#', '*', ' ', ' ', ' ', ' ', '#', ' ', '#' },
                { '#', ' ', '#', '#', '#', '#', '#', ' ', '#', '#', '#', ' ', '#', ' ', '#' },
                { '#', '*', ' ', ' ', ' ', ' ', ' ', ' ', '#', '*', ' ', ' ', '#', ' ', '#' },
                { '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', ' ', '#' },
                { '#', 'S', ' ', ' ', '*', ' ', ' ', ' ', ' ', ' ', ' ', '*', ' ', ' ', '#' },
                { '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#' }
            };
        }
        else
        {
            victoriaFinal = true;
            jugando = false;
        }

        cargarNuevoNivel = false;
    }

    Console.Clear();
    Console.WriteLine($"=== Laberinto v2 ===");
    Console.WriteLine($"Nivel: {nivelActual}/{nivelesTotales}");
    Console.WriteLine($"Tesoros recolectados: {tesorosRecolectados}/{tesorosTotales}");
    Console.WriteLine($"Tiempo transcurrido: {(DateTime.Now - tiempoInicio).ToString(@"hh\:mm\:ss")}");

    for(int fila = 0; fila < mapa.GetLength(0); fila++)
    {
        for(int columna = 0; columna < mapa.GetLength(1); columna++)
        {
            if (fila == jugadorY && columna == jugadorX)
            {
                Console.Write("@ "); // Representa al jugador
            }
            else
            {
                Console.Write(mapa[fila, columna] + " ");
            }
        }
        Console.WriteLine();
    }

    Console.WriteLine("\nUsa las flechas del teclado para moverte y recoger los tesoros (*) o pulsa ESC para salir.");

    ConsoleKeyInfo tecla = Console.ReadKey(true);
    int nuevaX = jugadorX;
    int nuevaY = jugadorY;

    switch(tecla.Key)
    {
        case ConsoleKey.UpArrow: nuevaY--; break;
        case ConsoleKey.DownArrow: nuevaY++; break;
        case ConsoleKey.LeftArrow: nuevaX--; break;
        case ConsoleKey.RightArrow: nuevaX++; break;
        case ConsoleKey.Escape: jugando = false; break;
    }

    if ((nuevaY < 0 || nuevaY >= mapa.GetLength(0)) || (nuevaX < 0 || nuevaX >= mapa.GetLength(1)))
    {
        continue; // Saltar las siguientes instrucciones y esperar la siguiente entrada del jugador
    }

    jugadorX = nuevaX;
    jugadorY = nuevaY;
}
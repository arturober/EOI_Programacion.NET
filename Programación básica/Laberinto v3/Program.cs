bool jugando = true;
bool victoriaFinal = false;

int nivelActual = 1;
int nivelesTotales = 3;
int vidas  = 3;
int bombas = 0;

DateTime tiempoInicio = DateTime.Now;

int jugadorX = 1;
int jugadorY = 1;
int tesorosRecolectados = 0;
int tesorosTotales = 0;

char[,] mapa = new char[0, 0];
bool cargarNuevoNivel = true;

while (jugando)
{
    if (cargarNuevoNivel)
    {
        tesorosRecolectados = 0;
        jugadorX = 1;
        jugadorY = 1;

        if (nivelActual == 1)
        {
            tesorosTotales = 4;
            // Añadida una bomba (B) en el centro del mapa
            mapa = new char[,] {
                { '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#' },
                { '#', ' ', ' ', ' ', '#', '*', ' ', ' ', ' ', ' ', '#' },
                { '#', ' ', '#', ' ', '#', ' ', '#', 'T', '#', ' ', '#' },
                { '#', '*', '#', ' ', 'B', ' ', 'T', ' ', '#', ' ', '#' },
                { '#', '#', '#', '#', '#', ' ', '#', ' ', '#', ' ', '#' },
                { '#', ' ', ' ', ' ', ' ', ' ', '#', ' ', 'T', '*', '#' },
                { '#', ' ', '#', 'T', '#', '#', '#', '#', '#', ' ', '#' },
                { '#', ' ', ' ', ' ', '*', ' ', ' ', ' ', ' ', ' ', '#' },
                { '#', '#', '#', '#', '#', '#', '#', '#', '#', 'S', '#' }
            };
        }
        else if (nivelActual == 2)
        {
            tesorosTotales = 5;
            mapa = new char[,] {
                { '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#' },
                { '#', ' ', ' ', 'T', ' ', ' ', 'B', ' ', 'T', ' ', ' ', '*', '#' },
                { '#', ' ', '#', '#', '#', '#', '#', '#', '#', '#', '#', ' ', '#' },
                { '#', ' ', '#', '*', ' ', ' ', ' ', ' ', ' ', ' ', '#', ' ', '#' },
                { '#', 'T', '#', ' ', '#', '#', '#', '#', '#', ' ', '#', 'T', '#' },
                { '#', ' ', '#', ' ', '#', '*', ' ', '*', '#', ' ', '#', ' ', '#' },
                { '#', '*', ' ', ' ', 'T', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '#' },
                { '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', 'S', '#' }
            };
        }
        else if (nivelActual == 3)
        {
            tesorosTotales = 6;
            mapa = new char[,] {
                { '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#' },
                { '#', ' ', ' ', ' ', '#', '*', ' ', ' ', 'T', ' ', 'B', ' ', ' ', ' ', '#' },
                { '#', ' ', '#', ' ', '#', ' ', '#', '#', '#', '#', '#', '#', '#', ' ', '#' },
                { '#', 'T', '#', ' ', 'T', ' ', '#', '*', ' ', ' ', ' ', 'T', '#', ' ', '#' },
                { '#', ' ', '#', '#', '#', '#', '#', ' ', '#', '#', '#', ' ', '#', 'T', '#' },
                { '#', '*', ' ', 'B', ' ', ' ', 'T', ' ', '#', '*', ' ', ' ', '#', ' ', '#' },
                { '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', ' ', '#' },
                { '#', 'S', ' ', 'T', '*', ' ', ' ', ' ', ' ', ' ', ' ', '*', 'T', ' ', '#' },
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
    Console.WriteLine("Laberinto v3");
    Console.WriteLine($"Nivel: {nivelActual}/{nivelesTotales} | Vidas: {vidas}");
    Console.WriteLine($"Tesoros: {tesorosRecolectados}/{tesorosTotales} | Bombas: {bombas}");

    for (int fila = 0; fila < mapa.GetLength(0); fila++)
    {
        for (int columna = 0; columna < mapa.GetLength(1); columna++)
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

    Console.WriteLine("Usa las teclas flechas para moverte. Presiona ESC para salir.");
    Console.WriteLine("[B] Bomba | [ESPACIO] Usar bomba | [T] Trampa | [*] Tesoros | [S] Salida");

    ConsoleKeyInfo tecla = Console.ReadKey(true);

    int nuevoX = jugadorX;
    int nuevoY = jugadorY;

    switch (tecla.Key)
    {
        case ConsoleKey.UpArrow: nuevoY--; break;
        case ConsoleKey.DownArrow: nuevoY++; break;
        case ConsoleKey.LeftArrow: nuevoX--; break;
        case ConsoleKey.RightArrow: nuevoX++; break;
        case ConsoleKey.Escape: jugando = false; break;
    }

    if ((nuevoY < 0 || nuevoY >= mapa.GetLength(0)) || (nuevoX < 0 || nuevoX >= mapa.GetLength(1)))
    {
        continue; // Evita que el jugador se mueva fuera del mapa
    }

    char casillaDestino = mapa[nuevoY, nuevoX];

    if (casillaDestino == 'S' && tesorosRecolectados < tesorosTotales)
    {
        Console.WriteLine("¡Debes recoger todos los tesoros antes de salir!");
        Console.ReadKey();
        continue;
    }

    if (casillaDestino != '#')
    {
        jugadorX = nuevoX;
        jugadorY = nuevoY;

        if (casillaDestino == '*')
        {
            tesorosRecolectados++;
            mapa[nuevoY, nuevoX] = ' '; // Elimina el tesoro del mapa
        }

        else if (casillaDestino == 'S')
        {
            nivelActual++;
            cargarNuevoNivel = true;
        }
    }
}
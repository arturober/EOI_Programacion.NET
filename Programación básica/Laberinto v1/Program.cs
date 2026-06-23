bool jugando = true;
int jugadorX = 1;
int jugadorY = 1;

int tesorosRecolectados = 0;
int tesorosTotales = 4;

char[,] mapa = {
    // 0    1    2    3    4    5    6    7    8    9   10
    { '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#' }, // 0
    { '#', ' ', ' ', ' ', '#', '*', ' ', ' ', ' ', ' ', '#' }, // 1 
    { '#', ' ', '#', ' ', '#', ' ', '#', '#', '#', ' ', '#' }, // 2
    { '#', ' ', '#', ' ', ' ', ' ', '#', ' ', '#', ' ', '#' }, // 3
    { '#', '*', '#', ' ', ' ', ' ', ' ', ' ', '#', ' ', '#' }, // 4
    { '#', ' ', '#', '#', '#', '#', '#', ' ', '#', ' ', '#' }, // 5
    { '#', ' ', ' ', ' ', ' ', ' ', '#', ' ', '#', '*', '#' }, // 6
    { '#', '#', '#', '#', '#', ' ', '#', ' ', '#', ' ', '#' }, // 7
    { '#', ' ', ' ', ' ', ' ', ' ', '#', ' ', ' ', ' ', '#' }, // 8
    { '#', ' ', '#', '#', '#', '#', '#', '#', '#', ' ', '#' }, // 9
    { '#', ' ', ' ', ' ', '*', ' ', ' ', ' ', ' ', ' ', '#' }, // 10
    { '#', '#', '#', '#', '#', '#', '#', '#', '#', 'S', '#' }  // 11
};

while (jugando)
{
    // Limpiar la consola y mostrar el estado del juego
    Console.Clear();
    Console.WriteLine("=== Laberinto ===");
    Console.WriteLine($"Tesoros recolectados: {tesorosRecolectados}/{tesorosTotales}");

    // Mostrar el mapa
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

    // Solicitar al jugador que introduzca un movimiento
    Console.WriteLine("\nUsa las flechas del teclado para moverte (Pulsa ESC para salir).");
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

    char casillaDestino = mapa[nuevaY, nuevaX];

    if (casillaDestino != '#')
    {
        jugadorX = nuevaX;
        jugadorY = nuevaY;  

        if (casillaDestino == '*')
        {
            tesorosRecolectados++;
            mapa[nuevaY, nuevaX] = ' '; // Eliminar el tesoro del mapa
        }
        
        if ((casillaDestino == 'S') && (tesorosRecolectados == tesorosTotales))
        {
            Console.Clear();
            Console.WriteLine("¡Felicidades! Has recolectado todos los tesoros y has salido del laberinto.");
            jugando = false;
        }
    }
}

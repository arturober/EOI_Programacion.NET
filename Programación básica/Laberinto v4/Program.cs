const char PARED = '#';
const char VACIO = ' ';
const char SUELO_FICHERO = '.';

const char TESORO = '*';
const char TRAMPA = 'T';
const char TRAMPA_INVISIBLE = 'X';
const char BOMBA = 'B';
const char SALIDA = 'S';
const char LLAVE = 'L';
const char PUERTA = 'P';

const char INICIO_JUGADOR = 'J';
const char JUGADOR = '@';

const string CARPETA_NIVELES = "niveles";
const string FICHERO_RANKING = "ranking.txt";

bool jugando = true;
bool victoriaFinal = false;
int nivelActual = 1;
int nivelesTotales = 3;
int vidas = 3;
int bombas = 0;
bool tieneLlave = false;
int jugadorX = 1;
int jugadorY = 1;
int tesorosRecolectados = 0;
int tesorosTotales = 0;
int tesorosTotalesPartida = 0;
int movimientos = 0;
string mensaje = "Recoge todos los tesoros y encuentra la salida.";
DateTime tiempoInicio = DateTime.Now;
char[,] mapa = new char[0, 0];
bool debeCargarNivel = true;

CrearFicherosDeNivelSiNoExisten();

while (jugando)
{
    if (debeCargarNivel)
    {
        CargarNivel();

        if (!jugando)
        {
            break;
        }
    }

    DibujarInterfaz();

    ConsoleKeyInfo tecla = Console.ReadKey(true);

    int destinoX = jugadorX;
    int destinoY = jugadorY;

    if (tecla.Key == ConsoleKey.Escape)
    {
        jugando = false;
        mensaje = "Te has rendido.";
        continue;
    }
    if (tecla.Key == ConsoleKey.Spacebar)
    {
        UsarBomba();
        continue;
    }

    bool esMovimientoValido = CalcularDestino(tecla.Key, ref destinoX, ref destinoY);

    if (!esMovimientoValido)
    {
        mensaje = "Tecla o movimiento incorrecto. Usa las flechas para moverte, ESC para salir o ESPACIO para usar una bomba.";
        continue;
    }

    IntentoMoverJugador(destinoX, destinoY);
}

void CrearFicherosDeNivelSiNoExisten()
{
}

void CargarNivel()
{
    if (nivelActual > nivelesTotales)
    {
        jugando = false;
        victoriaFinal = true;
        return;
    }

    tesorosRecolectados = 0;
    tieneLlave = false;

    string nombreFichero = "nivel" + nivelActual + ".txt";
    string rutaFichero = Path.Combine(CARPETA_NIVELES, nombreFichero);

    try
    {
        mapa = CargarMapaDesdeFichero(rutaFichero);    
    }
    catch (Exception error) {
    }

    //tesorosTotales = ContarTesoros();

    debeCargarNivel = false;

    mensaje = "Nivel " + nivelActual + " cargado. Recoge todos los tesoros y encuentra la salida.";
}

void DibujarInterfaz()
{
    Console.Clear();

    for (int fila = 0; fila < mapa.GetLength(0); fila++)
    {
        for (int columna = 0; columna < mapa.GetLength(1); columna++)
        {
            if (fila == jugadorY && columna == jugadorX)
            {
                Console.Write(JUGADOR + " ");
            }
            else
            {
                Console.Write(mapa[fila, columna] + " ");
            }
        }
        Console.WriteLine();
    }
}

void UsarBomba()
{
}

bool CalcularDestino(ConsoleKey tecla, ref int destinoX, ref int destinoY)
{
    switch (tecla)
    {
        case ConsoleKey.UpArrow: destinoY--; return true;
        case ConsoleKey.DownArrow: destinoY++; return true;
        case ConsoleKey.LeftArrow: destinoX--; return true;
        case ConsoleKey.RightArrow: destinoX++; return true;
        default: return false; // Tecla no válida
    }
}

void IntentoMoverJugador(int destinoX, int destinoY)
{
    if (!EstaDentroDelMapa(destinoX, destinoY))
    {
        mensaje = "No puedes salirte del mapa.";
        return;
    }

    char casillaDestino = mapa[destinoY, destinoX];

    jugadorX = destinoX;
    jugadorY = destinoY;
}

char [,] CargarMapaDesdeFichero(string rutaFichero)
{
    string[] lineas = File.ReadAllLines(rutaFichero);

    int filas = lineas.Length;
    int columnas = lineas[0].Length;

    char[,] mapa = new char[filas, columnas];

    for (int fila = 0; fila < filas; fila++)
    {
        for (int columna = 0; columna < columnas; columna++)
        {
            char simbolo = lineas[fila][columna];

            if (simbolo == INICIO_JUGADOR)
            {
                jugadorX = columna;
                jugadorY = fila;
                simbolo = VACIO;
            }

            mapa[fila, columna] = simbolo;
        }
    }

    return mapa;
}

bool EstaDentroDelMapa(int x, int y)
{
    return (x >= 0 && x < mapa.GetLength(1)) && (y >= 0 && y < mapa.GetLength(0));
}
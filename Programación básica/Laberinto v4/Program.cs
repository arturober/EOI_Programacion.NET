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

TimeSpan tiempoTotal = DateTime.Now - tiempoInicio;
int segundosTotales = (int)tiempoTotal.TotalSeconds;
int puntuacion = tesorosTotalesPartida * 10 + vidas * 5 + bombas * 2 - segundosTotales - movimientos;

Console.WriteLine("Introduce tu nombre para guardar la puntuación:");
string nombreJugador = Console.ReadLine() ?? "Jugador";

string resultadoFinal = ObtenerResultadoFinal();

GuardarPuntuacion(nombreJugador, puntuacion, segundosTotales, movimientos, resultadoFinal);


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

    Console.WriteLine();
    Console.WriteLine("Laberinto v4");
    Console.WriteLine("Nivel: " + nivelActual + "/" + nivelesTotales);
    Console.WriteLine("Vidas: " + vidas);
    Console.WriteLine("Bombas: " + bombas);
    Console.WriteLine("Tesoros: " + tesorosRecolectados);
    Console.WriteLine("Tienes llave: " + (tieneLlave ? "Sí" : "No"));
    Console.WriteLine("Movimientos: " + movimientos);
    Console.WriteLine();

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
                char casilla = mapa[fila, columna];
                if (casilla == TRAMPA_INVISIBLE)
                {
                    Console.Write(VACIO + " ");
                }
                else
                {
                    Console.Write(casilla + " ");
                }
            }
        }
        Console.WriteLine();
    }

    Console.WriteLine();
    Console.WriteLine($"{TESORO}: tesoro, {TRAMPA}: trampa, {BOMBA}: bomba, {LLAVE}: llave, {PUERTA}: puerta, {SALIDA}: salida");
    Console.WriteLine("Mensaje: " + mensaje);
}

void UsarBomba()
{
    if (bombas <= 0) {
        mensaje = "No tienes bombas disponibles.";
        return;
    }
    
    bombas--;

    for (int fila = jugadorY - 1; fila <= jugadorY + 1; fila++)
    {
        for (int columna = jugadorX - 1; columna <= jugadorX + 1; columna++)
        {
            if (EstaDentroDelMapa(columna, fila))
            {
                char casilla = mapa[fila, columna];

                bool esBorde = fila == 0 || fila == mapa.GetLength(0) - 1 || columna == 0 || columna == mapa.GetLength(1) - 1;

                if (!esBorde && casilla == PARED)
                {
                    mapa[fila, columna] = VACIO;
                }
            }
        }
    }
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

    if (casillaDestino == PARED)
    {
        mensaje = "No puedes atravesar paredes.";
        return;
    }

    if (casillaDestino == PUERTA && !tieneLlave)
    {
        mensaje = "Necesitas una llave para abrir la puerta.";
        return;
    }

    if (casillaDestino == SALIDA && tesorosRecolectados < tesorosTotales)
    {
        mensaje = "Debes recoger todos los tesoros antes de salir.";
        return;
    }

    jugadorX = destinoX;
    jugadorY = destinoY;

    movimientos++;

    ProcesarCasilla(casillaDestino);
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
            else if (simbolo == SUELO_FICHERO)
            {
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

void ProcesarCasilla(char casilla)
{
    if (casilla == TESORO)
    {
        tesorosRecolectados++;
        tesorosTotalesPartida++;
        mapa[jugadorY, jugadorX] = VACIO;
        mensaje = "Has recogido un tesoro. Tesoros recogidos: " + tesorosRecolectados + "/" + tesorosTotales;
    }
    else if (casilla == BOMBA)
    {
        bombas++;
        mapa[jugadorY, jugadorX] = VACIO;
        mensaje = "Has recogido una bomba. Bombas disponibles: " + bombas;
    }
    else if (casilla == LLAVE)
    {
        tieneLlave = true;
        mapa[jugadorY, jugadorX] = VACIO;
        mensaje = "Has recogido una llave. Ahora puedes abrir puertas.";
    }
    else if (casilla == TRAMPA || casilla == TRAMPA_INVISIBLE)
    {
        vidas--;

        if (vidas <= 0)
        {
            jugando = false;
            mensaje = "¡Has perdido todas tus vidas! Fin del juego.";
            return;
        }

        jugadorX = 1;
        jugadorY = 1;

        mensaje = "¡Has caído en una trampa! Has perdido una vida. Vidas restantes: " + vidas;
    }
    else if (casilla == SALIDA)
    {
        nivelActual++;
        debeCargarNivel = true;
        mensaje = "¡Has encontrado la salida! Pasando al siguiente nivel.";
    }
}

void GuardarPuntuacion(string nombre, int puntuacion, int tiempo, int movimientos, string resultadoFinal)
{
    string linea = $"{nombre},{puntuacion},{tiempo},{movimientos},{resultadoFinal}";

    File.AppendAllText(FICHERO_RANKING, linea + Environment.NewLine);
}

string ObtenerResultadoFinal()
{
    if (victoriaFinal)
    {
        return "Victoria";
    }
    else if (vidas <= 0)
    {
        return "Derrota";
    }
    else
    {
        return "Rendición";
    }
}
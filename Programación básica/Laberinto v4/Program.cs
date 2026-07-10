using System;
using System.IO;

// ====================================================================
// JUEGO: EL LABERINTO DE LOS TESOROS - VERSIÓN CON FICHEROS
// ====================================================================
//
// Conceptos trabajados:
//
// - Arrays bidimensionales char[,]
// - Bucles for
// - Condicionales if / else if / else
// - switch
// - Métodos sencillos
// - Constantes
// - Variables booleanas
// - Contadores
// - Control básico de teclado
// - Lectura de ficheros con File.ReadAllLines()
// - Escritura de ficheros con File.WriteAllLines()
// - Añadir texto a un fichero con File.AppendAllText()
// - Comprobar si existe un fichero con File.Exists()
// - Comprobar si existe una carpeta con Directory.Exists()
// - Crear carpetas con Directory.CreateDirectory()
// - Separar datos del programa usando ficheros externos
//
// Nuevas funcionalidades:
//
// - Los niveles se cargan desde ficheros .txt
// - Si los ficheros de niveles no existen, el programa los crea
// - El jugador puede modificar los mapas sin tocar el código
// - Al final de la partida se guarda la puntuación en ranking.txt
// - El ranking se muestra ordenado de mayor a menor puntuación
//
// ====================================================================


// ====================================================================
// 1. CONSTANTES DEL MAPA
// ====================================================================
//
// Usamos constantes para no escribir directamente caracteres como '#',
// '*', 'T' o 'B' por todo el código.
//
// Esto hace que el programa sea más legible y más fácil de modificar.

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

// Este símbolo solo se usa dentro de los ficheros de nivel.
// Indica dónde empieza el jugador.
// En el mapa interno del juego se convertirá en espacio vacío.
const char INICIO_JUGADOR = 'J';

// Este símbolo representa visualmente al jugador en pantalla.
const char JUGADOR = '@';


// ====================================================================
// 2. CONSTANTES DE FICHEROS
// ====================================================================
//
// Los niveles estarán dentro de una carpeta llamada "niveles".
// El ranking se guardará en un fichero llamado "ranking.txt".

const string CARPETA_NIVELES = "niveles";
const string FICHERO_RANKING = "ranking.txt";


// ====================================================================
// 3. VARIABLES PRINCIPALES DEL JUEGO
// ====================================================================

// Controla si el bucle principal debe seguir funcionando.
bool jugando = true;

// Se pondrá a true cuando el jugador complete todos los niveles.
bool victoriaFinal = false;

// Nivel actual en el que está el jugador.
int nivelActual = 1;

// Número total de niveles del juego.
int nivelesTotales = 3;

// Vidas disponibles al empezar la partida.
int vidas = 3;

// Las bombas se mantienen entre niveles.
int bombas = 0;

// Indica si el jugador tiene la llave del nivel actual.
// En esta versión, la llave se reinicia en cada nivel.
bool tieneLlave = false;

// Coordenadas del jugador dentro del mapa.
// X representa la columna.
// Y representa la fila.
int jugadorX = 1;
int jugadorY = 1;

// Tesoros recogidos en el nivel actual.
int tesorosRecolectados = 0;

// Tesoros totales que hay en el nivel actual.
// Se calculan automáticamente recorriendo el mapa.
int tesorosTotales = 0;

// Tesoros recogidos durante toda la partida.
// Sirve para calcular la puntuación final.
int tesorosTotalesPartida = 0;

// Número de movimientos válidos realizados por el jugador.
int movimientos = 0;

// Este mensaje se muestra debajo del mapa.
// Sirve para informar al jugador de lo que acaba de pasar.
string mensaje = "Recoge todos los tesoros y busca la salida.";

// Guarda el momento en que empieza la partida.
DateTime tiempoInicio = DateTime.Now;

// El mapa empieza vacío.
// Después se cargará desde un fichero .txt.
char[,] mapa = new char[0, 0];

// Indica si al comenzar una vuelta del bucle hay que cargar un nuevo nivel.
bool debeCargarNivel = true;


// ====================================================================
// 4. PREPARACIÓN INICIAL DE FICHEROS
// ====================================================================
//
// Antes de empezar el juego, comprobamos si existen los ficheros de nivel.
//
// Si no existen, los creamos automáticamente.
//
// - niveles/nivel1.txt
// - niveles/nivel2.txt
// - niveles/nivel3.txt
//
// y modificar los laberintos.

CrearFicherosDeNivelesSiNoExisten();


// ====================================================================
// 5. BUCLE PRINCIPAL DEL JUEGO
// ====================================================================

while (jugando)
{
    // ------------------------------------------------------------
    // CARGAR NIVEL
    // ------------------------------------------------------------
    //
    // Si acabamos de empezar o acabamos de pasar de nivel,
    // cargamos el mapa correspondiente desde un fichero .txt.

    if (debeCargarNivel)
    {
        CargarNivel();

        // Si CargarNivel ha detectado que ya no quedan más niveles,
        // jugando será false y salimos del bucle.
        if (!jugando)
        {
            break;
        }
    }

    // ------------------------------------------------------------
    // DIBUJAR PANTALLA
    // ------------------------------------------------------------

    DibujarInterfaz();

    // ------------------------------------------------------------
    // LEER TECLA
    // ------------------------------------------------------------
    //
    // Console.ReadKey(true) lee una tecla sin mostrarla por pantalla.

    ConsoleKeyInfo tecla = Console.ReadKey(true);

    // Estas variables representan la posición a la que quiere moverse
    // el jugador. Al principio son iguales a la posición actual.
    int destinoX = jugadorX;
    int destinoY = jugadorY;

    // ------------------------------------------------------------
    // TECLA ESCAPE: RENDIRSE
    // ------------------------------------------------------------

    if (tecla.Key == ConsoleKey.Escape)
    {
        jugando = false;
        mensaje = "Has decidido rendirte.";
        continue;
    }

    // ------------------------------------------------------------
    // TECLA ESPACIO: USAR BOMBA
    // ------------------------------------------------------------
    //
    // Usar una bomba no cuenta como movimiento.

    if (tecla.Key == ConsoleKey.Spacebar)
    {
        UsarBomba();
        continue;
    }

    // ------------------------------------------------------------
    // CALCULAR NUEVA POSICIÓN SEGÚN LA TECLA
    // ------------------------------------------------------------

    bool esMovimiento = CalcularDestino(tecla.Key, ref destinoX, ref destinoY);

    // Si el jugador pulsa una tecla que no es válida, no hacemos nada.
    if (!esMovimiento)
    {
        mensaje = "Tecla no válida. Usa las flechas, ESPACIO o ESC.";
        continue;
    }

    // ------------------------------------------------------------
    // INTENTAR MOVER AL JUGADOR
    // ------------------------------------------------------------

    IntentarMoverJugador(destinoX, destinoY);
}


// ====================================================================
// 6. PANTALLA FINAL Y RANKING
// ====================================================================

Console.Clear();

TimeSpan tiempoJugado = DateTime.Now - tiempoInicio;
int segundosTotales = (int)tiempoJugado.TotalSeconds;

// Calculamos una puntuación sencilla.
//
// La fórmula se puede modificar fácilmente en clase.
//
// Suma:
// - 100 puntos por cada tesoro recogido.
// - 50 puntos por cada vida restante.
// - 25 puntos por cada bomba guardada.
//
// Resta:
// - 1 punto por cada segundo usado.
// - 1 punto por cada movimiento realizado.

int puntuacion = tesorosTotalesPartida * 100
                 + vidas * 50
                 + bombas * 25
                 - segundosTotales
                 - movimientos;

// Evitamos mostrar puntuaciones negativas.
if (puntuacion < 0)
{
    puntuacion = 0;
}

// Mostramos la pantalla final correspondiente.
if (victoriaFinal)
{
    Console.WriteLine("==================================================");
    Console.WriteLine(" ¡VICTORIA ABSOLUTA! Has escapado del laberinto");
    Console.WriteLine("==================================================");
    Console.WriteLine($"Has completado los {nivelesTotales} niveles.");
    Console.WriteLine($"Tiempo total invertido: {segundosTotales} segundos.");
    Console.WriteLine($"Movimientos realizados: {movimientos}");
    Console.WriteLine($"Tesoros totales recogidos: {tesorosTotalesPartida}");
    Console.WriteLine($"Vidas sobrantes: {vidas}");
    Console.WriteLine($"Bombas guardadas: {bombas}");
    Console.WriteLine($"Puntuación final: {puntuacion}");

    if (segundosTotales < 30)
    {
        Console.WriteLine("\n¡Eres muy rápido! Un tiempo excelente.");
    }
    else if (movimientos < 100)
    {
        Console.WriteLine("\n¡Has sido muy eficiente explorando el laberinto!");
    }
}
else if (vidas <= 0)
{
    Console.WriteLine("==================================================");
    Console.WriteLine(" GAME OVER ");
    Console.WriteLine("==================================================");
    Console.WriteLine("Has caído en demasiadas trampas y te has quedado sin vidas...");
    Console.WriteLine($"Tiempo jugado: {segundosTotales} segundos.");
    Console.WriteLine($"Movimientos realizados: {movimientos}");
    Console.WriteLine($"Tesoros totales recogidos: {tesorosTotalesPartida}");
    Console.WriteLine($"Puntuación final: {puntuacion}");
}
else
{
    Console.WriteLine("==================================================");
    Console.WriteLine(" JUEGO CANCELADO ");
    Console.WriteLine("==================================================");
    Console.WriteLine("Te has rendido ante el laberinto...");
    Console.WriteLine($"Tiempo jugado: {segundosTotales} segundos.");
    Console.WriteLine($"Movimientos realizados: {movimientos}");
    Console.WriteLine($"Tesoros totales recogidos: {tesorosTotalesPartida}");
    Console.WriteLine($"Puntuación final: {puntuacion}");
}

// Guardamos la puntuación en el ranking.
Console.Write("\nIntroduce tu nombre para guardar la puntuación: ");
string nombreJugador = Console.ReadLine() ?? "";

// Si el usuario no escribe nada, usamos un nombre por defecto.
if (nombreJugador.Trim() == "")
{
    nombreJugador = "Jugador";
}

// Obtenemos el resultado final en forma de texto.
string resultadoFinal = ObtenerResultadoFinal();

// Guardamos la partida en el fichero ranking.txt.
GuardarPuntuacion(nombreJugador, puntuacion, segundosTotales, movimientos, resultadoFinal);

// Mostramos el ranking ordenado.
MostrarRanking();

Console.WriteLine("\nPresiona cualquier tecla para cerrar el programa.");
Console.ReadKey();


// ====================================================================
// 7. MÉTODOS DEL JUEGO
// ====================================================================


// --------------------------------------------------------------------
// CargarNivel
// --------------------------------------------------------------------
//
// Carga el mapa correspondiente al nivel actual.
//
// Antes, los mapas estaban escritos directamente dentro del código.
// Ahora se cargan desde ficheros externos:
//
// - niveles/nivel1.txt
// - niveles/nivel2.txt
// - niveles/nivel3.txt
//

void CargarNivel()
{
    // Si el nivel actual supera el número de niveles,
    // significa que el jugador ha completado el juego.
    if (nivelActual > nivelesTotales)
    {
        victoriaFinal = true;
        jugando = false;
        return;
    }

    // Reiniciamos valores propios del nivel actual.
    tesorosRecolectados = 0;
    tieneLlave = false;

    // Construimos la ruta del fichero correspondiente.
    //
    // Por ejemplo:
    //
    // nivelActual = 1  ->  niveles/nivel1.txt
    // nivelActual = 2  ->  niveles/nivel2.txt
    // nivelActual = 3  ->  niveles/nivel3.txt

    string nombreFichero = "nivel" + nivelActual + ".txt";
    string rutaNivel = Path.Combine(CARPETA_NIVELES, nombreFichero);

    try
    {
        // Cargamos el mapa desde el fichero.
        mapa = CargarMapaDesdeFichero(rutaNivel);
    }
    catch (Exception error)
    {
        // Si hay un error en el fichero, mostramos el problema y cancelamos.
        Console.Clear();
        Console.WriteLine("ERROR AL CARGAR EL NIVEL");
        Console.WriteLine("------------------------");
        Console.WriteLine(error.Message);
        Console.WriteLine();
        Console.WriteLine("Revisa el fichero del nivel y vuelve a ejecutar el programa.");
        Console.WriteLine("\nPresiona cualquier tecla para cerrar.");
        Console.ReadKey();

        jugando = false;
        return;
    }

    // Contamos automáticamente cuántos tesoros hay en el mapa.
    tesorosTotales = ContarTesoros();

    // Ya no hace falta cargar de nuevo el nivel hasta que el jugador
    // llegue a la salida.
    debeCargarNivel = false;

    mensaje = $"Nivel {nivelActual} cargado desde {rutaNivel}.";
}


// --------------------------------------------------------------------
// CargarMapaDesdeFichero
// --------------------------------------------------------------------
//
// Lee un fichero .txt y lo convierte en un array bidimensional char[,].
//
// En el fichero usamos estos símbolos:
//
// # = pared
// . = suelo vacío
// J = posición inicial del jugador
// * = tesoro
// T = trampa visible
// X = trampa invisible
// B = bomba
// L = llave
// P = puerta cerrada
// S = salida
//
// Importante:
//
// En el fichero usamos el punto '.' para representar suelo vacío.
// Esto es más cómodo que usar espacios, porque los espacios al final de
// una línea pueden perderse o no verse bien en algunos editores.
//
// Al cargar el mapa, el programa convierte cada '.' en un espacio ' '.

char[,] CargarMapaDesdeFichero(string rutaFichero)
{
    // Primero comprobamos si existe el fichero.
    if (!File.Exists(rutaFichero))
    {
        throw new Exception("No existe el fichero: " + rutaFichero);
    }

    // Leemos todas las líneas del fichero.
    string[] lineas = File.ReadAllLines(rutaFichero);

    // El fichero no puede estar vacío.
    if (lineas.Length == 0)
    {
        throw new Exception("El fichero está vacío: " + rutaFichero);
    }

    // El número de filas es el número de líneas del fichero.
    int filas = lineas.Length;

    // El número de columnas será la longitud de la primera línea.
    int columnas = lineas[0].Length;

    // Comprobamos que la primera línea no esté vacía.
    if (columnas == 0)
    {
        throw new Exception("La primera línea del fichero está vacía.");
    }

    // Creamos el array bidimensional.
    char[,] nuevoMapa = new char[filas, columnas];

    // Usamos esta variable para comprobar si el fichero tiene una posición
    // inicial para el jugador.
    bool inicioEncontrado = false;

    // Recorremos todas las filas del fichero.
    for (int fila = 0; fila < filas; fila++)
    {
        // Todos los mapas deben ser rectangulares.
        //
        // Eso significa que todas las filas deben tener el mismo número
        // de columnas.
        //
        // Si una fila tiene más o menos caracteres, el mapa no se puede
        // cargar correctamente como char[,].
        if (lineas[fila].Length != columnas)
        {
            throw new Exception(
                "El mapa no es rectangular. " +
                "La fila " + (fila + 1) + " tiene una longitud diferente."
            );
        }

        // Recorremos todas las columnas de la fila actual.
        for (int columna = 0; columna < columnas; columna++)
        {
            char simbolo = lineas[fila][columna];

            // Comprobamos que el símbolo sea válido.
            if (!EsSimboloValidoEnFichero(simbolo))
            {
                throw new Exception(
                    "Símbolo no válido en el mapa: '" + simbolo + "'. " +
                    "Fila: " + (fila + 1) + ", columna: " + (columna + 1)
                );
            }

            // Si encontramos el símbolo J, significa que aquí empieza
            // el jugador.
            if (simbolo == INICIO_JUGADOR)
            {
                // Solo permitimos una posición inicial.
                if (inicioEncontrado)
                {
                    throw new Exception("El mapa tiene más de una posición inicial 'J'.");
                }

                jugadorX = columna;
                jugadorY = fila;
                inicioEncontrado = true;

                // En el mapa interno, la casilla inicial se guarda como vacío.
                nuevoMapa[fila, columna] = VACIO;
            }
            else if (simbolo == SUELO_FICHERO)
            {
                // Convertimos el punto del fichero en un espacio vacío.
                nuevoMapa[fila, columna] = VACIO;
            }
            else
            {
                // El resto de símbolos se copian tal cual.
                nuevoMapa[fila, columna] = simbolo;
            }
        }
    }

    // Si el fichero no tiene J, usamos la posición clásica (1, 1).
    //
    // Esto permite que los mapas antiguos sigan funcionando.
    if (!inicioEncontrado)
    {
        jugadorX = 1;
        jugadorY = 1;

        // Comprobamos que esa posición exista y no sea una pared.
        if (filas <= 1 || columnas <= 1 || nuevoMapa[1, 1] == PARED)
        {
            throw new Exception(
                "El mapa no tiene posición inicial 'J' y la posición (1,1) no es válida."
            );
        }
    }

    return nuevoMapa;
}


// --------------------------------------------------------------------
// EsSimboloValidoEnFichero
// --------------------------------------------------------------------
//
// Comprueba si un carácter del fichero de nivel está permitido.
//
// Por ejemplo, si alguien escribe una letra que no existe en la leyenda,
// el programa mostrará un error claro.

bool EsSimboloValidoEnFichero(char simbolo)
{
    return simbolo == PARED ||
           simbolo == SUELO_FICHERO ||
           simbolo == VACIO ||
           simbolo == TESORO ||
           simbolo == TRAMPA ||
           simbolo == TRAMPA_INVISIBLE ||
           simbolo == BOMBA ||
           simbolo == SALIDA ||
           simbolo == LLAVE ||
           simbolo == PUERTA ||
           simbolo == INICIO_JUGADOR;
}


// --------------------------------------------------------------------
// DibujarInterfaz
// --------------------------------------------------------------------
//
// Limpia la consola y dibuja:
//
// - Título.
// - Datos del jugador.
// - Mapa.
// - Instrucciones.
// - Mensaje de estado.

void DibujarInterfaz()
{
    Console.Clear();

    Console.WriteLine("=== EL LABERINTO DE LOS TESOROS ===");
    Console.WriteLine($"Nivel: {nivelActual} / {nivelesTotales}");
    Console.WriteLine($"Vidas: {vidas} | Bombas: {bombas} | Llave: {(tieneLlave ? "Sí" : "No")}");
    Console.WriteLine($"Tesoros del nivel: {tesorosRecolectados} / {tesorosTotales}");
    Console.WriteLine($"Movimientos: {movimientos}");
    Console.WriteLine("-----------------------------------");

    // Recorremos todas las filas del mapa.
    for (int fila = 0; fila < mapa.GetLength(0); fila++)
    {
        // Recorremos todas las columnas del mapa.
        for (int columna = 0; columna < mapa.GetLength(1); columna++)
        {
            // Si la posición actual del bucle coincide con la posición
            // del jugador, dibujamos al jugador.
            if (fila == jugadorY && columna == jugadorX)
            {
                Console.Write(JUGADOR + " ");
            }
            else
            {
                char casilla = mapa[fila, columna];

                // Las trampas invisibles se dibujan como si fueran espacios vacíos.
                // El jugador no sabe dónde están hasta que cae en una.
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

    Console.WriteLine("-----------------------------------");
    Console.WriteLine("[FLECHAS] Moverse");
    Console.WriteLine("[ESPACIO] Usar bomba");
    Console.WriteLine("[ESC] Rendirse");
    Console.WriteLine();
    Console.WriteLine($"{TESORO} = Tesoro | {SALIDA} = Salida | {TRAMPA} = Trampa | {BOMBA} = Bomba | {LLAVE} = Llave | {PUERTA} = Puerta");
    Console.WriteLine("Las trampas invisibles no se muestran en el mapa.");
    Console.WriteLine();
    Console.WriteLine("Mensaje: " + mensaje);
}


// --------------------------------------------------------------------
// CalcularDestino
// --------------------------------------------------------------------
//
// Recibe la tecla pulsada y modifica destinoX y destinoY.
//
// Usamos ref porque queremos que los cambios realizados dentro del método
// afecten a las variables originales.
//
// Devuelve true si la tecla era una flecha.
// Devuelve false si la tecla no era una tecla de movimiento.

bool CalcularDestino(ConsoleKey tecla, ref int destinoX, ref int destinoY)
{
    switch (tecla)
    {
        case ConsoleKey.UpArrow:
            destinoY--;
            return true;

        case ConsoleKey.DownArrow:
            destinoY++;
            return true;

        case ConsoleKey.LeftArrow:
            destinoX--;
            return true;

        case ConsoleKey.RightArrow:
            destinoX++;
            return true;

        default:
            return false;
    }
}


// --------------------------------------------------------------------
// IntentarMoverJugador
// --------------------------------------------------------------------
//
// Comprueba si el jugador puede moverse a la posición indicada.
//
// Este método se encarga de:
//
// - Comprobar límites del mapa.
// - Evitar atravesar paredes.
// - Evitar atravesar puertas sin llave.
// - Evitar salir del nivel sin todos los tesoros.
// - Mover al jugador.
// - Procesar lo que hay en la casilla destino.

void IntentarMoverJugador(int destinoX, int destinoY)
{
    // Primero comprobamos si la posición está dentro del mapa.
    if (!EstaDentroDelMapa(destinoX, destinoY))
    {
        mensaje = "No puedes salirte del mapa.";
        return;
    }

    // Obtenemos el contenido de la casilla a la que el jugador quiere ir.
    char casillaDestino = mapa[destinoY, destinoX];

    // Si hay una pared, el jugador no puede moverse.
    if (casillaDestino == PARED)
    {
        mensaje = "Hay una pared. No puedes pasar.";
        return;
    }

    // Si hay una puerta y el jugador no tiene llave, no puede pasar.
    if (casillaDestino == PUERTA && !tieneLlave)
    {
        mensaje = "La puerta está cerrada. Necesitas una llave.";
        return;
    }

    // Si la casilla es la salida pero faltan tesoros, no puede salir.
    if (casillaDestino == SALIDA && tesorosRecolectados < tesorosTotales)
    {
        mensaje = "Necesitas recoger todos los tesoros antes de salir.";
        return;
    }

    // Si hemos llegado hasta aquí, el movimiento es válido.
    jugadorX = destinoX;
    jugadorY = destinoY;

    // Aumentamos el contador de movimientos solo cuando el jugador
    // se mueve realmente.
    movimientos++;

    // Procesamos el efecto de la casilla.
    ProcesarCasilla(casillaDestino);
}


// --------------------------------------------------------------------
// ProcesarCasilla
// --------------------------------------------------------------------
//
// Ejecuta la acción correspondiente según la casilla que ha pisado
// el jugador.
//
// Ejemplos:
//
// - Si pisa un tesoro, lo recoge.
// - Si pisa una trampa, pierde una vida.
// - Si pisa una bomba, la añade al inventario.
// - Si pisa una llave, la recoge.
// - Si pisa una puerta con llave, la abre.
// - Si pisa la salida, pasa al siguiente nivel.

void ProcesarCasilla(char casillaDestino)
{
    if (casillaDestino == TESORO)
    {
        tesorosRecolectados++;
        tesorosTotalesPartida++;

        // Borramos el tesoro del mapa para que no pueda recogerse otra vez.
        mapa[jugadorY, jugadorX] = VACIO;

        mensaje = "Has recogido un tesoro.";
    }
    else if (casillaDestino == BOMBA)
    {
        bombas++;

        // Borramos la bomba del mapa porque ya está en el inventario.
        mapa[jugadorY, jugadorX] = VACIO;

        mensaje = "Has recogido una bomba.";
    }
    else if (casillaDestino == LLAVE)
    {
        tieneLlave = true;

        // Borramos la llave del mapa porque ya la tiene el jugador.
        mapa[jugadorY, jugadorX] = VACIO;

        mensaje = "Has recogido una llave. Ahora puedes abrir puertas.";
    }
    else if (casillaDestino == PUERTA)
    {
        // Si el jugador ha llegado aquí, significa que tenía la llave,
        // porque esa comprobación se hizo antes en IntentarMoverJugador.
        mapa[jugadorY, jugadorX] = VACIO;

        mensaje = "Has abierto la puerta.";
    }
    else if (casillaDestino == TRAMPA || casillaDestino == TRAMPA_INVISIBLE)
    {
        vidas--;

        // Mandamos al jugador al inicio del nivel.
        jugadorX = 1;
        jugadorY = 1;

        if (casillaDestino == TRAMPA_INVISIBLE)
        {
            mensaje = "¡Has caído en una trampa invisible! Pierdes una vida.";
        }
        else
        {
            mensaje = "¡Has caído en una trampa! Pierdes una vida.";
        }

        if (vidas <= 0)
        {
            jugando = false;
        }
    }
    else if (casillaDestino == SALIDA)
    {
        // Si el jugador llega aquí, ya tiene todos los tesoros,
        // porque la comprobación se hizo antes.
        nivelActual++;
        debeCargarNivel = true;

        mensaje = "Has encontrado la salida. Pasas al siguiente nivel.";
    }
    else
    {
        // Si la casilla estaba vacía, simplemente se mueve.
        mensaje = "Te has movido.";
    }
}


// --------------------------------------------------------------------
// UsarBomba
// --------------------------------------------------------------------
//
// La bomba destruye paredes cercanas al jugador.
//
// Afecta a esta zona:
//
//     X X X
//     X @ X
//     X X X
//
// Es decir, destruye paredes en un radio de 1 casilla alrededor
// del jugador.
//
// No destruye los bordes exteriores del mapa.
// Así evitamos que el jugador pueda salirse del laberinto.

void UsarBomba()
{
    if (bombas <= 0)
    {
        mensaje = "No tienes bombas.";
        return;
    }

    bombas--;

    int paredesDestruidas = 0;

    // Recorremos las filas cercanas al jugador.
    for (int fila = jugadorY - 1; fila <= jugadorY + 1; fila++)
    {
        // Recorremos las columnas cercanas al jugador.
        for (int columna = jugadorX - 1; columna <= jugadorX + 1; columna++)
        {
            // Comprobamos que la posición esté dentro del mapa.
            if (EstaDentroDelMapa(columna, fila))
            {
                // Evitamos destruir las paredes exteriores.
                bool esBordeExterior =
                    fila == 0 ||
                    fila == mapa.GetLength(0) - 1 ||
                    columna == 0 ||
                    columna == mapa.GetLength(1) - 1;

                // Solo destruimos paredes interiores.
                // No destruimos tesoros, trampas, bombas, llaves, puertas ni salidas.
                if (!esBordeExterior && mapa[fila, columna] == PARED)
                {
                    mapa[fila, columna] = VACIO;
                    paredesDestruidas++;
                }
            }
        }
    }

    if (paredesDestruidas > 0)
    {
        mensaje = $"Has usado una bomba y has destruido {paredesDestruidas} pared/es cercana/s.";
    }
    else
    {
        mensaje = "Has usado una bomba, pero no había paredes cercanas.";
    }
}


// --------------------------------------------------------------------
// EstaDentroDelMapa
// --------------------------------------------------------------------
//
// Devuelve true si las coordenadas están dentro del mapa.
// Devuelve false si se salen por arriba, abajo, izquierda o derecha.

bool EstaDentroDelMapa(int x, int y)
{
    return y >= 0 &&
           y < mapa.GetLength(0) &&
           x >= 0 &&
           x < mapa.GetLength(1);
}


// --------------------------------------------------------------------
// ContarTesoros
// --------------------------------------------------------------------
//
// Recorre todo el mapa y cuenta cuántos tesoros hay.
//
// Esto evita tener que escribir manualmente:
//
//     tesorosTotales = 4;
//
// Si modificamos el mapa y añadimos o quitamos tesoros,
// el programa seguirá funcionando correctamente.

int ContarTesoros()
{
    int total = 0;

    for (int fila = 0; fila < mapa.GetLength(0); fila++)
    {
        for (int columna = 0; columna < mapa.GetLength(1); columna++)
        {
            if (mapa[fila, columna] == TESORO)
            {
                total++;
            }
        }
    }

    return total;
}


// ====================================================================
// 8. MÉTODOS PARA EL RANKING
// ====================================================================


// --------------------------------------------------------------------
// ObtenerResultadoFinal
// --------------------------------------------------------------------
//
// Devuelve el resultado final de la partida como texto.
//
// Este texto será guardado en ranking.txt.

string ObtenerResultadoFinal()
{
    if (victoriaFinal)
    {
        return "Victoria";
    }
    else if (vidas <= 0)
    {
        return "Game Over";
    }
    else
    {
        return "Rendición";
    }
}


// --------------------------------------------------------------------
// GuardarPuntuacion
// --------------------------------------------------------------------
//
// Guarda la puntuación de la partida en el fichero ranking.txt.
//
// Cada partida se guarda en una línea con este formato:
//
// Nombre;Puntuacion;Tiempo;Movimientos;Resultado
//
// Ejemplo:
//
// Fernando;725;43;96;Victoria
//
// Usamos punto y coma ; como separador.
// Luego, al leer el ranking, podremos separar los datos usando Split(';').

void GuardarPuntuacion(string nombre, int puntuacion, int tiempo, int movimientosRealizados, string resultado)
{
    // Quitamos espacios al principio y al final del nombre.
    nombre = nombre.Trim();

    // Si el nombre está vacío, usamos un valor por defecto.
    if (nombre == "")
    {
        nombre = "Jugador";
    }

    // Evitamos que el nombre tenga punto y coma, porque usamos ; como separador.
    nombre = nombre.Replace(";", ",");

    // Creamos la línea que se guardará en el fichero.
    string linea = nombre + ";" +
                   puntuacion + ";" +
                   tiempo + ";" +
                   movimientosRealizados + ";" +
                   resultado;

    // AppendAllText añade texto al final del fichero.
    //
    // Si ranking.txt no existe, lo crea automáticamente.
    //
    // Environment.NewLine añade el salto de línea correcto según el sistema.
    File.AppendAllText(FICHERO_RANKING, linea + Environment.NewLine);

    Console.WriteLine("\nPuntuación guardada correctamente en " + FICHERO_RANKING + ".");
}


// --------------------------------------------------------------------
// MostrarRanking
// --------------------------------------------------------------------
//
// Lee el fichero ranking.txt y muestra las 10 mejores puntuaciones.
//
// Para no usar conceptos demasiado avanzados, hacemos una ordenación
// sencilla con bucles.
//
// Esta ordenación compara las puntuaciones y coloca primero las mayores.

void MostrarRanking()
{
    Console.WriteLine();
    Console.WriteLine("============== TOP 10 RANKING ==============");

    // Si el fichero no existe, todavía no hay ranking.
    if (!File.Exists(FICHERO_RANKING))
    {
        Console.WriteLine("Todavía no hay puntuaciones guardadas.");
        return;
    }

    // Leemos todas las líneas del ranking.
    string[] lineas = File.ReadAllLines(FICHERO_RANKING);

    if (lineas.Length == 0)
    {
        Console.WriteLine("El ranking está vacío.");
        return;
    }

    // Creamos arrays para guardar los datos de las partidas.
    //
    // Usamos el tamaño máximo posible: lineas.Length.
    // Puede que alguna línea esté mal escrita, así que usaremos
    // totalPartidasValidas para saber cuántas líneas se han podido leer bien.

    string[] nombres = new string[lineas.Length];
    int[] puntuaciones = new int[lineas.Length];
    int[] tiempos = new int[lineas.Length];
    int[] movimientosRanking = new int[lineas.Length];
    string[] resultados = new string[lineas.Length];

    int totalPartidasValidas = 0;

    // Recorremos todas las líneas del fichero.
    for (int i = 0; i < lineas.Length; i++)
    {
        // Cada línea debería tener este formato:
        //
        // Nombre;Puntuacion;Tiempo;Movimientos;Resultado
        //
        // Split(';') separa la línea en partes.

        string[] datos = lineas[i].Split(';');

        // Comprobamos que haya exactamente 5 datos.
        if (datos.Length == 5)
        {
            string nombre = datos[0];
            string textoPuntuacion = datos[1];
            string textoTiempo = datos[2];
            string textoMovimientos = datos[3];
            string resultado = datos[4];

            // TryParse intenta convertir un texto en número.
            //
            // Si puede convertirlo, devuelve true.
            // Si no puede, devuelve false.
            //
            // Esto evita que el programa falle si alguien modifica
            // ranking.txt a mano y escribe algo incorrecto.

            bool puntuacionValida = int.TryParse(textoPuntuacion, out int puntos);
            bool tiempoValido = int.TryParse(textoTiempo, out int tiempo);
            bool movimientosValidos = int.TryParse(textoMovimientos, out int movs);

            if (puntuacionValida && tiempoValido && movimientosValidos)
            {
                nombres[totalPartidasValidas] = nombre;
                puntuaciones[totalPartidasValidas] = puntos;
                tiempos[totalPartidasValidas] = tiempo;
                movimientosRanking[totalPartidasValidas] = movs;
                resultados[totalPartidasValidas] = resultado;

                totalPartidasValidas++;
            }
        }
    }

    if (totalPartidasValidas == 0)
    {
        Console.WriteLine("No hay partidas válidas en el ranking.");
        return;
    }

    // ------------------------------------------------------------
    // ORDENAR EL RANKING
    // ------------------------------------------------------------
    //
    // Ordenamos de mayor a menor puntuación.
    //
    // Si dos jugadores tienen la misma puntuación, ponemos primero
    // al que haya tardado menos tiempo.
    //
    // Esta ordenación es sencilla de entender porque solo usa bucles
    // y variables temporales.

    for (int i = 0; i < totalPartidasValidas - 1; i++)
    {
        for (int j = i + 1; j < totalPartidasValidas; j++)
        {
            bool debeIntercambiar =
                puntuaciones[j] > puntuaciones[i] ||
                puntuaciones[j] == puntuaciones[i] && tiempos[j] < tiempos[i];

            if (debeIntercambiar)
            {
                // Intercambiamos nombres.
                string tempNombre = nombres[i];
                nombres[i] = nombres[j];
                nombres[j] = tempNombre;

                // Intercambiamos puntuaciones.
                int tempPuntuacion = puntuaciones[i];
                puntuaciones[i] = puntuaciones[j];
                puntuaciones[j] = tempPuntuacion;

                // Intercambiamos tiempos.
                int tempTiempo = tiempos[i];
                tiempos[i] = tiempos[j];
                tiempos[j] = tempTiempo;

                // Intercambiamos movimientos.
                int tempMovimientos = movimientosRanking[i];
                movimientosRanking[i] = movimientosRanking[j];
                movimientosRanking[j] = tempMovimientos;

                // Intercambiamos resultados.
                string tempResultado = resultados[i];
                resultados[i] = resultados[j];
                resultados[j] = tempResultado;
            }
        }
    }

    // Mostramos como máximo las 10 mejores partidas.
    int limite = totalPartidasValidas;

    if (limite > 10)
    {
        limite = 10;
    }

    for (int i = 0; i < limite; i++)
    {
        Console.WriteLine(
            $"{i + 1}. {nombres[i]} - " +
            $"{puntuaciones[i]} puntos - " +
            $"{tiempos[i]}s - " +
            $"{movimientosRanking[i]} movimientos - " +
            $"{resultados[i]}"
        );
    }
}


// ====================================================================
// 9. MÉTODOS PARA CREAR LOS FICHEROS DE NIVELES
// ====================================================================


// --------------------------------------------------------------------
// CrearFicherosDeNivelesSiNoExisten
// --------------------------------------------------------------------
//
// Este método prepara los ficheros del juego.
//
// Hace dos cosas:
//
// 1. Crea la carpeta "niveles" si no existe.
// 2. Crea nivel1.txt, nivel2.txt y nivel3.txt si no existen.
//
// Importante:
//
// Si un fichero ya existe, NO lo sobrescribe.

void CrearFicherosDeNivelesSiNoExisten()
{
    // Si la carpeta de niveles no existe, la creamos.
    if (!Directory.Exists(CARPETA_NIVELES))
    {
        Directory.CreateDirectory(CARPETA_NIVELES);
    }

    // Creamos cada nivel solo si no existe.
    CrearNivelSiNoExiste("nivel1.txt", ObtenerTextoNivel1());
    CrearNivelSiNoExiste("nivel2.txt", ObtenerTextoNivel2());
    CrearNivelSiNoExiste("nivel3.txt", ObtenerTextoNivel3());
}


// --------------------------------------------------------------------
// CrearNivelSiNoExiste
// --------------------------------------------------------------------
//
// Recibe el nombre de un fichero y las líneas del mapa.
//
// Si el fichero no existe, lo crea.
// Si ya existe, no hace nada.

void CrearNivelSiNoExiste(string nombreFichero, string[] lineasNivel)
{
    string ruta = Path.Combine(CARPETA_NIVELES, nombreFichero);

    if (!File.Exists(ruta))
    {
        File.WriteAllLines(ruta, lineasNivel);
    }
}


// --------------------------------------------------------------------
// ObtenerTextoNivel1
// --------------------------------------------------------------------
//
// Devuelve el contenido inicial de nivel1.txt.
//
// Cada string representa una fila del mapa.
//
// Leyenda:
//
// # = pared
// . = suelo vacío
// J = posición inicial del jugador
// * = tesoro
// T = trampa visible
// X = trampa invisible
// B = bomba
// L = llave
// P = puerta cerrada
// S = salida

string[] ObtenerTextoNivel1()
{
    return new string[]
    {
        "###########",
        "#J....L..*#",
        "#.###.###.#",
        "#.*...B...#",
        "#.###.###.#",
        "#...T...*.#",
        "###.#######",
        "#*....X.PS#",
        "###########"
    };
}


// --------------------------------------------------------------------
// ObtenerTextoNivel2
// --------------------------------------------------------------------
//
// Devuelve el contenido inicial de nivel2.txt.

string[] ObtenerTextoNivel2()
{
    return new string[]
    {
        "#############",
        "#J..T.LB..*.#",
        "#.#########.#",
        "#.*.......#.#",
        "#.###.###.#.#",
        "#...#.*.X.#.#",
        "###.#.###.#.#",
        "#*..T.....PS#",
        "#############"
    };
}


// --------------------------------------------------------------------
// ObtenerTextoNivel3
// --------------------------------------------------------------------
//
// Devuelve el contenido inicial de nivel3.txt.

string[] ObtenerTextoNivel3()
{
    return new string[]
    {
        "###############",
        "#J..#*..T.BL..#",
        "#.#.#.#######.#",
        "#.#...#*..T.#.#",
        "#.#.###.###.#.#",
        "#...B...#.*.#.#",
        "#####.#####.#.#",
        "#*..X.....T.PS#",
        "###############"
    };
}
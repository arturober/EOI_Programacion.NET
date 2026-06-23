// ====================================================================
// JUEGO: 3 EN RAYA (Humano vs Máquina)
// VERSIÓN CON VALIDACIÓN DE ENTRADAS PARA EL HUMANO
// El ordenador elige al azar una casilla vacía para colocar su ficha.
// Conceptos aplicados: Arrays 2D, Bucles, Condicionales, TryParse
// y generación de números aleatorios (Random).
// ====================================================================

// 1. VARIABLES GLOBALES DEL JUEGO
bool jugando = true;
bool hayGanador = false;
char turno = 'X';      // Empieza el jugador humano ('X')
int turnosJugados = 0; 
const int TURNOS_MAXIMOS = 9; // Definimos el número máximo de turnos como constante

// Generador de números al azar para el "cerebro" de la máquina
Random generadorAzar = new Random();

char[,] tablero = {
    { '-', '-', '-' },
    { '-', '-', '-' },
    { '-', '-', '-' } 
};

// 2. BUCLE PRINCIPAL DEL JUEGO
while (jugando)
{
    Console.Clear();
    Console.WriteLine("=== 3 EN RAYA: HUMANO VS MÁQUINA ===\n");
    
    // --- DIBUJAR EL TABLERO ---
    for (int fila = 0; fila < 3; fila++)
    {
        for (int columna = 0; columna < 3; columna++)
        {
            Console.Write(tablero[fila, columna] + " ");
        }
        Console.WriteLine(); 
    }
    Console.WriteLine(); 

    // 3. EVALUAR ESTADO DEL JUEGO
    if (hayGanador)
    {
        if (turno == 'X')
            Console.WriteLine("¡ENHORABUENA! Has ganado a la máquina.");
        else
            Console.WriteLine("¡OH NO! La máquina te ha ganado.");
        break; 
    }
    else if (turnosJugados == TURNOS_MAXIMOS) // Usamos la constante aquí
    {
        Console.WriteLine("¡Es un EMPATE! Ninguno ha conseguido el 3 en raya.");
        break; 
    }

    // 4. TURNO: HUMANO O MÁQUINA
    int filaElegida = -1;
    int columnaElegida = -1;
    bool movimientoValido = false;

    if (turno == 'X')
    {
        // ==========================
        // TURNO DEL HUMANO
        // ==========================
        Console.WriteLine("Es tu turno (X).");
        
        while (!movimientoValido)
        {
            Console.Write("Introduce la fila (0, 1 o 2): ");
            string entradaFila = Console.ReadLine()!;
            
            Console.Write("Introduce la columna (0, 1 o 2): ");
            string entradaColumna = Console.ReadLine()!;

            // Validamos que lo introducido sean números
            if (int.TryParse(entradaFila, out filaElegida) && int.TryParse(entradaColumna, out columnaElegida))
            {
                // Validamos que no se salga del tablero
                if ((filaElegida >= 0 && filaElegida <= 2) && (columnaElegida >= 0 && columnaElegida <= 2))
                {
                    // Validamos que la casilla esté vacía
                    if (tablero[filaElegida, columnaElegida] == '-')
                    {
                        movimientoValido = true; 
                    }
                    else
                    {
                        Console.WriteLine("¡Error! Esa casilla ya está ocupada.\n");
                    }
                }
                else
                {
                    Console.WriteLine("¡Error! Las coordenadas deben ser 0, 1 o 2.\n");
                }
            }
            else
            {
                Console.WriteLine("¡Error! Debes escribir un número válido.\n");
            }
        }
    }
    else
    {
        // ==========================
        // TURNO DE LA MÁQUINA
        // ==========================
        Console.WriteLine("Turno de la Máquina (O)... Pensando...");
        
        while (!movimientoValido)
        {
            // La máquina prueba suerte eligiendo números del 0 al 2
            filaElegida = generadorAzar.Next(0, 3);
            columnaElegida = generadorAzar.Next(0, 3);

            // Si tiene suerte y la casilla está libre, hace su movimiento
            if (tablero[filaElegida, columnaElegida] == '-')
            {
                Console.WriteLine($"La máquina ha elegido la fila {filaElegida} y columna {columnaElegida}.");
                Console.WriteLine("(Pulsa cualquier tecla para continuar)");
                Console.ReadKey(); // Pausamos para que el humano vea lo que ha pasado
                movimientoValido = true;
            }
        }
    }

    // 5. APLICAR EL MOVIMIENTO (Para cualquiera de los dos)
    tablero[filaElegida, columnaElegida] = turno;
    turnosJugados++; 

    // 6. COMPROBAR CONDICIONES DE VICTORIA
    for (int i = 0; i < 3; i++)
    {
        if (tablero[i, 0] == turno && tablero[i, 1] == turno && tablero[i, 2] == turno) { hayGanador = true; }
        if (tablero[0, i] == turno && tablero[1, i] == turno && tablero[2, i] == turno) { hayGanador = true; }
    }
    if (tablero[0, 0] == turno && tablero[1, 1] == turno && tablero[2, 2] == turno) { hayGanador = true; }
    if (tablero[0, 2] == turno && tablero[1, 1] == turno && tablero[2, 0] == turno) { hayGanador = true; }

    // 7. CAMBIO DE TURNO
    if (!hayGanador && turnosJugados < TURNOS_MAXIMOS) // Usamos la constante aquí también
    {
        turno = (turno == 'X') ? 'O' : 'X';
    }
}

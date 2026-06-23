// ====================================================================
// JUEGO: 3 EN RAYA (Humano vs Máquina) 
// VERSIÓN SIMPLE, SIN VALIDACIÓN DE ENTRADAS
// El ordenador elige al azar una casilla vacía para colocar su ficha.
// Conceptos aplicados: Arrays 2D, Bucles (while, for), Condicionales, 
// Constantes, int.Parse() y generación de números aleatorios (Random).
// ====================================================================

bool jugando = true;
bool hayGanador = false;
char turno = 'X';      
int turnosJugados = 0; 
const int TURNOS_MAXIMOS = 9;
Random azar = new Random();

char[,] tablero = {
    { '-', '-', '-' },
    { '-', '-', '-' },
    { '-', '-', '-' } 
};

while (jugando)
{
    Console.Clear();
    Console.WriteLine("=== 3 EN RAYA: VERSIÓN SIMPLIFICADA ===\n");
    
    // 1. DIBUJAR EL TABLERO
    for (int fila = 0; fila < 3; fila++)
    {
        for (int columna = 0; columna < 3; columna++)
        {
            Console.Write(tablero[fila, columna] + " ");
        }
        Console.WriteLine(); 
    }
    Console.WriteLine(); 

    // 2. EVALUAR SI ALGUIEN GANÓ EN EL TURNO ANTERIOR
    if (hayGanador)
    {
        Console.WriteLine($"¡Fin del juego! Ha ganado: {turno}");
        break; 
    }
    else if (turnosJugados == TURNOS_MAXIMOS)
    {
        Console.WriteLine("¡Es un EMPATE!");
        break; 
    }

    // 3. ELEGIR CASILLA (Bucle hasta elegir una vacía)
    int filaElegida = -1;
    int columnaElegida = -1;
    bool valido = false;

    while (!valido)
    {
        if (turno == 'X')
        {
            // TURNO HUMANO: Usamos Parse directo (¡Peligro si escribes letras!)
            Console.WriteLine("Tu turno (X).");
            Console.Write("Fila (0, 1 o 2): ");
            filaElegida = int.Parse(Console.ReadLine()!);
            
            Console.Write("Columna (0, 1 o 2): ");
            columnaElegida = int.Parse(Console.ReadLine()!);
        }
        else
        {
            // TURNO MÁQUINA: Elige al azar
            Console.WriteLine("La Máquina (O) está pensando...");
            filaElegida = azar.Next(0, 3);
            columnaElegida = azar.Next(0, 3);
        }

        // LÓGICA COMPARTIDA: Ambos comprueban la casilla de la misma forma
        if (tablero[filaElegida, columnaElegida] == '-')
        {
            valido = true; // Casilla libre, salimos del bucle
            
            if (turno == 'O') 
            {
                Console.WriteLine("Pulsa Intro para ver su jugada...");
                Console.ReadLine(); // Pausa solo para la máquina
            }
        }
        else if (turno == 'X')
        {
            // Solo avisamos al humano del error, la máquina reintenta en silencio
            Console.WriteLine("¡Error! Casilla ocupada, prueba de nuevo.\n");
        }
    }

    // 4. APLICAR EL MOVIMIENTO
    tablero[filaElegida, columnaElegida] = turno;
    turnosJugados++; 

    // 5. COMPROBAR VICTORIA
    for (int i = 0; i < 3; i++)
    {
        if (tablero[i, 0] == turno && tablero[i, 1] == turno && tablero[i, 2] == turno) { hayGanador = true; }
        if (tablero[0, i] == turno && tablero[1, i] == turno && tablero[2, i] == turno) { hayGanador = true; }
    }
    if (tablero[0, 0] == turno && tablero[1, 1] == turno && tablero[2, 2] == turno) { hayGanador = true; }
    if (tablero[0, 2] == turno && tablero[1, 1] == turno && tablero[2, 0] == turno) { hayGanador = true; }

    // 6. CAMBIAR TURNO
    if (!hayGanador && turnosJugados < TURNOS_MAXIMOS)
    {
        turno = (turno == 'X') ? 'O' : 'X';
    }
}

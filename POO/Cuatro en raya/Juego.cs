class Juego
{
    private readonly Tablero tablero;
    private readonly Jugador jugadorHumano;
    private readonly Jugador jugadorOrdenador;

    public Juego()
    {
        tablero = new Tablero();
        jugadorHumano = new JugadorHumano("Jugador", 'X');
        jugadorOrdenador = new JugadorOrdenador("Ordenador", 'O');
    }

    public void Iniciar()
    {
        Console.WriteLine("Bienvenido al juego de Cuatro en Raya!");
        
        tablero.InicializarTablero();

        Jugador jugadorActual = jugadorHumano;

        bool partidaTerminada = false;
        
        while (!partidaTerminada)
        {
            tablero.Mostrar();

            int columna = jugadorActual.ElegirColumna(tablero);

            tablero.ColocarFicha(columna, jugadorActual.Ficha);

            if (tablero.HayGanador(jugadorActual.Ficha))
            {
                Console.WriteLine($"¡Ha ganado {jugadorActual.Nombre}!");
                tablero.Mostrar();
                partidaTerminada = true;
            }
            else if (tablero.EstaLleno())
            {
                Console.WriteLine("¡Empate! El tablero está lleno.");
                tablero.Mostrar();
                partidaTerminada = true;
            }
            else
            {
                jugadorActual = (jugadorActual == jugadorHumano) ? jugadorOrdenador : jugadorHumano;
            }
        }
    }
}
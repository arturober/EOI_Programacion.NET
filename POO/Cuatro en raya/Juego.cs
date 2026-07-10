// Juego coordina una partida completa.
//
// Esta clase no conoce los detalles internos del array del tablero ni cómo decide
// cada jugador su movimiento. Su responsabilidad es organizar los turnos, pedir
// una columna, colocar la ficha y comprobar si la partida ha terminado.
class Juego
{
    // Los tres objetos se crean una sola vez en el constructor y no se sustituyen
    // durante la partida. Por eso los campos se declaran readonly.
    private readonly Tablero tablero;
    private readonly Jugador jugadorHumano;
    private readonly Jugador jugadorOrdenador;

    public Juego()
    {
        tablero = new Tablero();

        // Las variables utilizan el tipo base Jugador. Gracias al polimorfismo,
        // Juego puede pedir ElegirColumna sin preocuparse de si decide una persona
        // o el ordenador.
        jugadorHumano = new JugadorHumano("Jugador", 'X');
        jugadorOrdenador = new JugadorOrdenador("Ordenador", 'O');
    }

    public void Iniciar()
    {
        // Reiniciar permite que el mismo objeto Juego pueda iniciar otra partida
        // correctamente en el futuro, aunque actualmente se juegue una sola.
        tablero.Reiniciar();

        // El jugador humano comienza la partida.
        Jugador jugadorActual = jugadorHumano;
        bool partidaTerminada = false;

        while (!partidaTerminada)
        {
            // Solo Juego limpia la consola. De esta forma, Tablero se limita a
            // dibujar el tablero y no decide cuándo debe borrarse la pantalla.
            MostrarEstadoDelJuego();

            Console.WriteLine();
            Console.WriteLine(
                $"Turno de {jugadorActual.Nombre} " +
                $"({tablero.ObtenerEmojiDeFicha(jugadorActual.Ficha)})");

            // Cada tipo de jugador implementa su propia forma de elegir columna.
            // El valor devuelto siempre utiliza índices internos de 0 a Columnas - 1.
            int columna = jugadorActual.ElegirColumna(tablero);

            // Tanto el jugador humano como el ordenador comprueban previamente
            // que la columna es válida. El resultado booleano no necesita usarse
            // aquí porque una columna inválida nunca debería llegar a este punto.
            tablero.ColocarFicha(columna, jugadorActual.Ficha);

            // La victoria debe comprobarse antes que el empate. La última ficha
            // podría llenar el tablero y, al mismo tiempo, completar cuatro fichas.
            if (tablero.HayGanador(jugadorActual.Ficha))
            {
                MostrarResultado($"¡Ha ganado {jugadorActual.Nombre}!");
                partidaTerminada = true;
            }
            else if (tablero.EstaLleno())
            {
                MostrarResultado("Empate. El tablero está lleno.");
                partidaTerminada = true;
            }
            else
            {
                // Solo se cambia de turno cuando la partida todavía continúa.
                jugadorActual = CambiarTurno(jugadorActual);
            }
        }
    }

    private void MostrarEstadoDelJuego()
    {
        Console.Clear();

        Console.WriteLine("=== 4 EN RAYA ===");
        Console.WriteLine(
            $"Tú juegas con {tablero.ObtenerEmojiDeFicha(jugadorHumano.Ficha)}.");
        Console.WriteLine(
            $"El ordenador juega con " +
            $"{tablero.ObtenerEmojiDeFicha(jugadorOrdenador.Ficha)}.");
        Console.WriteLine();

        tablero.Mostrar();
    }

    private void MostrarResultado(string mensaje)
    {
        // Se vuelve a dibujar el estado para que la última ficha colocada pueda
        // verse antes de mostrar el mensaje de victoria o empate.
        MostrarEstadoDelJuego();

        Console.WriteLine();
        Console.WriteLine(mensaje);
    }

    private Jugador CambiarTurno(Jugador jugadorActual)
    {
        // Se comparan las referencias porque ambos jugadores se crean una sola vez
        // y se conservan durante toda la partida.
        if (jugadorActual == jugadorHumano)
        {
            return jugadorOrdenador;
        }

        return jugadorHumano;
    }
}
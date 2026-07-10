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
        tablero.Mostrar();
    }
}
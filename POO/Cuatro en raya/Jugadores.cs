// Jugador reúne los datos y el comportamiento común de cualquier participante.
//
// Es abstracta porque no tendría sentido crear un "Jugador" genérico: cada tipo
// concreto debe indicar cómo elige una columna.
abstract class Jugador
{
    // Las propiedades solo tienen get porque el nombre y la ficha no deben cambiar
    // después de construir el jugador.
    public string Nombre { get; }
    public char Ficha { get; }

    protected Jugador(string nombre, char ficha)
    {
        Nombre = nombre;
        Ficha = ficha;
    }

    // El método es abstracto porque la decisión depende del tipo de jugador.
    // Todos devuelven el índice interno de la columna: de 0 a Columnas - 1.
    public abstract int ElegirColumna(Tablero tablero);
}

// Representa al usuario que introduce sus movimientos mediante el teclado.
class JugadorHumano : Jugador
{
    public JugadorHumano(string nombre, char ficha)
        : base(nombre, ficha)
    {
    }

    public override int ElegirColumna(Tablero tablero)
    {
        // El bucle continúa hasta obtener una entrada numérica, dentro del rango
        // permitido y correspondiente a una columna que todavía tenga espacio.
        while (true)
        {
            Console.Write($"Elige una columna del 1 al {Tablero.Columnas}: ");

            // ReadLine puede devolver null en determinadas ejecuciones redirigidas.
            // Convertir ese resultado en una cadena vacía permite que TryParse lo
            // trate simplemente como una entrada no numérica.
            string texto = Console.ReadLine() ?? string.Empty;

            if (!int.TryParse(texto, out int columnaElegida))
            {
                Console.WriteLine("Debes escribir un número entero.");
                continue;
            }

            // El jugador utiliza números del 1 al 7 porque son más naturales.
            // Antes de convertirlos a índices internos, validamos ese intervalo.
            if (columnaElegida < 1 || columnaElegida > Tablero.Columnas)
            {
                Console.WriteLine(
                    $"La columna debe estar entre 1 y {Tablero.Columnas}.");
                continue;
            }

            // Los arrays de C# empiezan en 0, por lo que la columna visible 1 se
            // corresponde con el índice 0, la visible 2 con el índice 1, etc.
            int columna = columnaElegida - 1;

            if (!tablero.ColumnaEsValida(columna))
            {
                Console.WriteLine("Esa columna está llena. Elige otra.");
                continue;
            }

            return columna;
        }
    }
}

// Representa al rival controlado por el programa.
//
// Su estrategia es deliberadamente sencilla y fácil de estudiar:
// 1. Si puede ganar en este turno, realiza ese movimiento.
// 2. Si el rival puede ganar en el siguiente, bloquea esa columna.
// 3. Si el centro está libre, lo ocupa porque suele ser una posición ventajosa.
// 4. En cualquier otro caso, elige aleatoriamente una columna disponible.
class JugadorOrdenador : Jugador
{
    // Se reutiliza el mismo generador durante toda la partida. Crear un Random en
    // cada turno puede producir secuencias repetidas cuando las llamadas se hacen
    // con muy poca diferencia de tiempo.
    private readonly Random random;

    public JugadorOrdenador(string nombre, char ficha)
        : base(nombre, ficha)
    {
        random = new Random();
    }

    public override int ElegirColumna(Tablero tablero)
    {
        Console.WriteLine("El ordenador está pensando...");

        // Primero se busca una jugada que permita ganar inmediatamente.
        int? columnaGanadora = BuscarColumnaGanadora(tablero, Ficha);

        if (columnaGanadora.HasValue)
        {
            return columnaGanadora.Value;
        }

        // Si el ordenador no puede ganar, simula las fichas del rival para saber
        // si necesita bloquear una victoria inmediata.
        char fichaRival = ObtenerFichaRival();
        int? columnaParaBloquear = BuscarColumnaGanadora(tablero, fichaRival);

        if (columnaParaBloquear.HasValue)
        {
            return columnaParaBloquear.Value;
        }

        // En un tablero de siete columnas, la división entera 7 / 2 produce 3,
        // que es el índice interno de la columna central visible como columna 4.
        // Calcularlo evita dejar escrito directamente un valor rígido como 3.
        int columnaCentral = Tablero.Columnas / 2;

        if (tablero.ColumnaEsValida(columnaCentral))
        {
            return columnaCentral;
        }

        return ElegirColumnaAleatoria(tablero);
    }

    private int? BuscarColumnaGanadora(Tablero tablero, char ficha)
    {
        // Se prueba temporalmente cada columna disponible. Después de comprobar
        // el resultado, la ficha simulada se elimina para dejar el tablero tal y
        // como estaba antes de la prueba.
        for (int columna = 0; columna < Tablero.Columnas; columna++)
        {
            if (!tablero.ColumnaEsValida(columna))
            {
                continue;
            }

            tablero.ColocarFicha(columna, ficha);
            bool ganaConEstaColumna = tablero.HayGanador(ficha);
            tablero.DeshacerUltimaFicha(columna);

            if (ganaConEstaColumna)
            {
                return columna;
            }
        }

        // null expresa claramente que ninguna columna produce una victoria.
        return null;
    }

    private int ElegirColumnaAleatoria(Tablero tablero)
    {
        List<int> columnasDisponibles = new List<int>();

        // Solo se añaden columnas que realmente pueden recibir una ficha. Así,
        // cualquier elemento elegido posteriormente será siempre un movimiento
        // legal.
        for (int columna = 0; columna < Tablero.Columnas; columna++)
        {
            if (tablero.ColumnaEsValida(columna))
            {
                columnasDisponibles.Add(columna);
            }
        }

        // Este método solo se ejecuta durante una partida no terminada, por lo que
        // debe existir al menos una columna disponible. Random.Next devuelve una
        // posición entre 0 y Count - 1.
        int posicionAleatoria = random.Next(columnasDisponibles.Count);
        return columnasDisponibles[posicionAleatoria];
    }

    private char ObtenerFichaRival()
    {
        // El juego utiliza únicamente las fichas X y O.
        if (Ficha == 'X')
        {
            return 'O';
        }

        return 'X';
    }
}
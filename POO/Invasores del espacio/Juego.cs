// ============================================================================
// MOTOR PRINCIPAL DEL JUEGO
// ============================================================================
//
// Esta clase contiene todas las reglas de Space Invaders:
// - Crea las distintas oleadas de alienígenas.
// - Procesa las acciones introducidas por el jugador.
// - Actualiza la nave, las balas y los enemigos.
// - Detecta los impactos y la derrota.
// - Mantiene la puntuación y el récord.
// - Dibuja el tablero utilizando distintos colores.
//
// En esta versión no existe un último nivel ni una victoria definitiva.
// Cada vez que el jugador destruye todos los alienígenas se crea un nuevo nivel
// y la dificultad aumenta. La partida solamente termina cuando ocurre una de
// estas dos situaciones:
//
// 1. Un alienígena alcanza la fila de la nave.
// 2. El jugador pulsa la tecla X para salir.
//
// La dificultad aumenta de tres maneras sencillas:
//
// 1. Cada nivel incorpora un alienígena adicional, hasta ocupar las doce
//    columnas del tablero.
// 2. Los alienígenas se mueven con mayor frecuencia al avanzar de nivel.
// 3. En niveles muy altos avanzan varias filas cada vez que les toca moverse.
//
// Estas reglas permiten crear una partida potencialmente infinita sin añadir
// clases nuevas ni complicar excesivamente la estructura del programa.
// ============================================================================
class Juego
{
    // ------------------------------------------------------------------------
    // CONFIGURACIÓN GENERAL DEL TABLERO
    // ------------------------------------------------------------------------

    // Ancho representa el número de columnas disponibles.
    private const int Ancho = 12;

    // Alto representa el número de filas del tablero.
    private const int Alto = 12;

    // La nave comienza en la última fila, por lo que los alienígenas disponen
    // inicialmente de once filas para acercarse al jugador.

    // ------------------------------------------------------------------------
    // CONFIGURACIÓN DE LA DIFICULTAD
    // ------------------------------------------------------------------------

    // El primer nivel comienza con tres enemigos.
    private const int NumeroInicialAliens = 3;

    // En los primeros niveles los alienígenas se mueven cada tres turnos.
    // Después se reducirá este intervalo hasta alcanzar un mínimo de un turno.
    private const int TurnosInicialesPorMovimientoAlien = 3;

    // Cada ocho niveles los alienígenas avanzarán una fila adicional cuando
    // llegue su turno de movimiento:
    //
    // - Niveles 1 a 8:   avanzan 1 fila.
    // - Niveles 9 a 16:  avanzan 2 filas.
    // - Niveles 17 a 24: avanzan 3 filas.
    //
    // El valor puede continuar creciendo porque la partida no tiene un último
    // nivel. En la práctica, la dificultad terminará derrotando al jugador.
    private const int NivelesPorFilaAdicional = 8;

    // Cada alien destruido concede cien puntos.
    private const int PuntosPorAlien = 100;

    // Este carácter representa una posición vacía del tablero.
    private const char CasillaVacia = '.';

    // ------------------------------------------------------------------------
    // OBJETOS Y COLECCIONES
    // ------------------------------------------------------------------------

    // Random se crea una sola vez y se reutiliza durante toda la partida.
    // Se utiliza para elegir las columnas iniciales de los alienígenas.
    private readonly Random _aleatorio = new Random();

    // Los alienígenas se guardan en una lista porque su cantidad puede cambiar
    // entre niveles y porque se eliminan cuando reciben un disparo.
    private readonly List<Alien> _aliens = new List<Alien>();

    // Las balas también se guardan en una lista porque pueden aparecer y
    // desaparecer continuamente durante la partida.
    private readonly List<Bala> _balas = new List<Bala>();

    // Solo existe una nave durante toda la partida.
    private readonly Nave _nave;

    // ------------------------------------------------------------------------
    // ESTADO ACTUAL DE LA PARTIDA
    // ------------------------------------------------------------------------

    // El nivel comienza en uno y aumenta sin un límite máximo establecido.
    private int _nivelActual = 1;

    // Contiene todos los puntos obtenidos durante la partida actual.
    private int _puntuacion;

    // Contiene la mejor puntuación cargada desde el fichero record.txt.
    private int _record;

    // Cuenta los turnos válidos consumidos dentro del nivel actual.
    // Se utiliza para decidir cuándo deben avanzar los alienígenas.
    private int _numeroTurno;

    // Indica si el récord ha cambiado durante la partida y, por tanto, debe
    // escribirse en el fichero antes de terminar.
    private bool _recordModificado;

    public Juego()
    {
        // La nave aparece aproximadamente en el centro de la última fila.
        _nave = new Nave(Ancho / 2, Alto - 1);

        // El récord se conserva entre ejecuciones gracias a record.txt.
        _record = GestorRecord.Cargar();

        // Se crea la primera oleada de enemigos.
        CrearAliensDelNivelActual();
    }

    // Ejecuta la partida hasta que el jugador pierde o decide salir.
    //
    // Ya no se devuelve ResultadoPartida.Victoria porque no existe un último
    // nivel. Superar una oleada siempre conduce a la siguiente.
    public ResultadoPartida Ejecutar()
    {
        // El primer mensaje informa del objetivo y de la dificultad inicial.
        string mensajeTurno = CrearMensajeInicioNivel();

        // El bucle representa la partida completa. No tiene un número fijo de
        // repeticiones porque el jugador puede superar cualquier cantidad de
        // niveles mientras consiga mantenerse con vida.
        while (true)
        {
            DibujarJuego(mensajeTurno);

            // LeerAccion solo devuelve controles válidos. Una tecla incorrecta
            // se vuelve a solicitar y no consume un turno del juego.
            AccionJugador accion = EntradaConsola.LeerAccion();

            if (accion == AccionJugador.Salir)
            {
                // Aunque el jugador abandone voluntariamente, se guarda el récord
                // si lo había superado durante la partida.
                GuardarRecordSiEsNecesario();

                return ResultadoPartida.Salida;
            }

            // Se actualizan todas las entidades y se obtiene un posible mensaje
            // relacionado con los impactos del turno.
            mensajeTurno = ProcesarTurno(accion);

            // Si no quedan enemigos, el nivel actual se ha completado.
            //
            // En lugar de terminar con una victoria, se incrementa el nivel, se
            // limpia el escenario y se crea una oleada más difícil.
            if (_aliens.Count == 0)
            {
                int nivelCompletado = _nivelActual;

                PrepararSiguienteNivel();

                mensajeTurno =
                    $"¡NIVEL {nivelCompletado} COMPLETADO! "
                    + CrearMensajeInicioNivel();
            }

            // La invasión se comprueba después de resolver los impactos.
            // Así, una bala todavía puede destruir a un alien que acaba de
            // avanzar antes de declarar la derrota.
            if (HayInvasion())
            {
                DibujarJuego(mensajeTurno);

                string mensajeRecord = GuardarRecordSiEsNecesario();

                MostrarResultado(
                    $"¡GAME OVER! Has alcanzado el nivel {_nivelActual}.",
                    mensajeRecord);

                return ResultadoPartida.Derrota;
            }
        }
    }

    // Procesa un turno completo después de recibir una acción válida.
    private string ProcesarTurno(AccionJugador accion)
    {
        // Cada llamada representa un turno real consumido por el jugador.
        _numeroTurno++;

        // Disparar crea una bala antes de actualizar las entidades.
        if (accion == AccionJugador.Disparar)
        {
            Disparar();
        }

        // La nave se mueve únicamente si la acción es Izquierda o Derecha.
        // Ante Disparar permanece en la misma columna.
        _nave.Actualizar(accion, Ancho);

        // Las balas avanzan en todos los turnos, independientemente de la acción
        // que haya realizado el jugador.
        ActualizarBalas(accion);

        // Primero se comprueba si alguna bala ha entrado en la casilla que ya
        // ocupaba un alienígena.
        int aliensDestruidos = ProcesarImpactos();

        // Las balas que han salido por la parte superior dejan de ser necesarias.
        EliminarBalasFueraDelTablero();

        // Los alienígenas no se mueven necesariamente en todos los turnos.
        // La frecuencia depende del nivel actual.
        if (DebenMoverseLosAliens())
        {
            // En niveles altos pueden avanzar más de una fila cada vez.
            int filasPorMovimiento = ObtenerFilasPorMovimientoAlien();

            // El movimiento se realiza fila a fila, en vez de modificar Y de
            // golpe. Esto permite comprobar impactos después de cada paso y evita
            // que un alien atraviese una bala sin ser detectado.
            for (int paso = 0; paso < filasPorMovimiento; paso++)
            {
                ActualizarAliens(accion);

                // Se vuelven a comprobar los impactos por si un alien acaba de
                // entrar en la casilla ocupada por una bala.
                aliensDestruidos += ProcesarImpactos();

                // Si todos los alienígenas han sido destruidos, no tiene sentido
                // continuar realizando los pasos restantes de movimiento.
                if (_aliens.Count == 0)
                {
                    break;
                }

                // Si un alien ya ha alcanzado la nave, tampoco es necesario que
                // el resto de pasos del mismo turno continúen ejecutándose.
                if (HayInvasion())
                {
                    break;
                }
            }
        }

        // Los puntos se añaden después de reunir todos los impactos producidos
        // durante el turno.
        if (aliensDestruidos > 0)
        {
            RegistrarPuntos(aliensDestruidos);
        }

        return CrearMensajeImpactos(aliensDestruidos);
    }

    // Crea una nueva bala en la posición de la nave.
    private void Disparar()
    {
        // La bala aparece inicialmente en la misma casilla que la nave.
        // En este mismo turno se actualizará y subirá una fila.
        Bala nuevaBala = new Bala(_nave.X, _nave.Y);
        _balas.Add(nuevaBala);
    }

    // Actualiza todas las balas activas.
    private void ActualizarBalas(AccionJugador accion)
    {
        foreach (Bala bala in _balas)
        {
            // La bala no utiliza realmente la acción, pero se pasa el valor para
            // cumplir el contrato común definido por Entidad.Actualizar.
            bala.Actualizar(accion, Ancho);
        }
    }

    // Desplaza todos los alienígenas exactamente una fila hacia abajo.
    private void ActualizarAliens(AccionJugador accion)
    {
        foreach (Alien alien in _aliens)
        {
            // Cada llamada a Actualizar aumenta en uno la coordenada Y del alien.
            alien.Actualizar(accion, Ancho);
        }
    }

    // Indica si ha llegado el momento de mover los enemigos.
    private bool DebenMoverseLosAliens()
    {
        int turnosPorMovimiento = ObtenerTurnosPorMovimientoAlien();

        // El resto será cero cuando _numeroTurno sea múltiplo del intervalo.
        return _numeroTurno % turnosPorMovimiento == 0;
    }

    // Calcula cada cuántos turnos deben moverse los alienígenas.
    //
    // La frecuencia aumenta de forma gradual:
    //
    // - Niveles 1 y 2: se mueven cada 3 turnos.
    // - Niveles 3 y 4: se mueven cada 2 turnos.
    // - Nivel 5 en adelante: se mueven en todos los turnos.
    //
    // Math.Max impide que el resultado llegue a cero o sea negativo.
    private int ObtenerTurnosPorMovimientoAlien()
    {
        int reduccion = (_nivelActual - 1) / 2;

        return Math.Max(
            TurnosInicialesPorMovimientoAlien - reduccion,
            1);
    }

    // Calcula cuántas filas avanzan los alienígenas cuando les toca moverse.
    //
    // La división entera hace que el aumento se produzca por bloques:
    //
    // - Nivel 1:  (1 - 1) / 8 = 0; avanzan 1 fila.
    // - Nivel 9:  (9 - 1) / 8 = 1; avanzan 2 filas.
    // - Nivel 17: (17 - 1) / 8 = 2; avanzan 3 filas.
    //
    // De esta forma la dificultad puede seguir aumentando incluso cuando ya se
    // haya alcanzado el máximo de doce alienígenas en pantalla.
    private int ObtenerFilasPorMovimientoAlien()
    {
        return 1 + (_nivelActual - 1) / NivelesPorFilaAdicional;
    }

    // Comprueba todos los posibles impactos entre balas y alienígenas.
    private int ProcesarImpactos()
    {
        int aliensDestruidos = 0;

        // Las listas se recorren desde el final porque se eliminan elementos
        // durante el recorrido. De este modo, los índices pendientes no cambian.
        for (int indiceAlien = _aliens.Count - 1;
            indiceAlien >= 0;
            indiceAlien--)
        {
            Alien alien = _aliens[indiceAlien];

            for (int indiceBala = _balas.Count - 1;
                indiceBala >= 0;
                indiceBala--)
            {
                Bala bala = _balas[indiceBala];

                if (HayImpacto(bala, alien))
                {
                    // Tanto la bala como el alien desaparecen tras el impacto.
                    _balas.RemoveAt(indiceBala);
                    _aliens.RemoveAt(indiceAlien);

                    aliensDestruidos++;

                    // El alien ya ha sido eliminado, por lo que no se compara con
                    // ninguna otra bala.
                    break;
                }
            }
        }

        return aliensDestruidos;
    }

    // Dos entidades colisionan cuando comparten columna y fila.
    private static bool HayImpacto(Bala bala, Alien alien)
    {
        return bala.X == alien.X && bala.Y == alien.Y;
    }

    // Elimina las balas que han salido por la parte superior del tablero.
    private void EliminarBalasFueraDelTablero()
    {
        for (int indice = _balas.Count - 1; indice >= 0; indice--)
        {
            if (_balas[indice].Y < 0)
            {
                _balas.RemoveAt(indice);
            }
        }
    }

    // Añade los puntos correspondientes y actualiza el récord provisional.
    private void RegistrarPuntos(int aliensDestruidos)
    {
        _puntuacion += aliensDestruidos * PuntosPorAlien;

        // El valor mostrado en pantalla se actualiza inmediatamente, aunque el
        // fichero no se escribe hasta que la partida termina o el jugador sale.
        if (_puntuacion > _record)
        {
            _record = _puntuacion;
            _recordModificado = true;
        }
    }

    // Crea el mensaje mostrado después de destruir uno o varios enemigos.
    private static string CrearMensajeImpactos(int aliensDestruidos)
    {
        if (aliensDestruidos == 0)
        {
            return string.Empty;
        }

        if (aliensDestruidos == 1)
        {
            return $"¡ALIEN DESTRUIDO! +{PuntosPorAlien} puntos.";
        }

        int puntosGanados = aliensDestruidos * PuntosPorAlien;

        return $"¡{aliensDestruidos} ALIENS DESTRUIDOS! "
            + $"+{puntosGanados} puntos.";
    }

    // Comprueba si algún alienígena ha alcanzado la fila de la nave.
    private bool HayInvasion()
    {
        foreach (Alien alien in _aliens)
        {
            if (alien.Y >= _nave.Y)
            {
                return true;
            }
        }

        return false;
    }

    // Limpia el nivel anterior y prepara la siguiente oleada.
    private void PrepararSiguienteNivel()
    {
        // El nivel puede aumentar indefinidamente.
        _nivelActual++;

        // Las balas del nivel anterior desaparecen para que la nueva oleada
        // comience en un estado claro y predecible.
        _balas.Clear();
        _aliens.Clear();

        // El contador vuelve a cero para que el intervalo de movimiento del nuevo
        // nivel empiece a contarse desde el primer turno.
        _numeroTurno = 0;

        CrearAliensDelNivelActual();
    }

    // Crea los alienígenas correspondientes al nivel actual.
    private void CrearAliensDelNivelActual()
    {
        int numeroAliens = ObtenerNumeroAliensDelNivel();

        while (_aliens.Count < numeroAliens)
        {
            int columna = _aleatorio.Next(Ancho);

            // No se colocan dos alienígenas en la misma columna inicial porque
            // visualmente parecería que solo existe uno.
            if (!ExisteAlienEnColumna(columna))
            {
                _aliens.Add(new Alien(columna, 0));
            }
        }
    }

    // Calcula cuántos enemigos debe contener la oleada actual.
    private int ObtenerNumeroAliensDelNivel()
    {
        // Cada nivel añade un alien respecto al anterior:
        //
        // - Nivel 1: 3 alienígenas.
        // - Nivel 2: 4 alienígenas.
        // - Nivel 3: 5 alienígenas.
        //
        // Math.Min limita la cantidad a Ancho. Como cada enemigo comienza en una
        // columna distinta, no pueden crearse más de doce en la primera fila.
        return Math.Min(
            NumeroInicialAliens + _nivelActual - 1,
            Ancho);
    }

    // Indica si una columna ya contiene un alienígena inicial.
    private bool ExisteAlienEnColumna(int columna)
    {
        foreach (Alien alien in _aliens)
        {
            if (alien.X == columna)
            {
                return true;
            }
        }

        return false;
    }

    // Crea el texto que describe la oleada que está a punto de comenzar.
    private string CrearMensajeInicioNivel()
    {
        int turnosPorMovimiento = ObtenerTurnosPorMovimientoAlien();
        int filasPorMovimiento = ObtenerFilasPorMovimientoAlien();

        return $"Comienza el nivel {_nivelActual}: {_aliens.Count} alienígenas, "
            + $"movimiento cada {turnosPorMovimiento} turno(s) y "
            + $"avance de {filasPorMovimiento} fila(s).";
    }

    // Guarda el récord únicamente cuando ha sido modificado.
    private string GuardarRecordSiEsNecesario()
    {
        if (!_recordModificado)
        {
            return string.Empty;
        }

        bool guardadoCorrectamente = GestorRecord.Guardar(_record);

        if (guardadoCorrectamente)
        {
            // Se vuelve a establecer false para evitar escrituras duplicadas.
            _recordModificado = false;
            return $"¡Nuevo récord guardado: {_record} puntos!";
        }

        return "Has conseguido un nuevo récord, pero no se pudo guardar "
            + "en el archivo record.txt.";
    }

    // Construye y muestra el estado visual del juego.
    private void DibujarJuego(string mensaje)
    {
        char[,] tablero = CrearTableroVacio();

        // Los alienígenas y las balas se colocan primero.
        foreach (Alien alien in _aliens)
        {
            DibujarEntidad(tablero, alien);
        }

        foreach (Bala bala in _balas)
        {
            DibujarEntidad(tablero, bala);
        }

        // La nave se dibuja al final para asegurar que sea visible en su fila.
        DibujarEntidad(tablero, _nave);

        Console.Clear();
        Console.WriteLine("=================================");
        Console.WriteLine("         SPACE INVADERS          ");
        Console.WriteLine("=================================");
        Console.WriteLine($"Nivel: {_nivelActual}");
        Console.WriteLine($"Puntuación: {_puntuacion}");
        Console.WriteLine($"Récord: {_record}");
        Console.WriteLine($"Alienígenas restantes: {_aliens.Count}");
        Console.WriteLine(
            $"Movimiento enemigo: cada {ObtenerTurnosPorMovimientoAlien()} "
            + $"turno(s), {ObtenerFilasPorMovimientoAlien()} fila(s)");
        Console.WriteLine();

        string bordeHorizontal =
            "+" + new string('-', Ancho * 2 + 1) + "+";

        Console.WriteLine(bordeHorizontal);

        for (int y = 0; y < Alto; y++)
        {
            Console.Write("| ");

            for (int x = 0; x < Ancho; x++)
            {
                EscribirCasillaConColor(tablero[y, x]);
                Console.Write(' ');
            }

            Console.WriteLine("|");
        }

        Console.WriteLine(bordeHorizontal);

        if (!string.IsNullOrWhiteSpace(mensaje))
        {
            Console.WriteLine();
            Console.WriteLine(mensaje);
        }
    }

    // Crea una matriz llena de casillas vacías.
    private static char[,] CrearTableroVacio()
    {
        char[,] tablero = new char[Alto, Ancho];

        for (int y = 0; y < Alto; y++)
        {
            for (int x = 0; x < Ancho; x++)
            {
                tablero[y, x] = CasillaVacia;
            }
        }

        return tablero;
    }

    // Coloca una entidad en la matriz cuando sus coordenadas son válidas.
    private static void DibujarEntidad(char[,] tablero, Entidad entidad)
    {
        if (entidad.EstaDentroDelTablero(Ancho, Alto))
        {
            tablero[entidad.Y, entidad.X] = entidad.Icono;
        }
    }

    // Escribe cada carácter con un color diferente según el tipo de entidad.
    private static void EscribirCasillaConColor(char casilla)
    {
        // Se guarda el color actual para restaurarlo después.
        ConsoleColor colorAnterior = Console.ForegroundColor;

        switch (casilla)
        {
            case 'A':
                Console.ForegroundColor = ConsoleColor.Cyan;
                break;

            case 'V':
                Console.ForegroundColor = ConsoleColor.Green;
                break;

            case '|':
                Console.ForegroundColor = ConsoleColor.Yellow;
                break;

            default:
                Console.ForegroundColor = ConsoleColor.DarkGray;
                break;
        }

        Console.Write(casilla);
        Console.ForegroundColor = colorAnterior;
    }

    // Muestra la información final cuando el jugador ha sido derrotado.
    private void MostrarResultado(
        string resultado,
        string mensajeRecord)
    {
        Console.WriteLine();
        Console.WriteLine(resultado);
        Console.WriteLine($"Puntuación final: {_puntuacion}");

        if (!string.IsNullOrWhiteSpace(mensajeRecord))
        {
            Console.WriteLine(mensajeRecord);
        }

        Console.WriteLine();
        Console.WriteLine("Pulsa una tecla para continuar...");
        Console.ReadKey(intercept: true);
    }
}

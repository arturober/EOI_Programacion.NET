using Godot;
using System;
using System.Collections.Generic;

// ============================================================================
// JUEGO: INVASORES DEL ESPACIO
// ============================================================================
//
// Esta clase es el nodo principal de la escena y coordina todo el juego.
// Contiene dos grupos de responsabilidades:
//
// 1. LÓGICA DEL JUEGO
//    - Iniciar una partida.
//    - Recibir las acciones del jugador.
//    - Mover la nave, las balas y los alienígenas.
//    - Detectar impactos y derrotas.
//    - Crear nuevos niveles.
//    - Gestionar la puntuación y el récord.
//
// 2. REPRESENTACIÓN GRÁFICA
//    - Dibujar el fondo y la cuadrícula.
//    - Convertir posiciones lógicas en posiciones de pantalla.
//    - Dibujar la nave, los alienígenas y las balas.
//    - Mostrar información, controles y la pantalla de derrota.
//
// El proyecto no utiliza imágenes, sprites, fuentes externas, sonidos ni escenas
// adicionales. Todo se genera mediante código usando las funciones de dibujo de
// Node2D. La representación es deliberadamente sencilla para que pueda entenderse
// y modificarse manualmente sin conocimientos gráficos avanzados.
//
// El juego conserva un sistema por turnos. Cada pulsación válida del jugador
// consume un turno. Las balas se mueven en todos los turnos y los alienígenas se
// mueven con una frecuencia que depende del nivel.
// ============================================================================

// partial es obligatorio en las clases C# que forman parte del sistema de nodos
// de Godot. Godot genera internamente otra parte de la clase para integrarla con
// el motor.
//
// Node2D es un nodo pensado para elementos bidimensionales. Al heredar de él se
// pueden sobrescribir métodos como _Ready(), _UnhandledInput() y _Draw().
public partial class Juego : Node2D
{
    // ------------------------------------------------------------------------
    // CONFIGURACIÓN LÓGICA DEL JUEGO
    // ------------------------------------------------------------------------

    // Número de columnas del tablero lógico.
    //
    // Las columnas válidas están comprendidas entre 0 y Ancho - 1.
    private const int Ancho = 12;

    // Número de filas del tablero lógico.
    //
    // Las filas válidas están comprendidas entre 0 y Alto - 1. La fila 0 está
    // arriba y la fila Alto - 1 está abajo.
    private const int Alto = 12;

    // Cantidad de enemigos que aparecen en el nivel 1.
    // Cada nivel posterior añade uno más hasta alcanzar el ancho del tablero.
    private const int NumeroInicialAliens = 3;

    // En el primer nivel los alienígenas avanzan cada tres turnos.
    // La frecuencia aumentará gradualmente al subir de nivel.
    private const int TurnosInicialesPorMovimientoAlien = 3;

    // Cada ocho niveles los alienígenas avanzan una fila adicional cuando les
    // corresponde moverse:
    //
    // - Niveles 1 a 8: avanzan 1 fila.
    // - Niveles 9 a 16: avanzan 2 filas.
    // - Niveles 17 a 24: avanzan 3 filas.
    private const int NivelesPorFilaAdicional = 8;

    // Puntos concedidos por cada alienígena destruido.
    private const int PuntosPorAlien = 100;

    // ------------------------------------------------------------------------
    // CONFIGURACIÓN GRÁFICA
    // ------------------------------------------------------------------------

    // Tamaño en píxeles de cada casilla cuadrada.
    //
    // La lógica del juego no depende de este valor. Cambiarlo modifica el tamaño
    // visual del tablero, pero no las posiciones X e Y de las entidades.
    private const float TamanoCasilla = 40.0f;

    // Posición en píxeles de la esquina superior izquierda del tablero.
    //
    // Se declara static readonly porque Vector2 no puede ser const. El valor se
    // crea una vez y no debe sustituirse durante la ejecución.
    private static readonly Vector2 PosicionTablero = new Vector2(280, 145);

    // Los colores de Godot utilizan componentes entre 0 y 1:
    //
    // - 0 representa ausencia del componente.
    // - 1 representa su intensidad máxima.
    //
    // Se emplea una paleta reducida para que el código gráfico sea fácil de leer
    // y modificar. Cada color tiene una finalidad clara.

    // Color utilizado para limpiar y rellenar toda la ventana.
    private static readonly Color ColorFondo = new Color(0.03f, 0.04f, 0.09f);

    // Color de relleno del área de juego.
    private static readonly Color ColorTablero = new Color(0.06f, 0.08f, 0.16f);

    // Color de los bordes, la cuadrícula y los textos secundarios.
    private static readonly Color ColorLineas = new Color(0.20f, 0.35f, 0.50f);

    // Color principal de los textos y de los ojos de los alienígenas.
    private static readonly Color ColorTexto = new Color(1.00f, 1.00f, 1.00f);

    // Color con el que se dibuja la nave y el título.
    private static readonly Color ColorNave = new Color(0.10f, 0.70f, 1.00f);

    // Color con el que se dibujan los enemigos.
    private static readonly Color ColorAlien = new Color(0.30f, 0.90f, 0.30f);

    // Color con el que se dibujan las balas y el mensaje de nuevo récord.
    private static readonly Color ColorBala = new Color(1.00f, 0.00f, 0.00f);

    // Color utilizado para destacar la pantalla de derrota.
    private static readonly Color ColorPeligro = new Color(1.00f, 0.20f, 0.20f);

    // Referencia a la fuente integrada de Godot.
    //
    // null! indica al compilador que el campo comenzará sin valor, pero que el
    // programa se responsabiliza de asignarlo antes de utilizarlo. La asignación
    // se realiza en _Ready(). Se usa la fuente de reserva del motor para evitar
    // depender de un archivo de fuente externo.
    private Font _fuente = null!;

    // ------------------------------------------------------------------------
    // ENTIDADES Y COLECCIONES
    // ------------------------------------------------------------------------

    // Random se crea una sola vez y se reutiliza durante toda la ejecución.
    // Se emplea para seleccionar las columnas iniciales de los alienígenas.
    private readonly Random _aleatorio = new Random();

    // Los alienígenas se guardan en una lista porque su cantidad puede cambiar y
    // porque deben eliminarse cuando reciben un impacto.
    //
    // readonly impide sustituir la lista por otra, pero no impide añadir o quitar
    // elementos de su interior.
    private readonly List<Alien> _aliens = new List<Alien>();

    // Las balas también requieren una lista dinámica porque se crean y destruyen
    // continuamente durante la partida.
    private readonly List<Bala> _balas = new List<Bala>();

    // Solo existe una nave. Se crea al iniciar cada partida.
    private Nave _nave = null!;

    // ------------------------------------------------------------------------
    // ESTADO ACTUAL DE LA PARTIDA
    // ------------------------------------------------------------------------

    // Nivel que se está jugando. Comienza en 1 y aumenta al eliminar todos los
    // alienígenas de una oleada.
    private int _nivelActual;

    // Puntos obtenidos durante la partida actual.
    private int _puntuacion;

    // Mejor puntuación conocida. Se carga desde el fichero y puede actualizarse
    // inmediatamente en memoria al superar el valor anterior.
    private int _record;

    // Número de turnos válidos consumidos dentro del nivel actual.
    // Se utiliza para decidir cuándo deben moverse los enemigos.
    private int _numeroTurno;

    // Indica si el récord se ha superado y todavía debe escribirse en el fichero.
    // Evita guardar el archivo después de cada impacto.
    private bool _recordModificado;

    // Indica si el jugador ha sido derrotado.
    // Mientras sea true, las acciones normales dejan de procesarse y solo se
    // permiten las teclas de reinicio o salida.
    private bool _partidaTerminada;

    // Mensaje informativo mostrado debajo del tablero. Puede indicar el comienzo
    // de un nivel, la destrucción de enemigos o la superación de una oleada.
    private string _mensajeTurno = string.Empty;

    // Texto mostrado dentro de la pantalla de derrota.
    private string _mensajeResultado = string.Empty;

    // Mensaje relacionado con el guardado del récord al finalizar la partida.
    private string _mensajeRecord = string.Empty;

    // ------------------------------------------------------------------------
    // MÉTODOS PRINCIPALES DE GODOT
    // ------------------------------------------------------------------------

    // Godot llama automáticamente a _Ready() una vez cuando el nodo entra en el
    // árbol de la escena y está preparado para comenzar.
    //
    // En una aplicación de consola, esta inicialización se realizaría desde Main.
    // En Godot, _Ready() cumple la función de punto de inicio de esta escena.
    public override void _Ready()
    {
        // ThemeDB.FallbackFont proporciona una fuente integrada. Esto permite
        // dibujar texto sin añadir archivos externos al proyecto.
        _fuente = ThemeDB.FallbackFont;

        // Se crean la nave, el nivel inicial, los mensajes y el estado completo.
        IniciarNuevaPartida();
    }

    // Godot llama a _UnhandledInput() cuando se produce un evento de entrada que
    // no ha sido consumido previamente por otro nodo o elemento de interfaz.
    //
    // A diferencia de Console.ReadKey(), este método no bloquea el programa. El
    // motor continúa funcionando y entrega los eventos cuando se producen.
    public override void _UnhandledInput(InputEvent evento)
    {
        // Cuando la partida ha terminado, las teclas tienen otro significado:
        // R o Intro reinician y X o Escape salen. Por eso se delega el evento a
        // un método específico y no se procesa como un turno normal.
        if (_partidaTerminada)
        {
            ProcesarEntradaPartidaTerminada(evento);
            return;
        }

        // EntradaGodot intenta traducir el evento a una acción lógica.
        // Si devuelve false, la tecla no pertenece a los controles y se ignora.
        if (!EntradaGodot.IntentarLeerAccion(evento, out AccionJugador accion))
        {
            return;
        }

        // Se marca el evento como gestionado para evitar que otros nodos intenten
        // reaccionar también a la misma pulsación.
        GetViewport().SetInputAsHandled();

        // Salir no consume un turno. Antes de cerrar se intenta guardar el récord
        // si ha sido superado durante la partida.
        if (accion == AccionJugador.Salir)
        {
            GuardarRecordSiEsNecesario();
            GetTree().Quit();
            return;
        }

        // Cualquier otra acción válida representa un turno completo.
        ProcesarAccionJugador(accion);

        // QueueRedraw solicita a Godot que vuelva a ejecutar _Draw().
        // Es necesario porque las posiciones, puntos o mensajes pueden haber
        // cambiado y la pantalla debe reflejar el estado nuevo.
        QueueRedraw();
    }

    // ------------------------------------------------------------------------
    // INICIO, REINICIO Y FINAL DE LA PARTIDA
    // ------------------------------------------------------------------------

    // Restablece todos los valores necesarios para comenzar desde el nivel 1.
    //
    // Este mismo método se utiliza tanto al abrir el juego como al pulsar R o
    // Intro después de una derrota. Centralizar la inicialización evita duplicar
    // instrucciones y garantiza que ambos casos produzcan el mismo estado.
    private void IniciarNuevaPartida()
    {
        // Valores básicos de una partida nueva.
        _nivelActual = 1;
        _puntuacion = 0;
        _numeroTurno = 0;

        // El récord se vuelve a leer para recoger el valor persistente guardado.
        _record = GestorRecord.Cargar();

        // Al comenzar todavía no se ha superado el récord y la partida está activa.
        _recordModificado = false;
        _partidaTerminada = false;

        // Se eliminan mensajes que pudieran pertenecer a una derrota anterior.
        _mensajeResultado = string.Empty;
        _mensajeRecord = string.Empty;

        // Las listas se vacían porque podrían contener entidades de la partida
        // anterior. Se reutilizan las mismas listas en lugar de crear otras nuevas.
        _aliens.Clear();
        _balas.Clear();

        // La nave aparece aproximadamente en el centro de la última fila.
        // La división entera Ancho / 2 produce una columna central válida.
        _nave = new Nave(Ancho / 2, Alto - 1);

        // Se genera la primera oleada y se prepara su mensaje informativo.
        CrearAliensDelNivelActual();
        _mensajeTurno = CrearMensajeInicioNivel();

        // Se solicita un dibujo inicial o un redibujado tras reiniciar.
        QueueRedraw();
    }

    // Procesa las únicas acciones permitidas cuando aparece GAME OVER.
    private void ProcesarEntradaPartidaTerminada(InputEvent evento)
    {
        // R o Intro comienzan inmediatamente una partida nueva.
        if (EntradaGodot.EsTeclaReinicio(evento))
        {
            GetViewport().SetInputAsHandled();
            IniciarNuevaPartida();
            return;
        }

        // También se permite salir mediante X o Escape. Se reutiliza el método de
        // traducción normal para no repetir la comprobación de ambas teclas.
        if (EntradaGodot.IntentarLeerAccion(evento, out AccionJugador accion)
            && accion == AccionJugador.Salir)
        {
            GetViewport().SetInputAsHandled();
            GetTree().Quit();
        }
    }

    // Coordina todo lo que debe ocurrir después de una acción válida del jugador.
    //
    // ProcesarTurno se ocupa del movimiento y los impactos. Después se comprueba
    // si el nivel se ha completado o si los enemigos han alcanzado la nave.
    private void ProcesarAccionJugador(AccionJugador accion)
    {
        // ProcesarTurno devuelve el mensaje asociado a los impactos producidos.
        _mensajeTurno = ProcesarTurno(accion);

        // Una lista de enemigos vacía significa que se ha completado la oleada.
        if (_aliens.Count == 0)
        {
            // Se conserva el número antes de incrementarlo para mostrar qué nivel
            // acaba de superar el jugador.
            int nivelCompletado = _nivelActual;

            PrepararSiguienteNivel();

            _mensajeTurno =
                $"¡NIVEL {nivelCompletado} COMPLETADO! "
                + CrearMensajeInicioNivel();
        }

        // La derrota se comprueba después de resolver los impactos y de preparar
        // un posible nivel nuevo. Un alien que haya sido destruido en el mismo
        // turno no debe provocar una derrota.
        if (HayInvasion())
        {
            // El archivo solo se escribe si el récord ha cambiado.
            _mensajeRecord = GuardarRecordSiEsNecesario();

            _mensajeResultado =
                $"Has alcanzado el nivel {_nivelActual}.";

            // Desde este momento la entrada normal queda desactivada y _Draw()
            // mostrará el cuadro de GAME OVER.
            _partidaTerminada = true;
        }
    }

    // ------------------------------------------------------------------------
    // PROCESAMIENTO DE UN TURNO
    // ------------------------------------------------------------------------

    // Ejecuta todas las reglas correspondientes a una acción válida.
    //
    // El orden es importante:
    //
    // 1. Aumentar el contador de turnos.
    // 2. Crear una bala si el jugador dispara.
    // 3. Actualizar la nave.
    // 4. Mover las balas.
    // 5. Resolver impactos.
    // 6. Eliminar balas fuera del tablero.
    // 7. Mover los alienígenas cuando corresponda.
    // 8. Resolver nuevos impactos.
    // 9. Registrar los puntos.
    private string ProcesarTurno(AccionJugador accion)
    {
        // Solo las acciones válidas que llegan hasta aquí consumen un turno.
        _numeroTurno++;

        // La bala se crea antes de actualizar todas las balas. De esta forma nace
        // en la casilla de la nave y sube una fila durante el mismo turno.
        if (accion == AccionJugador.Disparar)
        {
            Disparar();
        }

        // La nave solo se desplazará si la acción es Izquierda o Derecha.
        // Ante Disparar permanecerá en su columna actual.
        _nave.Actualizar(accion, Ancho);

        // Todas las balas activas avanzan una fila en cada turno.
        ActualizarBalas(accion);

        // Se comprueban los impactos producidos por el movimiento de las balas.
        int aliensDestruidos = ProcesarImpactos();

        // Las balas con Y negativa ya han salido por arriba y dejan de ser útiles.
        EliminarBalasFueraDelTablero();

        // Los enemigos no avanzan necesariamente en todos los turnos. La función
        // utiliza el nivel actual para determinar su frecuencia.
        if (DebenMoverseLosAliens())
        {
            // En niveles altos pueden avanzar más de una fila cada vez.
            int filasPorMovimiento = ObtenerFilasPorMovimientoAlien();

            // El avance se realiza paso a paso en lugar de aumentar Y de golpe.
            // Así se comprueban impactos después de cada fila y se evita que un
            // alien atraviese una bala sin colisionar con ella.
            for (int paso = 0; paso < filasPorMovimiento; paso++)
            {
                ActualizarAliens(accion);

                // Un enemigo puede entrar en una casilla ocupada por una bala.
                aliensDestruidos += ProcesarImpactos();

                // No se realizan pasos innecesarios si ya no quedan enemigos o si
                // alguno ha alcanzado la fila de la nave.
                if (_aliens.Count == 0 || HayInvasion())
                {
                    break;
                }
            }
        }

        // Los puntos se añaden una sola vez después de contar todos los impactos
        // producidos durante el turno completo.
        if (aliensDestruidos > 0)
        {
            RegistrarPuntos(aliensDestruidos);
        }

        // El texto se mostrará debajo del tablero en el próximo redibujado.
        return CrearMensajeImpactos(aliensDestruidos);
    }

    // Crea una bala en la posición actual de la nave y la añade a la colección.
    private void Disparar()
    {
        _balas.Add(new Bala(_nave.X, _nave.Y));
    }

    // Actualiza todas las balas activas.
    private void ActualizarBalas(AccionJugador accion)
    {
        foreach (Bala bala in _balas)
        {
            // Bala ignora realmente accion y ancho, pero recibe ambos valores para
            // cumplir el contrato común definido por Entidad.Actualizar().
            bala.Actualizar(accion, Ancho);
        }
    }

    // Desplaza todos los alienígenas exactamente una fila hacia abajo.
    private void ActualizarAliens(AccionJugador accion)
    {
        foreach (Alien alien in _aliens)
        {
            alien.Actualizar(accion, Ancho);
        }
    }

    // Determina si el turno actual es uno de los turnos en los que deben avanzar
    // los enemigos.
    private bool DebenMoverseLosAliens()
    {
        int turnosPorMovimiento = ObtenerTurnosPorMovimientoAlien();

        // El resto de una división es cero cuando _numeroTurno es múltiplo del
        // intervalo. Por ejemplo, con un intervalo 3 se moverán en 3, 6, 9, etc.
        return _numeroTurno % turnosPorMovimiento == 0;
    }

    // Calcula cada cuántos turnos se mueven los alienígenas.
    //
    // La reducción aumenta en una unidad cada dos niveles debido a la división
    // entera:
    //
    // - Niveles 1 y 2: reducción 0, se mueven cada 3 turnos.
    // - Niveles 3 y 4: reducción 1, se mueven cada 2 turnos.
    // - Nivel 5 en adelante: el mínimo queda fijado en 1 turno.
    private int ObtenerTurnosPorMovimientoAlien()
    {
        int reduccion = (_nivelActual - 1) / 2;

        // Math.Max impide que el intervalo llegue a cero o sea negativo. Un valor
        // cero no podría utilizarse en la operación de resto de la función anterior.
        return Math.Max(
            TurnosInicialesPorMovimientoAlien - reduccion,
            1);
    }

    // Calcula cuántas filas avanzan los enemigos cuando llega su turno.
    //
    // La división entera crea bloques de ocho niveles. Se suma 1 para que el
    // resultado mínimo sea una fila.
    private int ObtenerFilasPorMovimientoAlien()
    {
        return 1 + (_nivelActual - 1) / NivelesPorFilaAdicional;
    }

    // ------------------------------------------------------------------------
    // IMPACTOS, ELIMINACIÓN Y PUNTUACIÓN
    // ------------------------------------------------------------------------

    // Comprueba todas las combinaciones posibles de balas y alienígenas.
    // Devuelve la cantidad de enemigos destruidos durante esta comprobación.
    private int ProcesarImpactos()
    {
        int aliensDestruidos = 0;

        // Las listas se recorren desde el final porque se eliminan elementos
        // durante el recorrido. Al borrar un elemento, solo cambian los índices de
        // los elementos situados después. Como esos índices ya se han procesado,
        // el recorrido puede continuar sin saltarse elementos.
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

                // Las colisiones se calculan con las coordenadas lógicas, no con
                // las figuras dibujadas en píxeles.
                if (HayImpacto(bala, alien))
                {
                    // Tanto la bala como el enemigo desaparecen tras el impacto.
                    _balas.RemoveAt(indiceBala);
                    _aliens.RemoveAt(indiceAlien);

                    aliensDestruidos++;

                    // El alien ya se ha eliminado, por lo que no debe compararse
                    // con más balas. break termina únicamente el bucle interior.
                    break;
                }
            }
        }

        return aliensDestruidos;
    }

    // Dos entidades colisionan cuando ocupan exactamente la misma columna y fila.
    //
    // El método es static porque no necesita acceder a ningún campo del objeto
    // Juego; su resultado depende únicamente de los dos parámetros recibidos.
    private static bool HayImpacto(Bala bala, Alien alien)
    {
        return bala.X == alien.X && bala.Y == alien.Y;
    }

    // Elimina las balas que han salido por la parte superior del tablero.
    private void EliminarBalasFueraDelTablero()
    {
        // También se recorre desde el final porque RemoveAt modifica los índices.
        for (int indice = _balas.Count - 1; indice >= 0; indice--)
        {
            // Una fila negativa significa que la bala ha superado la fila 0.
            if (_balas[indice].Y < 0)
            {
                _balas.RemoveAt(indice);
            }
        }
    }

    // Añade los puntos de los enemigos destruidos y actualiza el récord en memoria.
    private void RegistrarPuntos(int aliensDestruidos)
    {
        _puntuacion += aliensDestruidos * PuntosPorAlien;

        // El valor del récord se actualiza inmediatamente para que la pantalla
        // muestre el nuevo resultado. El fichero se escribirá al terminar o salir.
        if (_puntuacion > _record)
        {
            _record = _puntuacion;
            _recordModificado = true;
        }
    }

    // Construye un mensaje adecuado para el número de impactos del turno.
    private static string CrearMensajeImpactos(int aliensDestruidos)
    {
        // Sin impactos no se muestra ningún mensaje adicional.
        if (aliensDestruidos == 0)
        {
            return string.Empty;
        }

        int puntosGanados = aliensDestruidos * PuntosPorAlien;

        // Se utiliza una frase singular para un único enemigo.
        if (aliensDestruidos == 1)
        {
            return $"¡Alien destruido! +{puntosGanados} puntos.";
        }

        // Para dos o más enemigos se utiliza la forma plural.
        return $"¡{aliensDestruidos} aliens destruidos! "
            + $"+{puntosGanados} puntos.";
    }

    // Comprueba si algún alienígena ha alcanzado o superado la fila de la nave.
    private bool HayInvasion()
    {
        foreach (Alien alien in _aliens)
        {
            // Se utiliza >= y no == porque, en niveles altos, un alien puede
            // avanzar varias filas dentro del mismo turno.
            if (alien.Y >= _nave.Y)
            {
                return true;
            }
        }

        return false;
    }

    // ------------------------------------------------------------------------
    // CREACIÓN Y PROGRESIÓN DE NIVELES
    // ------------------------------------------------------------------------

    // Limpia los elementos temporales del nivel anterior y crea la nueva oleada.
    private void PrepararSiguienteNivel()
    {
        // La dificultad se calcula a partir del nuevo número de nivel.
        _nivelActual++;

        // El contador vuelve a cero para comenzar el intervalo del nuevo nivel
        // desde su primer turno.
        _numeroTurno = 0;

        // Las balas no se conservan entre oleadas. Esto evita que una bala del
        // nivel anterior destruya inmediatamente un enemigo recién creado.
        _balas.Clear();
        _aliens.Clear();

        CrearAliensDelNivelActual();
    }

    // Crea la cantidad de enemigos correspondiente al nivel actual.
    private void CrearAliensDelNivelActual()
    {
        int numeroAliens = ObtenerNumeroAliensDelNivel();

        // Se repite hasta alcanzar la cantidad calculada.
        while (_aliens.Count < numeroAliens)
        {
            // Next(Ancho) genera un valor comprendido entre 0 y Ancho - 1.
            int columna = _aleatorio.Next(Ancho);

            // Todos los alienígenas comienzan en la fila 0 y en columnas distintas.
            // Evitar duplicados hace visible cada enemigo desde el inicio.
            if (!ExisteAlienEnColumna(columna))
            {
                _aliens.Add(new Alien(columna, 0));
            }
        }
    }

    // Calcula la cantidad de alienígenas de la oleada.
    private int ObtenerNumeroAliensDelNivel()
    {
        // Cada nivel añade un enemigo:
        //
        // - Nivel 1: 3.
        // - Nivel 2: 4.
        // - Nivel 3: 5.
        //
        // Math.Min limita la cantidad al ancho porque cada enemigo comienza en
        // una columna diferente y solo existen doce columnas disponibles.
        return Math.Min(
            NumeroInicialAliens + _nivelActual - 1,
            Ancho);
    }

    // Indica si la columna recibida ya está ocupada por un alienígena.
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

    // Crea el texto informativo mostrado al comenzar cada oleada.
    private string CrearMensajeInicioNivel()
    {
        return $"Nivel {_nivelActual}: {_aliens.Count} aliens. "
            + $"Movimiento cada {ObtenerTurnosPorMovimientoAlien()} turno(s).";
    }

    // Guarda el récord solo cuando se ha modificado durante la partida.
    // Devuelve un mensaje que puede mostrarse al usuario.
    private string GuardarRecordSiEsNecesario()
    {
        // Si no se ha superado el récord, no hay nada que escribir.
        if (!_recordModificado)
        {
            return string.Empty;
        }

        // GestorRecord devuelve true cuando consigue escribir el archivo.
        if (GestorRecord.Guardar(_record))
        {
            // Se marca como guardado para evitar escrituras duplicadas.
            _recordModificado = false;
            return $"Nuevo récord guardado: {_record} puntos.";
        }

        // Un error de escritura no interrumpe el juego; únicamente se informa.
        return "El nuevo récord no se pudo guardar.";
    }

    // ------------------------------------------------------------------------
    // REPRESENTACIÓN GRÁFICA
    // ------------------------------------------------------------------------

    // Godot ejecuta _Draw() cuando el nodo necesita dibujarse y cada vez que se
    // solicita mediante QueueRedraw().
    //
    // No se debe modificar la lógica del juego desde este método. Su trabajo es
    // únicamente representar el estado actual. Separar dibujo y lógica evita que
    // una actualización visual cambie accidentalmente la partida.
    public override void _Draw()
    {
        // El primer rectángulo cubre toda la ventana y actúa como fondo.
        DrawRect(new Rect2(0, 0, 1040, 760), ColorFondo);

        // El orden de dibujo determina qué elementos quedan encima de otros.
        DibujarInformacion();
        DibujarTablero();
        DibujarEntidades();
        DibujarControles();

        // El cuadro de derrota se dibuja al final para que aparezca por encima del
        // tablero y de las entidades.
        if (_partidaTerminada)
        {
            DibujarPantallaDerrota();
        }
    }

    // Muestra el título y los datos principales de la partida.
    private void DibujarInformacion()
    {
        // El título se coloca utilizando coordenadas absolutas de la ventana.
        DibujarTexto(
            "INVASORES DEL ESPACIO",
            new Vector2(345, 45),
            28,
            ColorNave);

        // La interpolación de cadenas permite insertar valores mediante { }.
        // Los espacios separan visualmente los distintos datos en una misma línea.
        string datos =
            $"Nivel: {_nivelActual}    "
            + $"Puntos: {_puntuacion}    "
            + $"Récord: {_record}    "
            + $"Aliens: {_aliens.Count}";

        DibujarTexto(datos, new Vector2(280, 85), 18, ColorTexto);

        // La segunda línea explica la dificultad efectiva del nivel actual.
        string dificultad =
            $"Movimiento enemigo: cada {ObtenerTurnosPorMovimientoAlien()} "
            + $"turno(s), {ObtenerFilasPorMovimientoAlien()} fila(s)";

        DibujarTexto(dificultad, new Vector2(280, 115), 16, ColorLineas);
    }

    // Dibuja el fondo, el borde y la cuadrícula del tablero.
    private void DibujarTablero()
    {
        // El tamaño visual total se obtiene multiplicando el número de casillas
        // por el tamaño en píxeles de cada una.
        float anchoTablero = Ancho * TamanoCasilla;
        float altoTablero = Alto * TamanoCasilla;

        // Rect2 combina una posición y un tamaño. Aquí representa toda el área del
        // tablero, comenzando en PosicionTablero.
        Rect2 tablero = new Rect2(
            PosicionTablero,
            new Vector2(anchoTablero, altoTablero));

        // El tercer argumento true indica que el rectángulo debe rellenarse.
        DrawRect(tablero, ColorTablero, true);

        // El tercer argumento false dibuja solo el contorno. El último valor es el
        // grosor de la línea en píxeles.
        DrawRect(tablero, ColorLineas, false, 2.0f);

        // Se dibujan las separaciones verticales. No se dibujan los extremos porque
        // ya están incluidos en el contorno exterior.
        for (int columna = 1; columna < Ancho; columna++)
        {
            float x = PosicionTablero.X + columna * TamanoCasilla;

            DrawLine(
                new Vector2(x, PosicionTablero.Y),
                new Vector2(x, PosicionTablero.Y + altoTablero),
                ColorLineas);
        }

        // Se dibujan las separaciones horizontales siguiendo la misma idea.
        for (int fila = 1; fila < Alto; fila++)
        {
            float y = PosicionTablero.Y + fila * TamanoCasilla;

            DrawLine(
                new Vector2(PosicionTablero.X, y),
                new Vector2(PosicionTablero.X + anchoTablero, y),
                ColorLineas);
        }
    }

    // Recorre las colecciones y dibuja cada entidad en su casilla actual.
    private void DibujarEntidades()
    {
        // Los alienígenas se dibujan primero.
        foreach (Alien alien in _aliens)
        {
            // La comprobación evita dibujar objetos que estén fuera del tablero.
            if (alien.EstaDentroDelTablero(Ancho, Alto))
            {
                DibujarAlien(ObtenerCentroCasilla(alien));
            }
        }

        // Las balas se dibujan después. Una bala que haya alcanzado Y = -1 no se
        // representa aunque todavía exista brevemente antes de ser eliminada.
        foreach (Bala bala in _balas)
        {
            if (bala.EstaDentroDelTablero(Ancho, Alto))
            {
                DibujarBala(ObtenerCentroCasilla(bala));
            }
        }

        // La nave siempre permanece dentro del tablero gracias a Math.Clamp.
        DibujarNave(ObtenerCentroCasilla(_nave));
    }

    // Convierte una posición lógica de columna y fila en el centro de una casilla
    // expresado en píxeles.
    //
    // Por ejemplo, para X = 0:
    //
    // PosicionTablero.X + 0 * TamanoCasilla + TamanoCasilla / 2
    //
    // Esto sitúa el objeto en la mitad de la primera casilla y no en su esquina.
    private static Vector2 ObtenerCentroCasilla(Entidad entidad)
    {
        float centroX =
            PosicionTablero.X
            + entidad.X * TamanoCasilla
            + TamanoCasilla / 2;

        float centroY =
            PosicionTablero.Y
            + entidad.Y * TamanoCasilla
            + TamanoCasilla / 2;

        return new Vector2(centroX, centroY);
    }

    // Dibuja la nave como un triángulo sencillo.
    private void DibujarNave(Vector2 centro)
    {
        // Los tres puntos se calculan como desplazamientos relativos respecto al
        // centro de la casilla. Así el dibujo acompaña a la nave al cambiar X.
        Vector2[] puntos =
        {
            // Punta superior.
            centro + new Vector2(0, -15),

            // Esquina inferior derecha.
            centro + new Vector2(15, 14),

            // Esquina inferior izquierda.
            centro + new Vector2(-15, 14)
        };

        // DrawColoredPolygon rellena el polígono definido por los tres puntos.
        DrawColoredPolygon(puntos, ColorNave);
    }

    // Dibuja un alienígena mediante un rectángulo y dos círculos.
    private void DibujarAlien(Vector2 centro)
    {
        // El cuerpo mide 28 por 22 píxeles y se centra restando la mitad de sus
        // dimensiones a la posición recibida.
        Rect2 cuerpo = new Rect2(
            centro + new Vector2(-14, -11),
            new Vector2(28, 22));

        DrawRect(cuerpo, ColorAlien, true);

        // Los ojos son dos círculos blancos colocados de manera simétrica.
        DrawCircle(centro + new Vector2(-6, -2), 2.5f, ColorTexto);
        DrawCircle(centro + new Vector2(6, -2), 2.5f, ColorTexto);
    }

    // Dibuja una bala como un rectángulo vertical estrecho.
    private void DibujarBala(Vector2 centro)
    {
        // La bala mide 4 píxeles de ancho y 18 de alto. Los desplazamientos -2 y
        // -9 permiten centrarla exactamente en la casilla.
        Rect2 proyectil = new Rect2(
            centro + new Vector2(-2, -9),
            new Vector2(4, 18));

        DrawRect(proyectil, ColorBala, true);
    }

    // Muestra el mensaje del turno y una línea permanente con los controles.
    private void DibujarControles()
    {
        // No se intenta dibujar una cadena vacía. Esto evita una llamada gráfica
        // innecesaria en los turnos sin impactos ni mensajes especiales.
        if (!string.IsNullOrWhiteSpace(_mensajeTurno))
        {
            DibujarTexto(
                _mensajeTurno,
                new Vector2(160, 655),
                16,
                ColorTexto);
        }

        DibujarTexto(
            "[←] Izquierda    [→] Derecha    [ESPACIO] Disparar    [X/ESC] Salir",
            new Vector2(235, 710),
            16,
            ColorLineas);
    }

    // Dibuja un cuadro informativo por encima del tablero cuando termina la partida.
    private void DibujarPantallaDerrota()
    {
        // El cuadro se define mediante posición X, posición Y, ancho y alto.
        Rect2 cuadro = new Rect2(345, 300, 350, 190);

        // Primero se rellena con el color del fondo para ocultar parcialmente lo
        // que había debajo. Después se dibuja un borde rojo.
        DrawRect(cuadro, ColorFondo, true);
        DrawRect(cuadro, ColorPeligro, false, 3.0f);

        DibujarTexto("GAME OVER", new Vector2(430, 345), 30, ColorPeligro);
        DibujarTexto(_mensajeResultado, new Vector2(405, 385), 17, ColorTexto);

        DibujarTexto(
            $"Puntuación final: {_puntuacion}",
            new Vector2(420, 420),
            17,
            ColorTexto);

        // El mensaje de récord solo aparece cuando existe algo que comunicar.
        if (!string.IsNullOrWhiteSpace(_mensajeRecord))
        {
            DibujarTexto(
                _mensajeRecord,
                new Vector2(390, 450),
                15,
                ColorBala);
        }

        DibujarTexto(
            "Pulsa R o INTRO para volver a jugar",
            new Vector2(385, 478),
            14,
            ColorTexto);
    }

    // Método auxiliar que centraliza todas las llamadas a DrawString.
    //
    // Gracias a este método, el resto del código solo necesita proporcionar el
    // texto, su posición, tamaño y color. La fuente y los parámetros comunes se
    // configuran una sola vez aquí.
    private void DibujarTexto(
        string texto,
        Vector2 posicion,
        int tamano,
        Color color)
    {
        DrawString(
            _fuente,
            posicion,
            texto,
            HorizontalAlignment.Left,
            -1,
            tamano,
            color);
    }
}

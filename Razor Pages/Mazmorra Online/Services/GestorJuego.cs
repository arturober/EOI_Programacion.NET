using MazmorraOnline.Dtos;
using MazmorraOnline.Models;

namespace MazmorraOnline.Services;

// Contiene las reglas de la partida, la física y los datos compartidos.
// Solo existe una instancia de esta clase para toda la aplicación.
public class GestorJuego
{
    // Todos los mapas tienen 16 columnas, 9 filas y casillas de 60 píxeles.
    private const int ColumnasMapa = 16;
    private const int FilasMapa = 9;
    private const int TamanoCasilla = 60;

    public const int AnchoMapa = ColumnasMapa * TamanoCasilla;
    public const int AltoMapa = FilasMapa * TamanoCasilla;
    public const int MaximoJugadores = 16;

    private const int NumeroPowerUps = 8;
    private const float RadioJugador = 16;

    // Un corte corto de red no debe expulsar inmediatamente al jugador.
    private const int SegundosCortesiaDesconexion = 10;

    // Cinco minutos sin tocar ningún control liberan la plaza ocupada.
    private const int SegundosMaximoInactividad = 5 * 60;

    // A 400 píxeles por segundo cada proyectil avanza 40 píxeles por ciclo.
    private const float VelocidadProyectil = 400;

    // SignalR y el motor utilizan esta clase desde hilos diferentes.
    // lock evita que ambos modifiquen una lista al mismo tiempo.
    private readonly object _bloqueo = new();
    private readonly Partida _partida = new();
    private readonly List<Mapa> _mapas = new();
    private readonly List<ResultadoRondaDto> _resultados = new();
    private Mapa _mapaActual = new();
    private int _numeroRonda;
    private int _numeroParticipantesRonda;

    private readonly string[] _colores =
    {
        "#4dabf7", "#ff6b6b", "#51cf66", "#ffd43b",
        "#b197fc", "#ff922b", "#22b8cf", "#f06595",
        "#74c0fc", "#ffa8a8", "#8ce99a", "#ffe066",
        "#d0bfff", "#ffc078", "#66d9e8", "#faa2c1"
    };

    public GestorJuego()
    {
        // Los mapas se leen una sola vez cuando arranca la aplicación.
        CargarMapas();
        ElegirMapaAleatorio();
        CompletarPowerUps();
    }

    // Crea un jugador nuevo y devuelve el identificador que usará su navegador.
    public AccesoJuegoRespuesta Entrar(string nombre)
    {
        // ComprobarNombre también elimina espacios al principio y al final.
        nombre = ComprobarNombre(nombre);

        lock (_bloqueo)
        {
            // Una partida vacía comienza una nueva espera con un mapa al azar.
            // El mapa no volverá a cambiar cuando llegue el segundo jugador.
            if (_partida.Estado == EstadoPartida.Esperando
                && _partida.Jugadores.Count == 0)
            {
                PrepararMapaDeEspera();
            }

            if (_partida.Jugadores.Count >= MaximoJugadores)
            {
                throw new InvalidOperationException(
                    "La partida ha alcanzado el límite de 16 jugadores.");
            }

            if (_partida.Jugadores.Values.Any(jugador =>
                jugador.Nombre.Equals(
                    nombre, StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException(
                    "Ese nombre ya está siendo utilizado.");
            }

            Jugador jugador = new Jugador
            {
                Nombre = nombre,
                Color = ElegirColor()
            };

            if (_partida.Estado == EstadoPartida.Finalizada)
            {
                jugador.Vivo = false;
                jugador.X = AnchoMapa / 2;
                jugador.Y = AltoMapa / 2;
            }
            else
            {
                ColocarJugador(jugador);
            }

            _partida.Jugadores.Add(jugador.Id, jugador);

            if (_partida.Estado == EstadoPartida.Esperando
                && _partida.Jugadores.Count >= 2)
            {
                // La primera ronda usa el mapa en el que estaba practicando
                // la persona que llegó primero.
                IniciarRonda(elegirNuevoMapa: false);
            }
            else if (_partida.Estado == EstadoPartida.EnJuego)
            {
                _numeroParticipantesRonda =
                    Math.Max(
                        _numeroParticipantesRonda,
                        _partida.Jugadores.Count);
            }

            return new AccesoJuegoRespuesta
            {
                JugadorId = jugador.Id
            };
        }
    }

    // Indica si un identificador todavía pertenece a la partida.
    public bool ExisteJugador(string jugadorId)
    {
        lock (_bloqueo)
        {
            return _partida.Jugadores.ContainsKey(jugadorId);
        }
    }

    // Asocia la conexión actual con el jugador y cancela una desconexión previa.
    public bool ConectarJugador(
        string jugadorId,
        string conexionId)
    {
        lock (_bloqueo)
        {
            if (!_partida.Jugadores.TryGetValue(
                jugadorId, out Jugador? jugador))
            {
                return false;
            }

            if (jugador.DesconectadoDesde.HasValue)
            {
                // El tiempo sin red no se considera tiempo de inactividad.
                TimeSpan duracionDesconexion =
                    DateTime.UtcNow - jugador.DesconectadoDesde.Value;

                jugador.UltimaActividad =
                    jugador.UltimaActividad.Add(duracionDesconexion);
            }

            jugador.ConexionId = conexionId;
            jugador.DesconectadoDesde = null;
            return true;
        }
    }

    // Marca una desconexión, pero deja unos segundos para poder reconectar.
    public void MarcarJugadorDesconectado(
        string jugadorId,
        string conexionId)
    {
        lock (_bloqueo)
        {
            if (!_partida.Jugadores.TryGetValue(
                jugadorId, out Jugador? jugador))
            {
                return;
            }

            // Una conexión antigua puede cerrarse después de que ya exista
            // otra nueva. En ese caso no se modifica al jugador.
            if (jugador.ConexionId != conexionId)
            {
                return;
            }

            jugador.ConexionId = null;
            jugador.DesconectadoDesde = DateTime.UtcNow;

            // El personaje se detiene mientras se intenta recuperar la red.
            jugador.Accion = new AccionJugador();
        }
    }

    // Sustituye la acción anterior del jugador por la más reciente.
    public void GuardarAccion(
        string jugadorId,
        string conexionId,
        AccionJugador accion)
    {
        lock (_bloqueo)
        {
            if (_partida.Jugadores.TryGetValue(
                jugadorId, out Jugador? jugador)
                && jugador.ConexionId == conexionId)
            {
                if (EsInteraccionReal(jugador.Accion, accion))
                {
                    jugador.UltimaActividad = DateTime.UtcNow;
                }

                // Siempre se conserva la acción más reciente.
                jugador.Accion = accion;
            }
        }
    }

    // Quita un jugador y adapta la ronda al número de participantes restante.
    public bool EliminarJugador(string jugadorId)
    {
        lock (_bloqueo)
        {
            return EliminarJugadorInterno(jugadorId);
        }
    }

    // Crea una copia reducida del estado para enviarla mediante SignalR.
    public EstadoPartidaDto ObtenerEstado()
    {
        lock (_bloqueo)
        {
            return CrearEstado();
        }
    }

    // Avanza los temporizadores y la física el número de segundos indicado.
    public void Actualizar(float segundos)
    {
        lock (_bloqueo)
        {
            EliminarJugadoresAusentes();

            if (_partida.Estado == EstadoPartida.Esperando)
            {
                // Durante la espera se puede practicar sin iniciar una ronda.
                ActualizarEspera(segundos);
            }
            else if (_partida.Estado == EstadoPartida.EnJuego)
            {
                ActualizarRonda(segundos);
            }
            else if (_partida.Estado == EstadoPartida.Finalizada)
            {
                _partida.SegundosParaReiniciar -= segundos;

                if (_partida.SegundosParaReiniciar <= 0)
                {
                    if (_partida.Jugadores.Count >= 2)
                    {
                        // Las rondas siguientes sí pueden utilizar otro mapa.
                        IniciarRonda(elegirNuevoMapa: true);
                    }
                    else
                    {
                        PrepararEspera();
                    }
                }
            }
        }
    }

    // Elimina conexiones perdidas y navegadores abandonados durante demasiado
    // tiempo. Se ejecuta dentro del mismo lock que la física.
    private void EliminarJugadoresAusentes()
    {
        DateTime ahora = DateTime.UtcNow;

        List<string> jugadoresParaEliminar = _partida.Jugadores.Values
            .Where(jugador =>
                LlevaDemasiadoDesconectado(jugador, ahora)
                || LlevaDemasiadoInactivo(jugador, ahora))
            .Select(jugador => jugador.Id)
            .ToList();

        foreach (string jugadorId in jugadoresParaEliminar)
        {
            EliminarJugadorInterno(jugadorId);
        }
    }

    private static bool LlevaDemasiadoDesconectado(
        Jugador jugador,
        DateTime ahora)
    {
        return jugador.DesconectadoDesde.HasValue
            && (ahora - jugador.DesconectadoDesde.Value).TotalSeconds
                >= SegundosCortesiaDesconexion;
    }

    private static bool LlevaDemasiadoInactivo(
        Jugador jugador,
        DateTime ahora)
    {
        // Durante una desconexión se aplica únicamente su periodo de cortesía.
        return !jugador.DesconectadoDesde.HasValue
            && (ahora - jugador.UltimaActividad).TotalSeconds
                >= SegundosMaximoInactividad;
    }

    // Centraliza la eliminación para la salida voluntaria y las caducidades.
    private bool EliminarJugadorInterno(string jugadorId)
    {
        bool eliminado = _partida.Jugadores.Remove(jugadorId);

        if (!eliminado)
        {
            return false;
        }

        if (_partida.Jugadores.Count < 2)
        {
            PrepararEspera();
        }
        else
        {
            ComprobarGanador();
        }

        return true;
    }

    // Los paquetes idénticos enviados cada 100 ms no cuentan como actividad.
    private static bool EsInteraccionReal(
        AccionJugador anterior,
        AccionJugador nueva)
    {
        bool hayControlPulsado =
            nueva.Arriba || nueva.Abajo
            || nueva.Izquierda || nueva.Derecha
            || nueva.Disparar;

        bool haCambiadoUnControl =
            anterior.Arriba != nueva.Arriba
            || anterior.Abajo != nueva.Abajo
            || anterior.Izquierda != nueva.Izquierda
            || anterior.Derecha != nueva.Derecha
            || anterior.Disparar != nueva.Disparar;

        bool haCambiadoElAngulo =
            MathF.Abs(anterior.Angulo - nueva.Angulo) > 0.001f;

        return hayControlPulsado
            || haCambiadoUnControl
            || haCambiadoElAngulo;
    }

    // Devuelve copias de los mapas para no exponer las listas internas.
    public List<MapaDto> ObtenerMapas()
    {
        lock (_bloqueo)
        {
            return _mapas
                .Select(mapa => new MapaDto
                {
                    Nombre = mapa.Nombre,
                    Filas = new List<string>(mapa.Filas)
                })
                .ToList();
        }
    }

    // Ordena los jugadores por victorias, eliminaciones y nombre.
    public List<ClasificacionJugadorDto> ObtenerClasificacion()
    {
        lock (_bloqueo)
        {
            return _partida.Jugadores.Values
                .OrderByDescending(jugador => jugador.Victorias)
                .ThenByDescending(jugador => jugador.Eliminaciones)
                .ThenBy(jugador => jugador.Nombre)
                .Select(jugador => new ClasificacionJugadorDto
                {
                    Id = jugador.Id,
                    Nombre = jugador.Nombre,
                    Victorias = jugador.Victorias,
                    Eliminaciones = jugador.Eliminaciones
                })
                .ToList();
        }
    }

    // Busca un jugador y devuelve sus datos públicos más detallados.
    public JugadorDetalleDto? ObtenerJugador(string jugadorId)
    {
        lock (_bloqueo)
        {
            if (!_partida.Jugadores.TryGetValue(
                jugadorId, out Jugador? jugador))
            {
                return null;
            }

            return new JugadorDetalleDto
            {
                Id = jugador.Id,
                Nombre = jugador.Nombre,
                Vida = jugador.Vida,
                Vivo = jugador.Vivo,
                Victorias = jugador.Victorias,
                Eliminaciones = jugador.Eliminaciones,
                TieneEscudo = jugador.TiempoEscudo > 0,
                TieneVelocidad = jugador.TiempoVelocidad > 0,
                TieneDisparoRapido =
                    jugador.TiempoDisparoRapido > 0
            };
        }
    }

    // Devuelve una copia del historial para proteger la lista original.
    public List<ResultadoRondaDto> ObtenerResultados()
    {
        lock (_bloqueo)
        {
            return _resultados
                .Select(resultado => new ResultadoRondaDto
                {
                    Numero = resultado.Numero,
                    Ganador = resultado.Ganador,
                    NumeroJugadores = resultado.NumeroJugadores,
                    Mapa = resultado.Mapa,
                    Fecha = resultado.Fecha
                })
                .ToList();
        }
    }

    // Ejecuta todos los pasos que forman una actualización de una ronda activa.
    private void ActualizarRonda(float segundos)
    {
        _partida.SegundosRestantes -= segundos;
        ActualizarJugadores(segundos);
        ActualizarProyectiles(segundos);
        RecogerPowerUps();

        if (_partida.SegundosRestantes <= 0)
        {
            TerminarPorTiempo();
        }
        else
        {
            ComprobarGanador();
        }
    }

    // Permite moverse y disparar mientras llega un segundo jugador.
    private void ActualizarEspera(float segundos)
    {
        ActualizarJugadores(segundos);
        ActualizarProyectiles(segundos);

        // No se recogen mejoras ni se comprueba un ganador porque todavía
        // no existe una ronda que pueda otorgar puntos.
    }

    // Procesa las acciones comunes a la espera y a una ronda activa.
    private void ActualizarJugadores(float segundos)
    {
        foreach (Jugador jugador in _partida.Jugadores.Values)
        {
            if (!jugador.Vivo)
            {
                continue;
            }

            ReducirTemporizadores(jugador, segundos);

            AccionJugador accion = jugador.Accion;
            jugador.Angulo = accion.Angulo;

            MoverJugador(jugador, accion, segundos);

            if (accion.Disparar && jugador.TiempoRecarga <= 0)
            {
                CrearProyectil(jugador);

                jugador.TiempoRecarga =
                    jugador.TiempoDisparoRapido > 0 ? 0.18f : 0.45f;
            }
        }
    }

    // Convierte las cuatro direcciones en un desplazamiento sin ventaja diagonal.
    private void MoverJugador(
        Jugador jugador,
        AccionJugador accion,
        float segundos)
    {
        float movimientoX = 0;
        float movimientoY = 0;

        if (accion.Izquierda) movimientoX--;
        if (accion.Derecha) movimientoX++;
        if (accion.Arriba) movimientoY--;
        if (accion.Abajo) movimientoY++;

        // Evita que el movimiento diagonal sea más rápido.
        float longitud = MathF.Sqrt(
            movimientoX * movimientoX + movimientoY * movimientoY);

        if (longitud > 0)
        {
            movimientoX /= longitud;
            movimientoY /= longitud;
        }

        float velocidad =
            jugador.TiempoVelocidad > 0 ? 240 : 160;

        float nuevaX =
            jugador.X + movimientoX * velocidad * segundos;
        float nuevaY =
            jugador.Y + movimientoY * velocidad * segundos;

        nuevaX = Math.Clamp(
            nuevaX, RadioJugador, AnchoMapa - RadioJugador);
        nuevaY = Math.Clamp(
            nuevaY, RadioJugador, AltoMapa - RadioJugador);

        // Se comprueba cada eje por separado para deslizarse por los muros.
        if (!ChocaConMuro(nuevaX, jugador.Y, RadioJugador))
        {
            jugador.X = nuevaX;
        }

        if (!ChocaConMuro(jugador.X, nuevaY, RadioJugador))
        {
            jugador.Y = nuevaY;
        }
    }

    // Crea un proyectil delante del jugador y en la dirección de su ángulo.
    private void CrearProyectil(Jugador jugador)
    {
        float direccionX = MathF.Cos(jugador.Angulo);
        float direccionY = MathF.Sin(jugador.Angulo);

        Proyectil proyectil = new Proyectil
        {
            PropietarioId = jugador.Id,
            X = jugador.X + direccionX * 22,
            Y = jugador.Y + direccionY * 22,
            VelocidadX = direccionX * VelocidadProyectil,
            VelocidadY = direccionY * VelocidadProyectil
        };

        _partida.Proyectiles.Add(proyectil);
    }

    // Mueve los proyectiles y comprueba muros, límites e impactos.
    private void ActualizarProyectiles(float segundos)
    {
        for (int i = _partida.Proyectiles.Count - 1; i >= 0; i--)
        {
            Proyectil proyectil = _partida.Proyectiles[i];
            proyectil.X += proyectil.VelocidadX * segundos;
            proyectil.Y += proyectil.VelocidadY * segundos;

            bool fueraDelMapa =
                proyectil.X < 0 || proyectil.X > AnchoMapa
                || proyectil.Y < 0 || proyectil.Y > AltoMapa;

            if (fueraDelMapa || ChocaConMuro(
                proyectil.X, proyectil.Y, 4))
            {
                _partida.Proyectiles.RemoveAt(i);
                continue;
            }

            Jugador? alcanzado = _partida.Jugadores.Values
                .FirstOrDefault(jugador =>
                    jugador.Vivo
                    && jugador.Id != proyectil.PropietarioId
                    && DistanciaCuadrada(
                        jugador.X, jugador.Y,
                        proyectil.X, proyectil.Y) <= 400);

            if (alcanzado is not null)
            {
                int dano = alcanzado.TiempoEscudo > 0 ? 10 : 25;
                alcanzado.Vida = Math.Max(0, alcanzado.Vida - dano);

                if (alcanzado.Vida == 0)
                {
                    alcanzado.Vivo = false;

                    if (_partida.Jugadores.TryGetValue(
                        proyectil.PropietarioId,
                        out Jugador? atacante))
                    {
                        atacante.Eliminaciones++;
                    }
                }

                _partida.Proyectiles.RemoveAt(i);
            }
        }
    }

    // Aplica los power-ups cercanos y repone los que se han recogido.
    private void RecogerPowerUps()
    {
        foreach (Jugador jugador in _partida.Jugadores.Values)
        {
            if (!jugador.Vivo)
            {
                continue;
            }

            for (int i = _partida.PowerUps.Count - 1; i >= 0; i--)
            {
                PowerUp powerUp = _partida.PowerUps[i];

                if (DistanciaCuadrada(
                    jugador.X, jugador.Y,
                    powerUp.X, powerUp.Y) > 625)
                {
                    continue;
                }

                AplicarPowerUp(jugador, powerUp.Tipo);
                _partida.PowerUps.RemoveAt(i);
            }
        }

        CompletarPowerUps();
    }

    // Modifica el jugador según el tipo de mejora recogida.
    private static void AplicarPowerUp(
        Jugador jugador, TipoPowerUp tipo)
    {
        switch (tipo)
        {
            case TipoPowerUp.Vida:
                jugador.Vida = Math.Min(100, jugador.Vida + 40);
                break;
            case TipoPowerUp.Escudo:
                jugador.TiempoEscudo = 8;
                break;
            case TipoPowerUp.Velocidad:
                jugador.TiempoVelocidad = 8;
                break;
            case TipoPowerUp.DisparoRapido:
                jugador.TiempoDisparoRapido = 8;
                break;
        }
    }

    // Descuenta el tiempo transcurrido sin permitir valores negativos.
    private static void ReducirTemporizadores(
        Jugador jugador, float segundos)
    {
        jugador.TiempoRecarga =
            Math.Max(0, jugador.TiempoRecarga - segundos);
        jugador.TiempoEscudo =
            Math.Max(0, jugador.TiempoEscudo - segundos);
        jugador.TiempoVelocidad =
            Math.Max(0, jugador.TiempoVelocidad - segundos);
        jugador.TiempoDisparoRapido =
            Math.Max(0, jugador.TiempoDisparoRapido - segundos);
    }

    // Finaliza la ronda cuando queda como máximo un jugador vivo.
    private void ComprobarGanador()
    {
        if (_partida.Estado != EstadoPartida.EnJuego)
        {
            return;
        }

        List<Jugador> vivos = _partida.Jugadores.Values
            .Where(jugador => jugador.Vivo)
            .ToList();

        if (vivos.Count <= 1)
        {
            FinalizarRonda(vivos.FirstOrDefault());
        }
    }

    // Al agotarse el tiempo gana el jugador vivo que conserve más vida.
    private void TerminarPorTiempo()
    {
        Jugador? ganador = _partida.Jugadores.Values
            .Where(jugador => jugador.Vivo)
            .OrderByDescending(jugador => jugador.Vida)
            .FirstOrDefault();

        FinalizarRonda(ganador);
    }

    // Guarda el resultado y prepara la cuenta atrás para la siguiente ronda.
    private void FinalizarRonda(Jugador? ganador)
    {
        _partida.Estado = EstadoPartida.Finalizada;
        _partida.SegundosRestantes = 0;
        _partida.SegundosParaReiniciar = 5;
        _partida.Ganador = ganador?.Nombre;
        _partida.Proyectiles.Clear();

        if (ganador is not null)
        {
            ganador.Victorias++;
        }

        _resultados.Insert(0, new ResultadoRondaDto
        {
            Numero = _numeroRonda,
            Ganador = ganador?.Nombre,
            NumeroJugadores = _numeroParticipantesRonda,
            Mapa = _mapaActual.Nombre,
            Fecha = DateTime.UtcNow
        });

        // El historial es intencionadamente corto y solo vive en memoria.
        if (_resultados.Count > 10)
        {
            _resultados.RemoveAt(_resultados.Count - 1);
        }
    }

    // Reinicia los objetos y coloca a todos los participantes.
    private void IniciarRonda(bool elegirNuevoMapa)
    {
        _partida.Estado = EstadoPartida.EnJuego;
        _partida.SegundosRestantes = 90;
        _partida.SegundosParaReiniciar = 0;
        _partida.Ganador = null;
        _partida.Proyectiles.Clear();
        _partida.PowerUps.Clear();

        _numeroRonda++;
        _numeroParticipantesRonda = _partida.Jugadores.Count;

        if (elegirNuevoMapa)
        {
            ElegirMapaAleatorio();
        }

        PrepararJugadores();
        CompletarPowerUps();
    }

    // Sortea el mapa que utilizará el primer jugador durante la espera.
    private void PrepararMapaDeEspera()
    {
        ElegirMapaAleatorio();
        _partida.Proyectiles.Clear();
        _partida.PowerUps.Clear();
        CompletarPowerUps();
    }

    // Devuelve el juego al estado de espera cuando faltan jugadores.
    private void PrepararEspera()
    {
        _partida.Estado = EstadoPartida.Esperando;
        _partida.SegundosRestantes = 90;
        _partida.SegundosParaReiniciar = 0;
        _partida.Ganador = null;
        _partida.Proyectiles.Clear();

        PrepararJugadores();
    }

    // Reinicia y coloca a todos los jugadores sin superponerlos.
    private void PrepararJugadores()
    {
        // Se ignoran las posiciones de la ronda anterior.
        foreach (Jugador jugador in _partida.Jugadores.Values)
        {
            jugador.Vivo = false;
        }

        foreach (Jugador jugador in _partida.Jugadores.Values)
        {
            ReiniciarJugador(jugador);
            ColocarJugador(jugador);
        }
    }

    // Lee todos los archivos .txt de la carpeta Mapas.
    private void CargarMapas()
    {
        string carpeta = Path.Combine(
            AppContext.BaseDirectory, "Mapas");

        string[] archivos = Directory
            .GetFiles(carpeta, "*.txt")
            .OrderBy(archivo => archivo)
            .ToArray();

        if (archivos.Length == 0)
        {
            throw new InvalidOperationException(
                "No hay mapas en la carpeta Mapas.");
        }

        foreach (string archivo in archivos)
        {
            _mapas.Add(LeerMapa(archivo));
        }
    }

    // Convierte las letras de un archivo en muros y posiciones disponibles.
    private static Mapa LeerMapa(string archivo)
    {
        string[] filas = File.ReadAllLines(archivo);

        if (filas.Length != FilasMapa
            || filas.Any(fila => fila.Length != ColumnasMapa))
        {
            throw new InvalidOperationException(
                "Los mapas deben medir 16 columnas por 9 filas.");
        }

        Mapa mapa = new Mapa
        {
            Nombre = Path.GetFileNameWithoutExtension(archivo),
            Filas = filas.ToList()
        };

        for (int fila = 0; fila < filas.Length; fila++)
        {
            for (int columna = 0;
                 columna < filas[fila].Length;
                 columna++)
            {
                float x = columna * TamanoCasilla;
                float y = fila * TamanoCasilla;
                char casilla = filas[fila][columna];

                if (casilla == '#')
                {
                    mapa.Muros.Add(new Muro
                    {
                        X = x,
                        Y = y
                    });
                }
                else if (casilla == 'J')
                {
                    mapa.PosicionesJugadores.Add((
                        x + TamanoCasilla / 2,
                        y + TamanoCasilla / 2));
                }
                else if (casilla == 'P')
                {
                    mapa.PosicionesPowerUps.Add((
                        x + TamanoCasilla / 2,
                        y + TamanoCasilla / 2));
                }
            }
        }

        if (mapa.PosicionesJugadores.Count < MaximoJugadores
            || mapa.PosicionesPowerUps.Count < NumeroPowerUps)
        {
            throw new InvalidOperationException(
                "Cada mapa necesita al menos 16 J y 8 P.");
        }

        return mapa;
    }

    // Selecciona cualquiera de los mapas cargados con la misma probabilidad.
    private void ElegirMapaAleatorio()
    {
        _mapaActual = _mapas[Random.Shared.Next(_mapas.Count)];
    }

    // Añade mejoras hasta recuperar la cantidad configurada.
    private void CompletarPowerUps()
    {
        while (_partida.PowerUps.Count < NumeroPowerUps)
        {
            List<(float X, float Y)> posicionesLibres =
                _mapaActual.PosicionesPowerUps
                    .Where(posicion =>
                        !_partida.PowerUps.Any(powerUp =>
                            powerUp.X == posicion.X
                            && powerUp.Y == posicion.Y))
                    .ToList();

            (float X, float Y) posicion =
                posicionesLibres[Random.Shared.Next(
                    posicionesLibres.Count)];

            _partida.PowerUps.Add(new PowerUp
            {
                Tipo = (TipoPowerUp)Random.Shared.Next(4),
                X = posicion.X,
                Y = posicion.Y
            });
        }
    }

    // Busca una posición inicial que no esté ocupada por otro jugador vivo.
    private void ColocarJugador(Jugador jugador)
    {
        // Se empieza por un punto al azar y se busca el primero libre.
        List<(float X, float Y)> posiciones =
            _mapaActual.PosicionesJugadores;
        int inicio = Random.Shared.Next(posiciones.Count);

        for (int i = 0; i < posiciones.Count; i++)
        {
            int indice = (inicio + i) % posiciones.Count;
            (float X, float Y) posicion =
                posiciones[indice];

            bool ocupada = _partida.Jugadores.Values.Any(otroJugador =>
                    otroJugador.Id != jugador.Id
                    && otroJugador.Vivo
                    && DistanciaCuadrada(
                        posicion.X, posicion.Y,
                        otroJugador.X, otroJugador.Y) < 1600);

            if (!ocupada)
            {
                jugador.X = posicion.X;
                jugador.Y = posicion.Y;
                jugador.Vivo = true;
                return;
            }
        }

        // Solo se alcanzaría este punto si las 16 posiciones estuvieran
        // ocupadas, algo que no sucede con el límite de 16 jugadores.
        jugador.X = posiciones[0].X;
        jugador.Y = posiciones[0].Y;
        jugador.Vivo = true;
    }

    // Recupera los valores iniciales sin borrar victorias ni eliminaciones.
    private static void ReiniciarJugador(Jugador jugador)
    {
        // Las estadísticas acumuladas no se modifican entre rondas.
        jugador.Vida = 100;
        jugador.Vivo = true;
        jugador.Angulo = 0;
        jugador.TiempoEscudo = 0;
        jugador.TiempoVelocidad = 0;
        jugador.TiempoDisparoRapido = 0;
        jugador.TiempoRecarga = 0;
        jugador.Accion = new AccionJugador();
    }

    // Elige el primer color que todavía no utiliza otro jugador.
    private string ElegirColor()
    {
        return _colores.First(color =>
            !_partida.Jugadores.Values.Any(jugador =>
                jugador.Color == color));
    }

    // Comprueba la intersección entre un círculo y cualquiera de los muros.
    private bool ChocaConMuro(float x, float y, float radio)
    {
        return _mapaActual.Muros.Any(muro =>
            x + radio > muro.X
            && x - radio < muro.X + TamanoCasilla
            && y + radio > muro.Y
            && y - radio < muro.Y + TamanoCasilla);
    }

    // Convierte los modelos internos en objetos pequeños para el navegador.
    private EstadoPartidaDto CrearEstado()
    {
        return new EstadoPartidaDto
        {
            Estado = _partida.Estado,
            NumeroRonda = _numeroRonda,
            NombreMapa = _mapaActual.Nombre,
            SegundosRestantes =
                Math.Max(0, (int)MathF.Ceiling(
                    _partida.SegundosRestantes)),
            SegundosParaReiniciar =
                Math.Max(0, (int)MathF.Ceiling(
                    _partida.SegundosParaReiniciar)),
            Ganador = _partida.Ganador,
            Jugadores = _partida.Jugadores.Values
                .Select(jugador => new JugadorEstadoDto
                {
                    Id = jugador.Id,
                    Nombre = jugador.Nombre,
                    Color = jugador.Color,
                    X = jugador.X,
                    Y = jugador.Y,
                    Angulo = jugador.Angulo,
                    Vida = jugador.Vida,
                    Vivo = jugador.Vivo,
                    TieneEscudo = jugador.TiempoEscudo > 0,
                    Victorias = jugador.Victorias,
                    Eliminaciones = jugador.Eliminaciones
                })
                .ToList(),
            Proyectiles = _partida.Proyectiles
                .Select(proyectil => new ProyectilEstadoDto
                {
                    X = proyectil.X,
                    Y = proyectil.Y
                })
                .ToList(),
            PowerUps = _partida.PowerUps
                .Select(powerUp => new PowerUp
                {
                    Tipo = powerUp.Tipo,
                    X = powerUp.X,
                    Y = powerUp.Y
                })
                .ToList()
        };
    }

    // Limpia y valida el nombre recibido desde Razor Pages o la API.
    private static string ComprobarNombre(string nombre)
    {
        nombre = nombre?.Trim() ?? "";

        if (nombre.Length < 2 || nombre.Length > 15)
        {
            throw new InvalidOperationException(
                "El nombre debe tener entre 2 y 15 caracteres.");
        }

        return nombre;
    }

    // La distancia al cuadrado evita calcular una raíz cuadrada innecesaria.
    private static float DistanciaCuadrada(
        float x1, float y1, float x2, float y2)
    {
        float diferenciaX = x1 - x2;
        float diferenciaY = y1 - y2;

        return diferenciaX * diferenciaX
            + diferenciaY * diferenciaY;
    }
}

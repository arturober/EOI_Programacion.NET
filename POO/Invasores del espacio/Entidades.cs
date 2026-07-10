using System;

// ============================================================================
// ENTIDADES DEL JUEGO
// ============================================================================
//
// Este archivo contiene los objetos que pueden aparecer dentro del tablero:
// - La nave controlada por el jugador.
// - Los alienígenas enemigos.
// - Las balas disparadas por la nave.
//
// Todas estas clases comparten una posición, un icono y la capacidad de
// actualizarse. Por ese motivo heredan de la clase abstracta Entidad.
//
// La herencia permite concentrar en un único lugar las características comunes
// y evita repetir las propiedades X, Y e Icono en las tres clases derivadas.
// ============================================================================

abstract class Entidad
{
    // El constructor es protected porque Entidad es una clase abstracta.
    // Eso significa que no se crearán objetos de tipo Entidad directamente.
    // Solo podrán llamar a este constructor las clases que hereden de ella.
    protected Entidad(int x, int y, char icono)
    {
        X = x;
        Y = y;
        Icono = icono;
    }

    // X representa la columna ocupada por la entidad.
    //
    // El setter es protected para impedir que cualquier clase externa cambie
    // libremente la posición. Solo Entidad y sus clases derivadas pueden hacerlo.
    public int X { get; protected set; }

    // Y representa la fila ocupada por la entidad.
    // La fila 0 es la parte superior del tablero.
    public int Y { get; protected set; }

    // El icono identifica visualmente la entidad cuando se dibuja el tablero.
    // Solo se asigna en el constructor, por lo que no necesita un setter.
    public char Icono { get; }

    // Cada entidad se actualiza de una manera diferente:
    // - La nave se mueve según la acción del jugador.
    // - Los alienígenas avanzan hacia la parte inferior.
    // - Las balas se desplazan hacia la parte superior.
    //
    // El método abstracto obliga a cada clase derivada a implementar su propio
    // comportamiento. De esta forma se aplica polimorfismo de manera sencilla.
    public abstract void Actualizar(
        AccionJugador accion,
        int anchoTablero);

    // Antes de dibujar una entidad se comprueba que sus coordenadas pertenecen
    // al tablero. Esta comprobación es especialmente importante para las balas,
    // ya que pueden alcanzar temporalmente la fila -1 antes de ser eliminadas.
    public bool EstaDentroDelTablero(int anchoTablero, int altoTablero)
    {
        return X >= 0
            && X < anchoTablero
            && Y >= 0
            && Y < altoTablero;
    }
}

class Nave : Entidad
{
    // El icono se declara como constante porque todas las naves del juego
    // utilizarían siempre el mismo carácter.
    private const char IconoNave = 'A';

    public Nave(int x, int y)
        : base(x, y, IconoNave)
    {
    }

    public override void Actualizar(
        AccionJugador accion,
        int anchoTablero)
    {
        // La nave solo modifica su posición cuando el jugador solicita moverse.
        // Disparar no se gestiona aquí porque esa acción implica crear otra
        // entidad, una Bala, y esa responsabilidad pertenece al motor del juego.
        if (accion == AccionJugador.Izquierda)
        {
            X--;
        }
        else if (accion == AccionJugador.Derecha)
        {
            X++;
        }

        // Math.Clamp limita X al intervalo válido del tablero.
        //
        // El primer índice válido es 0 y el último es anchoTablero - 1 porque
        // los índices de arrays y matrices comienzan en cero.
        X = Math.Clamp(X, 0, anchoTablero - 1);
    }
}

class Alien : Entidad
{
    private const char IconoAlien = 'V';

    public Alien(int x, int y)
        : base(x, y, IconoAlien)
    {
    }

    public override void Actualizar(
        AccionJugador accion,
        int anchoTablero)
    {
        // Los parámetros no se utilizan en este tipo de entidad, pero deben
        // mantenerse porque forman parte del contrato definido por Entidad.
        //
        // Aumentar Y desplaza el alien una fila hacia abajo, es decir, hacia la
        // nave del jugador.
        Y++;
    }
}

class Bala : Entidad
{
    private const char IconoBala = '|';

    public Bala(int x, int y)
        : base(x, y, IconoBala)
    {
    }

    public override void Actualizar(
        AccionJugador accion,
        int anchoTablero)
    {
        // En una matriz, las filas superiores tienen índices menores.
        // Por eso reducir Y hace que la bala suba por el tablero.
        Y--;
    }
}

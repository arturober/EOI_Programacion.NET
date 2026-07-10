using System;

// ============================================================================
// ENTIDADES DEL JUEGO
// ============================================================================
//
// Este archivo contiene las clases que representan los objetos que se mueven
// dentro del tablero:
//
// - Entidad: clase base común.
// - Nave: objeto controlado por el jugador.
// - Alien: enemigo que avanza hacia la parte inferior.
// - Bala: proyectil que se desplaza hacia la parte superior.
//
// Estas clases no heredan de Node2D ni crean nodos de Godot. Son clases normales
// de C# que únicamente almacenan el estado lógico de cada objeto. La clase Juego
// se encarga después de convertir sus coordenadas de tablero en píxeles y de
// dibujarlas mediante las funciones gráficas de Godot.
//
// Esta separación es intencionada:
//
// 1. Las entidades se ocupan de su posición y movimiento.
// 2. Juego se ocupa de las reglas generales y de la representación gráfica.
// 3. La lógica no depende de imágenes, sprites ni coordenadas en píxeles.
//
// Gracias a ello se podría cambiar el aspecto gráfico sin modificar la forma en
// la que se mueven la nave, los alienígenas o las balas.
// ============================================================================

// Entidad es abstracta porque representa una idea general y no un objeto que
// deba crearse directamente. En el juego siempre se crean objetos más concretos:
// Nave, Alien o Bala.
abstract class Entidad
{
    // El constructor se declara protected para permitir que lo utilicen las
    // clases derivadas, pero impedir que se invoque desde cualquier otra clase.
    //
    // Recibe una posición lógica:
    // - x indica una columna del tablero.
    // - y indica una fila del tablero.
    //
    // Estas coordenadas no son píxeles. Por ejemplo, X = 3 e Y = 5 significan
    // columna 3 y fila 5, independientemente del tamaño visual de cada casilla.
    protected Entidad(int x, int y)
    {
        X = x;
        Y = y;
    }

    // X representa la columna ocupada por la entidad.
    //
    // El getter es público para que Juego pueda consultar la posición al dibujar
    // o detectar colisiones. El setter es protected para evitar que cualquier
    // parte del programa pueda cambiar libremente la posición. Solo Entidad y
    // sus clases derivadas pueden modificarla.
    public int X { get; protected set; }

    // Y representa la fila ocupada por la entidad.
    //
    // La fila 0 se encuentra en la parte superior del tablero. Al aumentar Y, la
    // entidad baja; al disminuir Y, la entidad sube.
    public int Y { get; protected set; }

    // Todas las entidades deben saber actualizarse, pero cada una lo hace de
    // manera diferente:
    //
    // - La nave responde a Izquierda y Derecha.
    // - El alien aumenta su fila.
    // - La bala reduce su fila.
    //
    // El método se declara abstracto para obligar a cada clase derivada a incluir
    // su propia implementación. Esto permite utilizar polimorfismo y mantener un
    // mismo contrato para todos los objetos del tablero.
    //
    // La acción y el ancho forman parte del contrato común. Algunas entidades no
    // necesitan ambos datos, pero se conservan para que todas compartan la misma
    // firma y la lógica sea parecida a la versión original de consola.
    public abstract void Actualizar(
        AccionJugador accion,
        int anchoTablero);

    // Comprueba si la posición lógica de la entidad pertenece al tablero.
    //
    // Una coordenada válida debe cumplir simultáneamente estas condiciones:
    //
    // - X no puede ser negativa.
    // - X debe ser menor que el ancho.
    // - Y no puede ser negativa.
    // - Y debe ser menor que el alto.
    //
    // Esta comprobación es especialmente útil para las balas. Cuando una bala
    // sale por la parte superior, su fila pasa temporalmente a ser -1 antes de
    // que Juego la elimine de la lista. No debe dibujarse durante ese instante.
    public bool EstaDentroDelTablero(int anchoTablero, int altoTablero)
    {
        return X >= 0
            && X < anchoTablero
            && Y >= 0
            && Y < altoTablero;
    }
}

// Nave representa al objeto controlado por el jugador.
//
// Hereda X, Y, EstaDentroDelTablero() y el contrato Actualizar() de Entidad.
class Nave : Entidad
{
    // El constructor de Nave no necesita trabajo adicional. Simplemente reenvía
    // las coordenadas recibidas al constructor de la clase base mediante base.
    public Nave(int x, int y)
        : base(x, y)
    {
    }

    // Actualiza la posición horizontal de la nave según la acción recibida.
    //
    // Disparar no cambia la posición. La creación de una bala es una regla global
    // del juego y, por tanto, se gestiona desde Juego, no desde la propia nave.
    public override void Actualizar(
        AccionJugador accion,
        int anchoTablero)
    {
        // Moverse a la izquierda equivale a reducir la columna en una unidad.
        if (accion == AccionJugador.Izquierda)
        {
            X--;
        }
        // Moverse a la derecha equivale a aumentar la columna en una unidad.
        else if (accion == AccionJugador.Derecha)
        {
            X++;
        }

        // Math.Clamp limita X al intervalo indicado.
        //
        // La primera columna válida es 0 y la última es anchoTablero - 1 porque
        // las posiciones comienzan a numerarse desde cero. Así se impide que la
        // nave abandone el tablero aunque el jugador siga pulsando una flecha en
        // uno de sus extremos.
        X = Math.Clamp(X, 0, anchoTablero - 1);
    }
}

// Alien representa a cada enemigo de una oleada.
class Alien : Entidad
{
    // Igual que en Nave, el constructor únicamente comunica la posición inicial
    // a la clase base.
    public Alien(int x, int y)
        : base(x, y)
    {
    }

    // Los alienígenas no responden directamente a las teclas del jugador. Cuando
    // Juego determina que deben avanzar, llama una vez a este método por cada fila
    // que deban recorrer.
    public override void Actualizar(
        AccionJugador accion,
        int anchoTablero)
    {
        // En una matriz o tablero, aumentar Y desplaza el objeto hacia abajo.
        //
        // Los parámetros accion y anchoTablero no son necesarios para este tipo
        // concreto de entidad, pero se mantienen porque forman parte del método
        // abstracto definido por Entidad.
        Y++;
    }
}

// Bala representa cada proyectil disparado por la nave.
class Bala : Entidad
{
    // La bala también comienza en una posición lógica del tablero.
    public Bala(int x, int y)
        : base(x, y)
    {
    }

    // Las balas se actualizan en todos los turnos, independientemente de si el
    // jugador se ha movido o ha disparado.
    public override void Actualizar(
        AccionJugador accion,
        int anchoTablero)
    {
        // Reducir Y desplaza la bala una fila hacia la parte superior.
        //
        // Igual que ocurre con Alien, los parámetros no son necesarios en esta
        // implementación, pero se conservan para respetar el contrato común.
        Y--;
    }
}

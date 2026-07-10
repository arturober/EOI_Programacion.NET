using Godot;

// ============================================================================
// ENTRADA DEL JUGADOR
// ============================================================================
//
// Este archivo separa las teclas físicas de las acciones lógicas del juego.
// Juego no necesita comprobar directamente si se ha pulsado una flecha, espacio
// o Escape. En su lugar recibe valores de AccionJugador con nombres claros.
//
// Esta separación ofrece varias ventajas:
//
// 1. La lógica principal es más legible.
// 2. Los controles están centralizados en un único archivo.
// 3. Cambiar una tecla no obliga a modificar Juego.cs.
// 4. Las teclas no válidas pueden ignorarse sin consumir ningún turno.
// ============================================================================

// La enumeración reúne todas las acciones admitidas durante una partida.
//
// Una enumeración resulta más segura y expresiva que usar números o cadenas. Por
// ejemplo, AccionJugador.Izquierda explica mejor la intención que un valor como
// 0 o un texto como "left".
enum AccionJugador
{
    // Desplaza la nave una columna hacia la izquierda.
    Izquierda,

    // Desplaza la nave una columna hacia la derecha.
    Derecha,

    // Crea una bala en la posición actual de la nave.
    Disparar,

    // Cierra el juego, tanto durante la partida como después de una derrota.
    Salir
}

// EntradaGodot no almacena estado, por lo que se declara static.
//
// No es necesario crear objetos de esta clase. Sus métodos se llaman directamente
// mediante EntradaGodot.IntentarLeerAccion() o EntradaGodot.EsTeclaReinicio().
static class EntradaGodot
{
    // Intenta convertir un evento recibido por Godot en una acción válida.
    //
    // El método devuelve:
    //
    // - true: se ha reconocido una tecla del juego y accion contiene el resultado.
    // - false: el evento no corresponde a una acción válida y debe ignorarse.
    //
    // El parámetro out permite devolver también la acción reconocida. Antes de
    // salir con true se asigna siempre un valor concreto a dicho parámetro.
    public static bool IntentarLeerAccion(
        InputEvent evento,
        out AccionJugador accion)
    {
        // Se asigna un valor inicial porque C# obliga a inicializar todos los
        // parámetros out antes de abandonar el método. Ese valor no se utiliza
        // cuando el método devuelve false.
        accion = default;

        // Godot utiliza una clase base llamada InputEvent para representar muchos
        // tipos de entrada: teclado, ratón, mando, pantalla táctil, etc.
        //
        // Aquí solo interesan eventos de tipo InputEventKey. Además:
        //
        // - tecla.Pressed debe ser true para reaccionar al presionar la tecla y no
        //   al soltarla.
        // - tecla.Echo debe ser false para ignorar la repetición automática que se
        //   produce al mantener una tecla pulsada.
        //
        // Sin comprobar Echo, mantener una flecha unos instantes podría consumir
        // numerosos turnos de forma involuntaria.
        if (evento is not InputEventKey tecla
            || !tecla.Pressed
            || tecla.Echo)
        {
            return false;
        }

        // Keycode contiene la tecla física que se ha pulsado. El switch traduce
        // esa tecla a una acción con significado para el resto del programa.
        switch (tecla.Keycode)
        {
            // Flecha izquierda: mover la nave hacia la izquierda.
            case Key.Left:
                accion = AccionJugador.Izquierda;
                return true;

            // Flecha derecha: mover la nave hacia la derecha.
            case Key.Right:
                accion = AccionJugador.Derecha;
                return true;

            // Barra espaciadora: disparar una nueva bala.
            case Key.Space:
                accion = AccionJugador.Disparar;
                return true;

            // Se ofrecen dos teclas para salir. X conserva el control del juego de
            // consola y Escape es una alternativa habitual en aplicaciones gráficas.
            case Key.X:
            case Key.Escape:
                accion = AccionJugador.Salir;
                return true;

            // Cualquier otra tecla se ignora. Como el método devuelve false, Juego
            // no procesa un turno y los enemigos no avanzan accidentalmente.
            default:
                return false;
        }
    }

    // Comprueba si el usuario solicita comenzar una partida nueva después de ser
    // derrotado.
    //
    // Esta acción se trata por separado porque R e Intro no son acciones de un
    // turno normal. Solo tienen sentido cuando la partida ya ha terminado.
    public static bool EsTeclaReinicio(InputEvent evento)
    {
        // La expresión devuelve true únicamente cuando:
        //
        // 1. El evento procede del teclado.
        // 2. La tecla se está presionando.
        // 3. No es una repetición automática.
        // 4. La tecla es R o Intro.
        return evento is InputEventKey tecla
            && tecla.Pressed
            && !tecla.Echo
            && (tecla.Keycode == Key.R || tecla.Keycode == Key.Enter);
    }
}

using System;

// ============================================================================
// ACCIONES DISPONIBLES DURANTE LA PARTIDA
// ============================================================================
//
// Esta enumeración representa las acciones que la clase Juego puede recibir.
// La lógica principal no trabaja directamente con teclas concretas, sino con
// acciones con un significado claro.
//
// Gracias a esta separación, Juego no necesita saber que la flecha izquierda
// mueve la nave, que la barra espaciadora dispara o que la tecla X permite salir.
// Si los controles cambiasen en el futuro, bastaría con modificar este archivo.
// ============================================================================
enum AccionJugador
{
    // Solicita desplazar la nave una posición hacia la izquierda.
    Izquierda,

    // Solicita desplazar la nave una posición hacia la derecha.
    Derecha,

    // Solicita crear una nueva bala en la posición actual de la nave.
    Disparar,

    // Solicita abandonar la partida y cerrar la aplicación.
    Salir
}

// ============================================================================
// POSIBLES RESULTADOS DE UNA PARTIDA
// ============================================================================
//
// Juego.Ejecutar devuelve uno de estos valores cuando termina.
//
// En esta versión no existe un resultado Victoria porque el juego no posee un
// último nivel. Cada oleada superada conduce automáticamente a otra más difícil.
// La partida solo finaliza cuando el jugador pierde o decide salir.
//
// Program utiliza este resultado para saber si debe preguntar si se desea jugar
// otra partida o si debe cerrar directamente la aplicación.
// ============================================================================
enum ResultadoPartida
{
    // Algún alienígena ha alcanzado la fila de la nave.
    Derrota,

    // El jugador ha pulsado X y ha solicitado salir voluntariamente.
    Salida
}

// ============================================================================
// LECTURA Y VALIDACIÓN DE LA ENTRADA DEL USUARIO
// ============================================================================
//
// Esta clase concentra toda la interacción con el teclado.
//
// Se declara como static porque no almacena estado propio y no es necesario
// crear objetos de tipo EntradaConsola. Sus métodos se utilizan directamente
// mediante el nombre de la clase:
//
// AccionJugador accion = EntradaConsola.LeerAccion();
// bool repetir = EntradaConsola.LeerConfirmacion("¿Volver a jugar?");
//
// Separar la entrada del resto del programa ofrece varias ventajas:
//
// 1. Juego se centra en las reglas y no en las teclas físicas.
// 2. Program no necesita repetir la validación de respuestas s/n.
// 3. Los controles pueden cambiarse desde un único lugar.
// 4. Las teclas incorrectas no consumen turnos de forma accidental.
// ============================================================================
static class EntradaConsola
{
    // Lee una tecla de control y la convierte en una acción del juego.
    //
    // El método no termina hasta que el usuario pulsa una tecla válida. Por ese
    // motivo utiliza un bucle while. Cuando se reconoce una tecla correcta, el
    // return devuelve la acción correspondiente y finaliza inmediatamente el
    // método.
    public static AccionJugador LeerAccion()
    {
        // Se muestran los controles antes de solicitar cada movimiento para que
        // el jugador pueda consultarlos en todo momento.
        MostrarControles();

        // El bucle se repite mientras no se haya pulsado una tecla admitida.
        while (true)
        {
            // Console.ReadKey espera una sola pulsación y no obliga a pulsar Intro.
            // Esto resulta más natural en un videojuego que utilizar ReadLine.
            //
            // El argumento intercept: true impide que la consola escriba por su
            // cuenta la tecla pulsada. Así evitamos que aparezcan caracteres o
            // espacios inesperados junto al tablero.
            //
            // Se consulta la propiedad Key, de tipo ConsoleKey, en lugar de
            // KeyChar. Las flechas del cursor son teclas especiales y no poseen
            // un carácter de texto normal que pueda compararse de forma fiable.
            ConsoleKey tecla = Console.ReadKey(intercept: true).Key;

            // Como la tecla no se ha mostrado automáticamente, se escribe un salto
            // de línea para que los mensajes posteriores comiencen debajo de
            // "Acción >" y no continúen en la misma línea.
            Console.WriteLine();

            // El switch traduce cada tecla física a una acción comprensible para
            // el resto del programa.
            switch (tecla)
            {
                // La flecha izquierda ordena mover la nave hacia la izquierda.
                case ConsoleKey.LeftArrow:
                    return AccionJugador.Izquierda;

                // La flecha derecha ordena mover la nave hacia la derecha.
                case ConsoleKey.RightArrow:
                    return AccionJugador.Derecha;

                // La barra espaciadora se utiliza para disparar.
                case ConsoleKey.Spacebar:
                    return AccionJugador.Disparar;

                // ConsoleKey.X reconoce la tecla X independientemente de que el
                // usuario la pulse con mayúscula o con minúscula.
                case ConsoleKey.X:
                    return AccionJugador.Salir;

                default:
                    // Cualquier otra tecla se considera inválida.
                    //
                    // No se devuelve ninguna acción y, por tanto, Juego no procesa
                    // ningún turno. Esto evita que los alienígenas avancen por un
                    // error accidental del jugador.
                    Console.WriteLine(
                        "Tecla no válida. Utiliza ←, →, espacio o X.");

                    // Se vuelve a mostrar el indicador de entrada sin repetir toda
                    // la línea de controles, que ya permanece visible encima.
                    Console.Write("Acción > ");
                    break;
            }
        }
    }

    // Solicita una respuesta afirmativa o negativa y devuelve un valor bool.
    //
    // Devuelve true cuando el usuario pulsa S y false cuando pulsa N.
    // Program utiliza este resultado para decidir si crea una partida nueva.
    //
    // El texto de la pregunta se recibe como parámetro para que el método pueda
    // reutilizarse con cualquier mensaje de confirmación.
    public static bool LeerConfirmacion(string mensaje)
    {
        // Se utiliza Write en lugar de WriteLine para que la respuesta se introduzca
        // visualmente a continuación de la pregunta.
        Console.Write(mensaje);

        // El bucle continúa hasta recibir una respuesta válida.
        while (true)
        {
            // Para una confirmación sí interesa consultar KeyChar, porque S y N sí
            // son caracteres normales.
            //
            // ToLowerInvariant convierte una posible S o N mayúscula en minúscula.
            // De esta forma se aceptan ambas variantes sin duplicar condiciones.
            char respuesta = char.ToLowerInvariant(
                Console.ReadKey(intercept: true).KeyChar);

            // Se muestra manualmente la respuesta porque intercept: true impide que
            // Console.ReadKey la escriba automáticamente.
            Console.WriteLine(respuesta);

            // La letra s representa una respuesta afirmativa.
            if (respuesta == 's')
            {
                return true;
            }

            // La letra n representa una respuesta negativa.
            if (respuesta == 'n')
            {
                return false;
            }

            // Una respuesta incorrecta no finaliza el método. Se informa al usuario
            // y el bucle vuelve a esperar otra tecla.
            Console.Write("Respuesta no válida. Pulsa S o N: ");
        }
    }

    // Muestra las teclas disponibles durante la partida.
    //
    // El método es private porque únicamente LeerAccion necesita utilizarlo.
    // Ninguna otra clase del proyecto debe encargarse de mostrar estos controles.
    private static void MostrarControles()
    {
        // Esta línea en blanco separa visualmente el tablero de las instrucciones.
        Console.WriteLine();

        // Todos los controles se muestran en una sola línea para ocupar poco
        // espacio vertical y poder consultarlos rápidamente.
        Console.WriteLine(
            "[←] Izquierda   [→] Derecha   [ESPACIO] Fuego   [X] Salir");

        // Write deja el cursor en la misma línea, indicando claramente que el
        // programa está esperando una pulsación del jugador.
        Console.Write("Acción > ");
    }
}

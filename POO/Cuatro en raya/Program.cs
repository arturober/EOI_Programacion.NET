// Esta clase contiene únicamente el punto de entrada de la aplicación.
//
// Se declara como static porque no necesitamos crear objetos de Program:
// su única responsabilidad es configurar la consola, iniciar el juego y
// esperar antes de cerrar la ventana.
static class Program
{
    private static void Main()
    {
        // El título permite identificar la aplicación en la barra de la ventana.
        Console.Title = "4 en raya";

        // Program no contiene las reglas del juego. Toda esa responsabilidad se
        // delega en la clase Juego para mantener cada clase centrada en una tarea.
        Juego juego = new Juego();
        juego.Iniciar();

        // Esperamos una tecla para que la ventana no se cierre inmediatamente
        // cuando el programa se ejecuta fuera de Visual Studio.
        Console.WriteLine();
        Console.WriteLine("Pulsa cualquier tecla para salir...");

        // intercept: true evita que la tecla pulsada aparezca escrita en pantalla.
        Console.ReadKey(intercept: true);
    }
}
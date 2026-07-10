using System;

// ============================================================================
// PUNTO DE ENTRADA DE LA APLICACIÓN
// ============================================================================
//
// Program se mantiene pequeño porque su única responsabilidad es controlar el
// ciclo general de la aplicación: iniciar partidas y preguntar si se desea jugar
// de nuevo. Todas las reglas concretas permanecen dentro de Juego.
// ============================================================================

static class Program
{
    private static void Main()
    {
        bool ejecutarOtraPartida = true;

        while (ejecutarOtraPartida)
        {
            // Se crea un objeto Juego nuevo en cada partida. Esto reinicia de
            // forma automática el nivel, la puntuación, las balas y los enemigos.
            Juego juego = new Juego();
            ResultadoPartida resultado = juego.Ejecutar();

            // La tecla x representa la intención de salir de la aplicación, no
            // simplemente de abandonar una ronda para comenzar otra.
            if (resultado == ResultadoPartida.Salida)
            {
                ejecutarOtraPartida = false;
                continue;
            }

            Console.WriteLine();

            ejecutarOtraPartida = EntradaConsola.LeerConfirmacion(
                "¿Quieres volver a jugar? (s/n): ");
        }

        Console.Clear();
        Console.WriteLine("Gracias por jugar a Space Invaders.");
    }
}

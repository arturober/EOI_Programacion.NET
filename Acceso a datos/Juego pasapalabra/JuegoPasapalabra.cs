using Microsoft.Data.Sqlite;

class JuegoPasapalabra
{
    private SqliteConnection conexion;

    public JuegoPasapalabra(SqliteConnection conexion)
    {
        this.conexion = conexion;
    }

    public void Jugar()
    {
        int idTema = 0; //SeleccionarTema();
        List<Pregunta> rosco = Pregunta.ObtenerRosco(conexion, idTema);
        Dictionary<char, string> estados = new Dictionary<char, string>();

        foreach (Pregunta pregunta in rosco)
        {
            estados[pregunta.Letra] = "Pendiente";
        }

        int indice = 0;
        int pendientes = rosco.Count;
        bool salir = false;

        while (pendientes > 0 && !salir)
        {
            Console.Clear();
            Console.WriteLine();
            Console.WriteLine("=== JUEGO PASAPALABRA ===");
            Console.WriteLine($"Preguntas pendientes: {pendientes}");
            Console.WriteLine();

            MostrarRosco(rosco, estados, rosco[indice].Letra);

            Pregunta preguntaActual = rosco[indice];
            Console.WriteLine();
            Console.WriteLine(preguntaActual.ObtenerEnunciado());
            Console.WriteLine();
            Console.Write("Introduzca su respuesta (o 'salir' para terminar): ");
            string respuesta = Console.ReadLine()?.Trim() ?? "";

            if (respuesta.Equals("salir", StringComparison.OrdinalIgnoreCase))
            {
                salir = true;
                continue;
            }

            if (TextoUtil.SonIguales(respuesta, preguntaActual.Respuesta))
            {
                estados[preguntaActual.Letra] = "correcta";
                pendientes--;
                Console.WriteLine("¡Respuesta correcta!");
            }
            else
            {
                estados[preguntaActual.Letra] = "incorrecta";
                pendientes--;
                Console.WriteLine($"Respuesta incorrecta. La respuesta correcta era: {preguntaActual.Respuesta}");
            }

            indice = (indice + 1) % rosco.Count;

            TextoUtil.Pausar();
        }

        MostrarRosco(rosco, estados);
    }

    private void MostrarRosco(List<Pregunta> rosco, Dictionary<char, string> estados, char? letraActual = null)
    {
        foreach(Pregunta pregunta in rosco)
        {
            char letra = pregunta.Letra;
            string estado = estados[letra];

            ConsoleColor color = ConsoleColor.DarkCyan;
            if (estado == "correcta")
            {
                color = ConsoleColor.Green;
            }
            else if (estado == "incorrecta")
            {
                color = ConsoleColor.Red;
            }
            else if (letraActual.HasValue && letraActual.Value == letra)
            {
                color = ConsoleColor.Yellow;
            }

            Console.ForegroundColor = color;
            Console.Write($"[{letra}] ");
            Console.ResetColor();
        }

        Console.WriteLine();
    }
}

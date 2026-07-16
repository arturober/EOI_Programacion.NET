using Microsoft.Data.Sqlite;
using System.Text;

class JuegoAhorcado
{
    private const int ErroresMaximos = 7;
    private readonly SqliteConnection conexion;

    public JuegoAhorcado(SqliteConnection conexion)
    {
        this.conexion = conexion;
    }

    public void Jugar()
    {
        int temaId = SeleccionarTema();
        Palabra? palabra = Palabra.ObtenerAleatoria(
            conexion,
            temaId);

        if (palabra == null)
        {
            Console.WriteLine(
                "No hay palabras disponibles para ese tema.");
            TextoUtil.Pausar();
        }
        else
        {
            JugarPartida(palabra);
        }
    }

    private int SeleccionarTema()
    {
        int temaIdSeleccionado = -1;

        while (temaIdSeleccionado == -1)
        {
            Console.Clear();
            EscribirEnColor(
                "ELIGE UN TEMA\n=============",
                ConsoleColor.Cyan);
            Console.WriteLine("0. Todos los temas");

            List<Tema> temas = Tema.Listar(conexion);

            foreach (Tema tema in temas)
            {
                int cantidad = tema.ContarPalabras(conexion);

                Console.WriteLine(
                    tema.Nombre +
                    " (Id: " + tema.Id + ")" +
                    " (" + cantidad + " palabras)");
            }

            Console.Write("Tema: ");
            string texto = Console.ReadLine() ?? "";

            bool esNumero = int.TryParse(
                texto,
                out int temaId);

            if (!esNumero || temaId < 0)
            {
                Console.WriteLine(
                    "Debes introducir un número válido.");
                TextoUtil.Pausar();
            }
            else if (temaId == 0)
            {
                temaIdSeleccionado = 0;
            }
            else
            {
                Tema? tema = Tema.BuscarPorId(
                    conexion,
                    temaId);

                int cantidad = Palabra.ContarPorTema(
                    conexion,
                    temaId);

                if (tema != null && cantidad > 0)
                {
                    temaIdSeleccionado = temaId;
                }
                else
                {
                    Console.WriteLine(
                        "Ese tema no existe o no contiene palabras.");
                    TextoUtil.Pausar();
                }
            }
        }

        return temaIdSeleccionado;
    }

    private void JugarPartida(Palabra palabra)
    {
        List<char> letrasUsadas = new List<char>();
        int errores = 0;
        bool pistaMostrada = false;
        bool partidaTerminada = false;

        while (!partidaTerminada)
        {
            MostrarEstado(
                palabra,
                letrasUsadas,
                errores,
                pistaMostrada);

            Console.Write(
                "Escribe una letra, una palabra, PISTA o SALIR: ");

            string entrada =
                (Console.ReadLine() ?? "").Trim();

            if (entrada.Equals(
                "salir",
                StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine(
                    "La palabra era: " + palabra.Texto);
                partidaTerminada = true;
            }
            else if (entrada.Equals(
                "pista",
                StringComparison.OrdinalIgnoreCase))
            {
                pistaMostrada = true;
            }
            else if (entrada != "")
            {
                string entradaNormalizada =
                    TextoUtil.NormalizarParaComparar(entrada);

                if (entradaNormalizada.Length == 1 &&
                    char.IsLetter(entradaNormalizada[0]))
                {
                    char letra = entradaNormalizada[0];

                    if (letrasUsadas.Contains(letra))
                    {
                        Console.WriteLine(
                            "Ya habías probado esa letra.");
                        TextoUtil.Pausar();
                    }
                    else
                    {
                        letrasUsadas.Add(letra);

                        if (!ContieneLetra(
                            palabra.Texto,
                            letra))
                        {
                            errores++;
                        }
                    }
                }
                else
                {
                    // También permitimos intentar resolver la palabra completa.
                    bool palabraCorrecta = TextoUtil.SonIguales(
                        entrada,
                        palabra.Texto);

                    if (palabraCorrecta)
                    {
                        MostrarVictoria(palabra, errores);
                        partidaTerminada = true;
                    }
                    else
                    {
                        errores++;
                    }
                }

                if (!partidaTerminada)
                {
                    bool completada = EstaCompleta(
                        palabra.Texto,
                        letrasUsadas);

                    if (completada)
                    {
                        MostrarVictoria(palabra, errores);
                        partidaTerminada = true;
                    }
                    else if (errores >= ErroresMaximos)
                    {
                        MostrarDerrota(palabra);
                        partidaTerminada = true;
                    }
                }
            }
        }

        TextoUtil.Pausar();
    }

    private void MostrarEstado(
        Palabra palabra,
        List<char> letrasUsadas,
        int errores,
        bool pistaMostrada)
    {
        Console.Clear();
        EscribirEnColor(
            "JUEGO DEL AHORCADO\n==================",
            ConsoleColor.Cyan);

        EscribirEnColor(
            "Tema: " + palabra.Tema.Nombre,
            ConsoleColor.DarkCyan);
        Console.WriteLine();

        DibujarAhorcado(errores);

        Console.WriteLine();
        EscribirEnColor(
            ObtenerPalabraOculta(
                palabra.Texto,
                letrasUsadas),
            ConsoleColor.Cyan);
        Console.WriteLine();

        ConsoleColor colorErrores = ConsoleColor.Yellow;

        if (errores >= 5)
        {
            colorErrores = ConsoleColor.Red;
        }

        EscribirEnColor(
            "Errores: " + errores +
            " de " + ErroresMaximos,
            colorErrores);

        string letras = "ninguna";

        if (letrasUsadas.Count > 0)
        {
            letras = string.Join(", ", letrasUsadas);
        }

        EscribirEnColor(
            "Letras usadas: " + letras,
            ConsoleColor.DarkYellow);

        if (pistaMostrada)
        {
            EscribirEnColor(
                "Pista: " + palabra.Pista,
                ConsoleColor.Magenta);
        }
    }

    private string ObtenerPalabraOculta(
        string palabra,
        List<char> letrasUsadas)
    {
        StringBuilder resultado = new StringBuilder();

        foreach (char caracter in palabra)
        {
            if (!char.IsLetter(caracter))
            {
                // Los espacios y los guiones se muestran desde el principio.
                resultado.Append(caracter);
            }
            else
            {
                char letraNormalizada =
                    TextoUtil.NormalizarCaracter(caracter);

                if (letrasUsadas.Contains(letraNormalizada))
                {
                    resultado.Append(caracter);
                }
                else
                {
                    resultado.Append('_');
                }
            }

            resultado.Append(' ');
        }

        return resultado.ToString();
    }

    private bool ContieneLetra(
        string palabra,
        char letra)
    {
        bool encontrada = false;

        foreach (char caracter in palabra)
        {
            if (char.IsLetter(caracter) &&
                TextoUtil.NormalizarCaracter(caracter) == letra)
            {
                encontrada = true;
            }
        }

        return encontrada;
    }

    private bool EstaCompleta(
        string palabra,
        List<char> letrasUsadas)
    {
        bool completa = true;

        foreach (char caracter in palabra)
        {
            if (char.IsLetter(caracter))
            {
                char letra =
                    TextoUtil.NormalizarCaracter(caracter);

                if (!letrasUsadas.Contains(letra))
                {
                    completa = false;
                }
            }
        }

        return completa;
    }

    private void MostrarVictoria(
        Palabra palabra,
        int errores)
    {
        Console.Clear();
        DibujarAhorcado(errores);
        Console.WriteLine();

        EscribirEnColor(
            "¡ENHORABUENA! HAS GANADO.",
            ConsoleColor.Green);
        EscribirEnColor(
            "La palabra era: " + palabra.Texto,
            ConsoleColor.Cyan);
    }

    private void MostrarDerrota(Palabra palabra)
    {
        Console.Clear();
        DibujarAhorcado(ErroresMaximos);
        Console.WriteLine();

        EscribirEnColor(
            "Has perdido esta partida.",
            ConsoleColor.Red);
        EscribirEnColor(
            "La palabra era: " + palabra.Texto,
            ConsoleColor.Cyan);
        EscribirEnColor(
            "Pista: " + palabra.Pista,
            ConsoleColor.Magenta);
    }

    private void DibujarAhorcado(int errores)
    {
        string[] dibujos =
        {
            @"
  +---+
      |
      |
      |
     ===",
            @"
  +---+
  |   |
      |
      |
     ===",
            @"
  +---+
  |   |
  O   |
      |
     ===",
            @"
  +---+
  |   |
  O   |
  |   |
     ===",
            @"
  +---+
  |   |
  O   |
 /|   |
     ===",
            @"
  +---+
  |   |
  O   |
 /|\  |
     ===",
            @"
  +---+
  |   |
  O   |
 /|\  |
 /    ===",
            @"
  +---+
  |   |
  O   |
 /|\  |
 / \  ==="
        };

        // El dibujo cambia de color conforme aumenta el peligro.
        ConsoleColor color = ConsoleColor.Green;

        if (errores >= 3)
        {
            color = ConsoleColor.Yellow;
        }

        if (errores >= 5)
        {
            color = ConsoleColor.Red;
        }

        EscribirEnColor(dibujos[errores], color);
    }

    private void EscribirEnColor(
        string texto,
        ConsoleColor color)
    {
        ConsoleColor colorAnterior = Console.ForegroundColor;
        Console.ForegroundColor = color;
        Console.WriteLine(texto);
        Console.ForegroundColor = colorAnterior;
    }
}

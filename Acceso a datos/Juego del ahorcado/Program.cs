using Microsoft.Data.Sqlite;
using System.Text;

class Program
{
    static void Main()
    {
        // Permitimos mostrar correctamente tildes, eñes y otros caracteres.
        Console.OutputEncoding = Encoding.UTF8;
        Console.InputEncoding = Encoding.UTF8;

        SqliteConnection conexion = BaseDatos.CrearConexion();

        using (conexion)
        {
            conexion.Open();
            BaseDatos.CrearTablas(conexion);

            // Estas clases se ocupan solamente de los menús y del juego.
            // Las consultas SQL están dentro de Tema y Palabra.
            GestorPalabras gestorPalabras =
                new GestorPalabras(conexion);
            GestorTemas gestorTemas =
                new GestorTemas(conexion);
            JuegoAhorcado juego =
                new JuegoAhorcado(conexion);

            bool salir = false;

            while (!salir)
            {
                MostrarMenuPrincipal();
                string opcion = Console.ReadLine() ?? "";

                switch (opcion)
                {
                    case "1":
                        juego.Jugar();
                        break;

                    case "2":
                        gestorPalabras.MostrarMenu();
                        break;

                    case "3":
                        gestorTemas.MostrarMenu();
                        break;

                    case "4":
                        MostrarTemas(conexion);
                        break;

                    case "0":
                        salir = true;
                        break;

                    default:
                        Console.WriteLine(
                            "Opción no válida. Inténtalo de nuevo.");
                        TextoUtil.Pausar();
                        break;
                }
            }
        }

        // Dejamos el color de la consola como estaba al terminar.
        Console.ResetColor();
    }

    static void MostrarMenuPrincipal()
    {
        Console.Clear();

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("====================================");
        Console.WriteLine("          JUEGO DEL AHORCADO");
        Console.WriteLine("====================================");
        Console.ResetColor();

        Console.WriteLine("1. Jugar");
        Console.WriteLine("2. Gestionar palabras (CRUD)");
        Console.WriteLine("3. Gestionar temas (CRUD)");
        Console.WriteLine("4. Mostrar temas disponibles");
        Console.WriteLine("0. Salir");
        Console.WriteLine("------------------------------------");
        Console.Write("Selecciona una opción: ");
    }

    static void MostrarTemas(SqliteConnection conexion)
    {
        Console.Clear();
        Console.WriteLine("TEMAS DISPONIBLES");
        Console.WriteLine("=================");

        List<Tema> temas = Tema.Listar(conexion);

        foreach (Tema tema in temas)
        {
            int cantidad = tema.ContarPalabras(conexion);

            Console.WriteLine(
                "- " + tema.Nombre +
                " (" + cantidad + " palabras)" +
                " (Id: " + tema.Id + ")");
            Console.WriteLine("  " + tema.Descripcion);
            Console.WriteLine();
        }

        TextoUtil.Pausar();
    }
}

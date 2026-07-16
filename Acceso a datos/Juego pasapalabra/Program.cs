using Microsoft.Data.Sqlite;
using System.Text;

class Program
{

    static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.InputEncoding = Encoding.UTF8;

        using (SqliteConnection conexion = BaseDatos.CrearConexion())
        {
            conexion.Open();

            GestorTemas gestorTemas = new GestorTemas(conexion);
            //GestorPalabras gestorPalabras = new GestorPalabras(conexion);
            JuegoPasapalabra juego = new JuegoPasapalabra(conexion);

            bool salir = false;
            while (!salir)
            {
                Console.Clear();
                Console.WriteLine();
                Console.WriteLine("=== MENÚ PRINCIPAL ===");
                Console.WriteLine("1. Jugar");
                Console.WriteLine("2. Gestionar temas");
                Console.WriteLine("3. Gestionar palabras");
                Console.WriteLine("0. Salir");
                Console.Write("Seleccione una opción: ");

                string opcion = Console.ReadLine() ?? " ";

                switch (opcion)
                {
                    case "1":
                        juego.Jugar();
                        break;

                    case "2":
                        gestorTemas.MostrarMenu();
                        break;

                    case "3":
                        //gestorPalabras.MostrarMenu();
                        break;

                    case "0":
                        salir = true;
                        break;

                    default:
                        Console.WriteLine("Opción no válida. Intente nuevamente.");
                        TextoUtil.Pausar();
                        break;
                }
            }
        }
    }
}
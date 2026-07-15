using Microsoft.Data.Sqlite;

class Program
{
    static void Main(string[] args)
    {
        using (SqliteConnection conexion = BaseDatos.CrearConexion())
        {
            conexion.Open();

            GestorCategorias gestorCategorias = new GestorCategorias(conexion);
            GestorTareas gestorTareas = new GestorTareas(conexion);

            bool salir = false;
            while (!salir)
            {
                Console.Clear();
                Console.WriteLine();
                Console.WriteLine("=== MENÚ PRINCIPAL ===");
                Console.WriteLine("1. Gestionar categorías");
                Console.WriteLine("2. Gestionar tareas");
                Console.WriteLine("0. Salir");
                Console.Write("Seleccione una opción: ");

                string opcion = Console.ReadLine() ?? "";

                switch (opcion)
                {
                    case "1":
                        gestorCategorias.MostrarMenu();
                        break;

                    case "2":
                        gestorTareas.MostrarMenu();
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
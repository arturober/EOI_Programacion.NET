using Microsoft.Data.Sqlite;

class ProgramaAgenda
{
    static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        string cadenaConexion = "Data Source=agenda.db";
        SqliteConnection conexion = new SqliteConnection(cadenaConexion);

        using (conexion)
        {
            conexion.Open();

            CrearTabla(conexion);

            bool salir = false;
            while (!salir)
            {
                MostrarMenu();
                string opcion = Console.ReadLine() ?? "";

                switch (opcion)
                {
                    case "1":
                        CrearPersona(conexion);
                        break;

                    case "0":
                        salir = true;
                        break;

                    default:
                        Console.WriteLine("Opción no válida. Intente nuevamente.");
                        break;
                }

                if (!salir)
                {
                    Console.WriteLine("Presione cualquier tecla para continuar...");
                    Console.ReadKey();
                }
            }
        }
    }

    static void CrearTabla(SqliteConnection conexion)
    {
        string sqlCrearTabla = 
                "CREATE TABLE IF NOT EXISTS personas (" +
                "id INTEGER PRIMARY KEY AUTOINCREMENT, " +
                "nombre TEXT NOT NULL, " +
                "telefono TEXT NOT NULL)";

        using (SqliteCommand comando = new SqliteCommand(sqlCrearTabla, conexion))
        {
            comando.ExecuteNonQuery();
        }
    }

    static void MostrarMenu()
    {
        Console.Clear();
        Console.WriteLine();
        Console.WriteLine("Agenda de Contactos");
        Console.WriteLine("------------------");
        Console.WriteLine("Seleccione una opción:");
        Console.WriteLine("1. Añadir persona");
        Console.WriteLine("0. Salir");
        Console.WriteLine("------------------");
    }

    static string LeerTextoObligatorio(string mensaje)
    {
        while(true)
        {
            Console.Write(mensaje);
            string texto = (Console.ReadLine() ?? "").Trim();

            if (texto != "")
            {
                return texto;
            }

            Console.WriteLine("El campo no puede estar vacío. Inténtalo de nuevo.");
        }
    }

    static void CrearPersona(SqliteConnection conexion)
    {
        string nombre = LeerTextoObligatorio("Introduce el nombre: ");
        string telefono = LeerTextoObligatorio("Introduce el teléfono: ");

        string sql = "INSERT INTO personas (nombre, telefono) VALUES (@nombre, @telefono)";

        using (SqliteCommand comando = new SqliteCommand(sql, conexion))
        {
            comando.Parameters.AddWithValue("@nombre", nombre);
            comando.Parameters.AddWithValue("@telefono", telefono);
            int filas = comando.ExecuteNonQuery();

            if (filas == 1)
            {
                Console.WriteLine("Persona añadida con éxito.");
            }
            else
            {
                Console.WriteLine("Error al añadir la persona.");
            }
        }
    }
}
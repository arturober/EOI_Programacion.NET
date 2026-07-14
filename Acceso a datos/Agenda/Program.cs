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

                    case "2":
                        MostrarPersonas(conexion);
                        break;

                    case "3":
                        ModificarPersona(conexion);
                        break;

                    case "4":
                        EliminarPersona(conexion);
                        break;

                    case "5":
                        BuscarPersona(conexion);
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
        Console.WriteLine("2. Mostrar personas");
        Console.WriteLine("3. Modificar persona");
        Console.WriteLine("4. Eliminar persona");
        Console.WriteLine("5. Buscar persona");
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

    static bool MostrarPersonas(SqliteConnection conexion)
    {
        string sql = "SELECT * FROM personas ORDER BY id";

        using (SqliteCommand comando = new SqliteCommand(sql, conexion))
        {
            using (SqliteDataReader lector = comando.ExecuteReader())
            {
                if (!lector.HasRows)
                {
                    Console.WriteLine("No hay personas en la agenda.");
                    return false;
                }

                Console.WriteLine("Personas en la agenda:");
                while (lector.Read())
                {
                    Console.WriteLine($"{lector["id"]} - {lector["nombre"]} ({lector["telefono"]})");
                }
            }
        }

        return true;
    }

    static int LeerNumeroEntero(string mensaje)
    {
        while (true)
        {
            Console.Write(mensaje);
            string input = Console.ReadLine() ?? "";
            bool esNumero = int.TryParse(input, out int numero);
            if (esNumero && numero > 0)
            {
                return numero;
            }
            Console.WriteLine("Número no válido. Inténtalo de nuevo.");
        }
    }

    static void ModificarPersona(SqliteConnection conexion)
    {
        bool hayPersonas = MostrarPersonas(conexion);

        if (!hayPersonas)
        {
            return;
        }

        int id = LeerNumeroEntero("ID de la persona que quieres modificar: ");
        string nombre = LeerTextoObligatorio("Introduce el nuevo nombre: ");
        string telefono = LeerTextoObligatorio("Introduce el nuevo teléfono: ");

        string sql = "UPDATE personas SET nombre = @nombre, telefono = @telefono WHERE id = @id";

        using (SqliteCommand comando = new SqliteCommand(sql, conexion))
        {
            comando.Parameters.AddWithValue("@nombre", nombre);
            comando.Parameters.AddWithValue("@telefono", telefono);
            comando.Parameters.AddWithValue("@id", id);
            int filas = comando.ExecuteNonQuery();

            if (filas == 1)
            {
                Console.WriteLine("Persona modificada con éxito.");
            }
            else
            {
                Console.WriteLine("Error al modificar la persona. Asegúrate de que el ID es correcto.");
            }
        }
    }

    static void EliminarPersona(SqliteConnection conexion)
    {
        bool hayPersonas = MostrarPersonas(conexion);

        if (!hayPersonas)
        {
            return;
        }

        int id = LeerNumeroEntero("ID de la persona que quieres eliminar: ");

        Console.Write($"¿Seguro que quieres eliminar la persona con el ID {id}? (s/n): ");
        string respuesta = (Console.ReadLine()??"").Trim().ToLower();
        if (respuesta != "s")
        {
            Console.WriteLine("Operación cancelada.");
            return;
        }

        string sql = "DELETE FROM personas WHERE id = @id";

        using (SqliteCommand comando = new SqliteCommand(sql, conexion))
        {
            comando.Parameters.AddWithValue("@id", id);
            int filas = comando.ExecuteNonQuery();

            if (filas == 1)
            {
                Console.WriteLine("Persona eliminada con éxito.");
            }
            else
            {
                Console.WriteLine("Error al eliminar la persona. Asegúrate de que el ID es correcto.");
            }
        }
    }

    static void BuscarPersona(SqliteConnection conexion)
    {
        string nombre = LeerTextoObligatorio("Introduce el nombre a buscar: ");

        string sql = "SELECT * FROM personas WHERE nombre LIKE @nombre ORDER BY id";

        using (SqliteCommand comando = new SqliteCommand(sql, conexion))
        {
            comando.Parameters.AddWithValue("@nombre", "%" + nombre + "%");

            using (SqliteDataReader lector = comando.ExecuteReader())
            {
                if (!lector.HasRows)
                {
                    Console.WriteLine("No se encontraron personas con ese nombre.");
                    return;
                }

                Console.WriteLine("Resultados de la búsqueda:");
                while (lector.Read())
                {
                    Console.WriteLine($"{lector["id"]} - {lector["nombre"]} ({lector["telefono"]})");
                }
            }
        }
    }
}
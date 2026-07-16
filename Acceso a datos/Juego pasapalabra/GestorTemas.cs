using Microsoft.Data.Sqlite;

class GestorTemas
{
    private SqliteConnection conexion;

    public GestorTemas(SqliteConnection conexion)
    {
        this.conexion = conexion;
    }

    public void MostrarMenu()
    {
        bool volver = false;
        while (!volver)
        {
            Console.Clear();
            Console.WriteLine();
            Console.WriteLine("=== GESTOR DE TEMAS ===");
            Console.WriteLine("1. Crear tema");
            Console.WriteLine("2. Mostrar temas");
            Console.WriteLine("3. Modificar tema");
            Console.WriteLine("4. Eliminar tema");
            Console.WriteLine("0. Volver al menú principal");
            Console.Write("Seleccione una opción: ");

            string opcion = Console.ReadLine() ?? "";

            switch (opcion)
            {
                case "1":
                    InsertarTema();
                    break;

                case "2":
                    MostrarTemas();
                    break;

                case "3":
                    ModificarTema();
                    break;

                case "4":
                    EliminarTema();
                    break;

                case "0":
                    volver = true;
                    break;

                default:
                    Console.WriteLine("Opción no válida. Inténtalo de nuevo.");
                    break;
            }

            if (!volver)
            {
                TextoUtil.Pausar();
            }
        }
    }

    private void MostrarTemas()
    {
        Console.Clear();
        Console.WriteLine("=== LISTA DE TEMAS ===");
        List<Tema> temas = Tema.Listar(conexion);
        MostrarLista(temas);
    }

    private void MostrarLista(List<Tema> temas)
    {
        if (temas.Count == 0)
        {
            Console.WriteLine("No hay temas disponibles.");
            return;
        }

        foreach (Tema tema in temas)
        {
            Console.WriteLine(tema.ToString());
        }

        Console.WriteLine();
    }

    private void InsertarTema()
    {
        Console.Clear();
        Console.WriteLine("=== INSERTAR TEMA ===");
        Console.WriteLine("=====================");

        string nombre = TextoUtil.LeerTextoObligatorio("Introduce el nombre del tema: ");

        if (Tema.Existe(conexion, nombre))
        {
            Console.WriteLine("Ya existe un tema con ese nombre. No se puede insertar.");
        }
        else
        {
            string descripcion = TextoUtil.LeerTextoObligatorio("Introduce la descripción del tema: ");

            Tema nuevoTema = new Tema(nombre, descripcion);
            if (nuevoTema.Insertar(conexion))
            {
                Console.WriteLine("Tema insertado correctamente.");
            }
            else
            {
                Console.WriteLine("Error al insertar el tema.");
            }
        }
    }

    private void EliminarTema()
    {
        Console.Clear();
        Console.WriteLine("=== ELIMINAR TEMA ===");
        Console.WriteLine("=====================");

        List<Tema> temas = Tema.Listar(conexion);
        MostrarLista(temas);

        if (temas.Count == 0)
        {
            Console.WriteLine("No hay temas disponibles.");
            return;
        }

        int id = TextoUtil.LeerEnteroPositivo("Introduce el ID del tema a eliminar: ");

        Tema? tema = Tema.BuscarPorId(conexion, id);
        if (tema == null)
        {
            Console.WriteLine("No se encontró un tema con ese ID.");
            return;
        }

        bool confirmacion = TextoUtil.Confirmar($"¿Estás seguro de que deseas eliminar el tema '{tema.Nombre}'?");
        if (confirmacion)
        {
            if (tema.Borrar(conexion))
            {
                Console.WriteLine("Tema eliminado correctamente.");
            }
            else
            {
                Console.WriteLine("Error al eliminar el tema.");
            }
        }
        else
        {
            Console.WriteLine("Operación cancelada. El tema no se ha eliminado.");
        }
    }

    private void ModificarTema()
    {
        Console.Clear();
        Console.WriteLine("=== MODIFICAR TEMA ===");
        Console.WriteLine("======================");

        List<Tema> temas = Tema.Listar(conexion);
        MostrarLista(temas);

        if (temas.Count == 0)
        {
            Console.WriteLine("No hay temas disponibles.");
            return;
        }

        int id = TextoUtil.LeerEnteroPositivo("Introduce el ID del tema a modificar: ");

        Tema? tema = Tema.BuscarPorId(conexion, id);
        if (tema == null)
        {
            Console.WriteLine("No se encontró un tema con ese ID.");
            return;
        }

        Console.WriteLine("Nombre actual: " + tema.Nombre);
        Console.Write("Nuevo nombre (Enter para conservar): ");
        string nuevoNombre = Console.ReadLine()?.Trim() ?? tema.Nombre;

        if (nuevoNombre != tema.Nombre && Tema.Existe(conexion, nuevoNombre))
        {
            Console.WriteLine("Ya existe un tema con ese nombre. No se puede modificar.");
            return;
        }

        Console.WriteLine("Descripción actual: " + tema.Descripcion);
        Console.Write("Nueva descripción (Enter para conservar): ");
        string nuevaDescripcion = Console.ReadLine()?.Trim() ?? tema.Descripcion;

        tema.Nombre = nuevoNombre;
        tema.Descripcion = nuevaDescripcion;

        if (tema.Actualizar(conexion))
        {
            Console.WriteLine("Tema modificado correctamente.");
        }
        else
        {
            Console.WriteLine("Error al modificar el tema.");
        }
    }
}
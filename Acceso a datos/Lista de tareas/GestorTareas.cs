using System.Reflection;
using Microsoft.Data.Sqlite;

class GestorTareas
{
    private SqliteConnection conexion;

    public GestorTareas(SqliteConnection conexion)
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
            Console.WriteLine("=== GESTOR DE TAREAS ===");
            Console.WriteLine("1. Crear tarea");
            Console.WriteLine("2. Mostrar tareas");
            Console.WriteLine("3. Modificar tarea");
            Console.WriteLine("4. Eliminar tarea");
            Console.WriteLine("0. Volver al menú principal");
            Console.Write("Seleccione una opción: ");

            string opcion = Console.ReadLine() ?? "";

            switch (opcion)
            {
                case "1":
                    InsertarTarea();
                    break;

                case "2":
                    //MostrarTareas();
                    break;

                case "3":
                    //ModificarTarea();
                    break;

                case "4":
                    EliminarTarea();
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

    private void InsertarTarea()
    {
        Console.WriteLine();
        Console.WriteLine("=== CREAR NUEVA TAREA ===");

        Categoria? categoria = SeleccionarCategoria();
        if (categoria == null)
        {
            Console.WriteLine("No se seleccionó una categoría válida.");
            return;
        }

        string titulo = TextoUtil.LeerTextoObligatorio("Introduce el título de la tarea: ");
        string descripcion = TextoUtil.LeerTextoObligatorio("Introduce la descripción de la tarea: ");

        Tarea tarea = new Tarea(titulo, descripcion, false, categoria);
        if (tarea.Insertar(conexion)) 
        {
            Console.WriteLine("Tarea creada con éxito.");
        }
        else
        {
            Console.WriteLine("Error al crear la tarea.");
        }
    }

    private Categoria? SeleccionarCategoria()
    {
        List<Categoria> categorias = Categoria.Listar(conexion);

        if (categorias.Count == 0)
        {
            return null;
        }

        Categoria? categoriaSeleccionada = null;

        while (categoriaSeleccionada == null)
        {
            Console.WriteLine();
            Console.WriteLine("=== SELECCIONAR CATEGORÍA ===");

            foreach (Categoria categoria in categorias)
            {
                Console.WriteLine($"{categoria.Id}. {categoria.Nombre}");
            }

            int idCategoria = TextoUtil.LeerEnteroPositivo("Introduce el ID de la categoría: ");
            categoriaSeleccionada = Categoria.BuscarPorId(conexion, idCategoria);

            if (categoriaSeleccionada == null)
            {
                Console.WriteLine("No se encontró una categoría con ese ID. Inténtalo de nuevo.");
            }
        }

        return categoriaSeleccionada;
    }

    private void EliminarTarea()
    {
        Console.Clear();
        Console.WriteLine();
        Console.WriteLine("=== ELIMINAR TAREA ===");

        List<Tarea> tareas = Tarea.Listar(conexion);
        MostrarLista(tareas);

        if (tareas.Count == 0)
        {
            return;
        }

        int idTarea = TextoUtil.LeerEnteroPositivo("Introduce el ID de la tarea a eliminar: ");
        Tarea? tareaAEliminar = Tarea.BuscarPorId(conexion, idTarea);

        if (tareaAEliminar == null)
        {
            Console.WriteLine("No se encontró una tarea con ese ID.");
            return;
        }

        bool confirmacion = TextoUtil.Confirmar("¿Estás seguro de que deseas eliminar la tarea?");

        if (confirmacion)
        {
            if (tareaAEliminar.Borrar(conexion))
            {
                Console.WriteLine("Tarea eliminada con éxito.");
            }
            else
            {
                Console.WriteLine("Error al eliminar la tarea.");
            }
        }
        else
        {
            Console.WriteLine("Operación cancelada. La tarea no se ha eliminado.");
        }
    }

    private void MostrarLista(List<Tarea> tareas)
    {
        if (tareas.Count == 0) {
            Console.WriteLine("No hay tareas disponibles.");
            return;
        }

        Console.WriteLine("Lista de tareas:");

        foreach (Tarea tarea in tareas)
        {
            Console.WriteLine(tarea.ToString());
        }
    }
}
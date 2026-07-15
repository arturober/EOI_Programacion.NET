using Microsoft.Data.Sqlite;

class GestorCategorias
{
    private SqliteConnection conexion;

    public GestorCategorias(SqliteConnection conexion)
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
            Console.WriteLine("=== GESTOR DE CATEGORÍAS ===");
            Console.WriteLine("1. Crear categoría");
            Console.WriteLine("2. Mostrar categorías");
            Console.WriteLine("3. Modificar categoría");
            Console.WriteLine("4. Eliminar categoría");
            Console.WriteLine("0. Volver al menú principal");
            Console.Write("Seleccione una opción: ");

            string opcion = Console.ReadLine() ?? "";

            switch (opcion)
            {
                case "1":
                    InsertarCategoria();
                    break;

                case "2":
                    MostrarCategorias();
                    break;

                case "3":
                    ModificarCategoria();
                    break;

                case "4":
                    EliminarCategoria();
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

    private void InsertarCategoria()
    {
        Console.Clear();
        Console.WriteLine("=== INSERTAR CATEGORÍA ===");

        string nombre = TextoUtil.LeerTextoObligatorio("Introduce el nombre de la categoría: ");

        if (Categoria.Existe(conexion, nombre))
        {
            Console.WriteLine("Ya existe una categoría con ese nombre. No se puede insertar.");
        }
        else {
            string descripcion = TextoUtil.LeerTextoObligatorio("Introduce la descripción de la categoría: ");

            Categoria nuevaCategoria = new Categoria(nombre, descripcion);
            if (nuevaCategoria.Insertar(conexion))
            {
                Console.WriteLine("Categoría insertada correctamente.");
            }
            else
            {
                Console.WriteLine("Error al insertar la categoría.");
            }
        }
    }

    private void EliminarCategoria()
    {
        Console.Clear();
        Console.WriteLine("=== ELIMINAR CATEGORÍA ===");
        Console.WriteLine("==========================");

        List<Categoria> categorias = Categoria.Listar(conexion);
        MostrarLista(categorias);

        if (categorias.Count == 0)
        {
            Console.WriteLine("No hay categorías disponibles.");
            return;
        }

        int idCategoria = TextoUtil.LeerEnteroPositivo("Introduce el ID de la categoría a eliminar: ");

        Categoria? categoria = Categoria.BuscarPorId(conexion, idCategoria);

        if (categoria == null)
        {
            Console.WriteLine("No se encontró una categoría con ese ID.");
            return;
        }

        Console.Write($"¿Estás seguro de que deseas eliminar la categoría '{categoria.Nombre}'?");
        if (TextoUtil.Confirmar(""))
        {
            if (categoria.Borrar(conexion))
            {
                Console.WriteLine("Categoría eliminada correctamente.");
            }
            else
            {
                Console.WriteLine("Error al eliminar la categoría.");
            }
        }
        else
        {
            Console.WriteLine("Operación cancelada. La categoría no se ha eliminado.");
        }
    }

    private void MostrarLista(List<Categoria> categorias)
    {
        if (categorias.Count == 0) return;

        Console.WriteLine("Lista de categorías:");

        foreach (Categoria categoria in categorias)
        {
            Console.WriteLine(categoria.ToString());
        }
    }

    private void MostrarCategorias()
    {
        Console.Clear();
        Console.WriteLine("=== LISTA DE CATEGORÍAS ===");
        Console.WriteLine("============================");

        List<Categoria> categorias = Categoria.Listar(conexion);
        MostrarLista(categorias);

        if (categorias.Count == 0)
        {
            Console.WriteLine("No hay categorías disponibles.");
        }
    }

    private void ModificarCategoria()
    {
        Console.Clear();
        Console.WriteLine("=== MODIFICAR CATEGORÍA ===");
        Console.WriteLine("============================");

        List<Categoria> categorias = Categoria.Listar(conexion);
        MostrarLista(categorias);

        if (categorias.Count == 0)
        {
            Console.WriteLine("No hay categorías disponibles.");
            return;
        }

        int idCategoria = TextoUtil.LeerEnteroPositivo("Introduce el ID de la categoría a modificar: ");

        Categoria? categoria = Categoria.BuscarPorId(conexion, idCategoria);

        if (categoria == null)
        {
            Console.WriteLine("No se encontró una categoría con ese ID.");
            return;
        }

        Console.WriteLine("Nombre actual: " + categoria.Nombre);
        Console.Write("Nuevo nombre (Enter para conservar): ");
        string nuevoNombre = Console.ReadLine()?.Trim() ?? categoria.Nombre;

        Console.WriteLine("Descripción actual: " + categoria.Descripcion);
        Console.Write("Nueva descripción (Enter para conservar): ");
        string nuevaDescripcion = Console.ReadLine()?.Trim() ?? categoria.Descripcion   ;

        categoria.Nombre = nuevoNombre;
        categoria.Descripcion = nuevaDescripcion;

        if (categoria.Actualizar(conexion))
        {
            Console.WriteLine("Categoría modificada correctamente.");
        }
        else
        {
            Console.WriteLine("Error al modificar la categoría.");
        }
    }
}
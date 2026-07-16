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
                    //InsertarTema();
                    break;

                case "2":
                    MostrarTemas();
                    break;

                case "3":
                    //ModificarTema();
                    break;

                case "4":
                    //EliminarTema();
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
}
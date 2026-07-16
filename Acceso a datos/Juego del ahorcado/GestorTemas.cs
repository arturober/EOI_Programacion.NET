using Microsoft.Data.Sqlite;

class GestorTemas
{
    private readonly SqliteConnection conexion;

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
            Console.WriteLine("GESTIÓN DE TEMAS");
            Console.WriteLine("================");
            Console.WriteLine("1. Añadir tema");
            Console.WriteLine("2. Mostrar todos los temas");
            Console.WriteLine("3. Buscar tema");
            Console.WriteLine("4. Modificar tema");
            Console.WriteLine("5. Eliminar tema");
            Console.WriteLine("0. Volver al menú principal");
            Console.WriteLine("----------------");
            Console.Write("Selecciona una opción: ");

            string opcion = Console.ReadLine() ?? "";

            switch (opcion)
            {
                case "1":
                    AnadirTema();
                    break;

                case "2":
                    MostrarTodos();
                    break;

                case "3":
                    BuscarTema();
                    break;

                case "4":
                    ModificarTema();
                    break;

                case "5":
                    EliminarTema();
                    break;

                case "0":
                    volver = true;
                    break;

                default:
                    Console.WriteLine("Opción no válida.");
                    TextoUtil.Pausar();
                    break;
            }
        }
    }

    private void AnadirTema()
    {
        Console.Clear();
        Console.WriteLine("AÑADIR TEMA");
        Console.WriteLine("============");

        string nombre = TextoUtil.LeerTextoObligatorio(
            "Nombre: ");

        bool yaExiste = Tema.Existe(
            conexion,
            nombre);

        if (yaExiste)
        {
            Console.WriteLine(
                "Ya existe un tema con ese nombre.");
        }
        else
        {
            string descripcion = TextoUtil.LeerTextoObligatorio(
                "Descripción: ");

            Tema tema = new Tema(
                nombre,
                descripcion);

            bool insertado = tema.Insertar(conexion);

            if (insertado)
            {
                Console.WriteLine(
                    "Tema añadido correctamente.");
            }
            else
            {
                Console.WriteLine(
                    "No se ha podido añadir el tema.");
            }
        }

        TextoUtil.Pausar();
    }

    private void MostrarTodos()
    {
        Console.Clear();
        Console.WriteLine("LISTA DE TEMAS");
        Console.WriteLine("===============");

        List<Tema> temas = Tema.Listar(conexion);
        MostrarLista(temas);

        TextoUtil.Pausar();
    }

    private void BuscarTema()
    {
        Console.Clear();
        Console.WriteLine("BUSCAR TEMA");
        Console.WriteLine("===========");

        string texto = TextoUtil.LeerTextoObligatorio(
            "Texto que quieres buscar: ");

        List<Tema> temas = Tema.Buscar(
            conexion,
            texto);

        MostrarLista(temas);
        TextoUtil.Pausar();
    }

    private void ModificarTema()
    {
        Console.Clear();
        Console.WriteLine("MODIFICAR TEMA");
        Console.WriteLine("==============");

        int id = TextoUtil.LeerEnteroPositivo(
            "ID del tema que quieres modificar: ");

        Tema? tema = Tema.BuscarPorId(
            conexion,
            id);

        if (tema == null)
        {
            Console.WriteLine(
                "No existe ningún tema con ese ID.");
        }
        else
        {
            Console.WriteLine("Nombre actual: " + tema.Nombre);
            Console.Write(
                "Nuevo nombre (Enter para conservarlo): ");

            string nuevoNombre =
                (Console.ReadLine() ?? "").Trim();

            if (nuevoNombre == "")
            {
                nuevoNombre = tema.Nombre;
            }

            bool existeOtro = Tema.Existe(
                conexion,
                nuevoNombre,
                tema.Id);

            if (existeOtro)
            {
                Console.WriteLine(
                    "Ya existe otro tema con ese nombre.");
            }
            else
            {
                Console.WriteLine(
                    "Descripción actual: " + tema.Descripcion);
                Console.Write(
                    "Nueva descripción (Enter para conservarla): ");

                string nuevaDescripcion =
                    (Console.ReadLine() ?? "").Trim();

                if (nuevaDescripcion == "")
                {
                    nuevaDescripcion = tema.Descripcion;
                }

                // Modificamos el objeto y después actualizamos su fila.
                tema.Nombre = nuevoNombre;
                tema.Descripcion = nuevaDescripcion;

                bool actualizado = tema.Actualizar(conexion);

                if (actualizado)
                {
                    Console.WriteLine(
                        "Tema modificado correctamente.");
                }
                else
                {
                    Console.WriteLine(
                        "No se ha podido modificar el tema.");
                }
            }
        }

        TextoUtil.Pausar();
    }

    private void EliminarTema()
    {
        Console.Clear();
        Console.WriteLine("ELIMINAR TEMA");
        Console.WriteLine("=============");

        int id = TextoUtil.LeerEnteroPositivo(
            "ID del tema que quieres eliminar: ");

        Tema? tema = Tema.BuscarPorId(
            conexion,
            id);

        if (tema == null)
        {
            Console.WriteLine(
                "No existe ningún tema con ese ID.");
        }
        else
        {
            int cantidadPalabras = tema.ContarPalabras(conexion);

            Console.WriteLine("Tema: " + tema.Nombre);
            Console.WriteLine(
                "Descripción: " + tema.Descripcion);
            Console.WriteLine(
                "Palabras asociadas: " + cantidadPalabras);

            if (cantidadPalabras > 0)
            {
                Console.WriteLine();
                Console.WriteLine(
                    "No se puede eliminar el tema porque contiene palabras.");
                Console.WriteLine(
                    "Primero debes eliminar esas palabras o cambiarlas de tema.");
            }
            else
            {
                bool confirmar = TextoUtil.Confirmar(
                    "¿Seguro que quieres eliminarlo?");

                if (confirmar)
                {
                    bool borrado = tema.Borrar(conexion);

                    if (borrado)
                    {
                        Console.WriteLine(
                            "Tema eliminado correctamente.");
                    }
                    else
                    {
                        Console.WriteLine(
                            "No se ha podido eliminar el tema.");
                    }
                }
                else
                {
                    Console.WriteLine("Operación cancelada.");
                }
            }
        }

        TextoUtil.Pausar();
    }

    private void MostrarLista(List<Tema> temas)
    {
        if (temas.Count == 0)
        {
            Console.WriteLine(
                "No se han encontrado temas.");
        }
        else
        {
            foreach (Tema tema in temas)
            {
                int cantidad = tema.ContarPalabras(conexion);

                Console.WriteLine(
                    tema.Id + ". " + tema.Nombre +
                    " (" + cantidad + " palabras)");
                Console.WriteLine(
                    "   " + tema.Descripcion);
                Console.WriteLine();
            }

            Console.WriteLine(
                "Total: " + temas.Count + " temas.");
        }
    }
}

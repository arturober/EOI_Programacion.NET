using Microsoft.Data.Sqlite;

class GestorPalabras
{
    private readonly SqliteConnection conexion;

    public GestorPalabras(SqliteConnection conexion)
    {
        this.conexion = conexion;
    }

    public void MostrarMenu()
    {
        bool volver = false;

        while (!volver)
        {
            Console.Clear();
            Console.WriteLine("GESTIÓN DE PALABRAS");
            Console.WriteLine("===================");
            Console.WriteLine("1. Añadir palabra");
            Console.WriteLine("2. Mostrar todas las palabras");
            Console.WriteLine("3. Buscar palabra");
            Console.WriteLine("4. Modificar palabra");
            Console.WriteLine("5. Eliminar palabra");
            Console.WriteLine("0. Volver al menú principal");
            Console.WriteLine("-------------------");
            Console.Write("Selecciona una opción: ");

            string opcion = Console.ReadLine() ?? "";

            switch (opcion)
            {
                case "1":
                    AnadirPalabra();
                    break;

                case "2":
                    MostrarTodas();
                    break;

                case "3":
                    BuscarPalabra();
                    break;

                case "4":
                    ModificarPalabra();
                    break;

                case "5":
                    EliminarPalabra();
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

    private void AnadirPalabra()
    {
        Console.Clear();
        Console.WriteLine("AÑADIR PALABRA");
        Console.WriteLine("==============");

        List<Tema> temas = Tema.Listar(conexion);

        if (temas.Count == 0)
        {
            Console.WriteLine(
                "No se puede añadir una palabra porque no existen temas.");
            Console.WriteLine(
                "Primero debes crear un tema desde el CRUD de temas.");
            TextoUtil.Pausar();
            return;
        }

        string texto = LeerPalabraValida(
            "Palabra o expresión: ");

        // Aunque Insertar() también lo comprueba, lo hacemos aquí para mostrar
        // un mensaje específico antes de pedir la pista y el tema.
        bool yaExiste = Palabra.Existe(
            conexion,
            texto);

        if (yaExiste)
        {
            Console.WriteLine(
                "Esa palabra ya existe en la base de datos.");
        }
        else
        {
            string pista = TextoUtil.LeerTextoObligatorio(
                "Pista: ");
            Tema tema = SeleccionarTema();

            Palabra palabra = new Palabra(
                texto,
                pista,
                tema);

            bool insertada = palabra.Insertar(conexion);

            if (insertada)
            {
                Console.WriteLine(
                    "Palabra añadida correctamente.");
            }
            else
            {
                Console.WriteLine(
                    "No se ha podido añadir la palabra.");
            }
        }

        TextoUtil.Pausar();
    }

    private void MostrarTodas()
    {
        Console.Clear();
        Console.WriteLine("LISTA DE PALABRAS");
        Console.WriteLine("=================");

        List<Palabra> palabras = Palabra.Listar(conexion);
        MostrarLista(palabras);

        TextoUtil.Pausar();
    }

    private void BuscarPalabra()
    {
        Console.Clear();
        Console.WriteLine("BUSCAR PALABRA");
        Console.WriteLine("==============");

        string texto = TextoUtil.LeerTextoObligatorio(
            "Texto que quieres buscar: ");

        List<Palabra> palabras = Palabra.Buscar(
            conexion,
            texto);

        MostrarLista(palabras);
        TextoUtil.Pausar();
    }

    private void ModificarPalabra()
    {
        Console.Clear();
        Console.WriteLine("MODIFICAR PALABRA");
        Console.WriteLine("=================");

        int id = TextoUtil.LeerEnteroPositivo(
            "ID de la palabra que quieres modificar: ");

        Palabra? palabra = Palabra.BuscarPorId(
            conexion,
            id);

        if (palabra == null)
        {
            Console.WriteLine(
                "No existe ninguna palabra con ese ID.");
        }
        else
        {
            string nuevoTexto = LeerPalabraOpcional(palabra.Texto);

            bool existeOtra = Palabra.Existe(
                conexion,
                nuevoTexto,
                palabra.Id);

            if (existeOtra)
            {
                Console.WriteLine(
                    "Ya existe otra palabra igual en la base de datos.");
            }
            else
            {
                Console.WriteLine(
                    "Pista actual: " + palabra.Pista);
                Console.Write(
                    "Nueva pista (Enter para conservarla): ");

                string nuevaPista =
                    (Console.ReadLine() ?? "").Trim();

                if (nuevaPista == "")
                {
                    nuevaPista = palabra.Pista;
                }

                Tema nuevoTema = palabra.Tema;

                Console.WriteLine(
                    "Tema actual: " + palabra.Tema.Nombre);

                bool cambiarTema = TextoUtil.Confirmar(
                    "¿Quieres cambiar el tema?");

                if (cambiarTema)
                {
                    nuevoTema = SeleccionarTema();
                }

                // Modificamos las propiedades del objeto y después le pedimos
                // al propio objeto que actualice su fila de la base de datos.
                palabra.Texto = nuevoTexto;
                palabra.Pista = nuevaPista;
                palabra.Tema = nuevoTema;

                bool actualizada = palabra.Actualizar(conexion);

                if (actualizada)
                {
                    Console.WriteLine(
                        "Palabra modificada correctamente.");
                }
                else
                {
                    Console.WriteLine(
                        "No se ha podido modificar la palabra.");
                }
            }
        }

        TextoUtil.Pausar();
    }

    private void EliminarPalabra()
    {
        Console.Clear();
        Console.WriteLine("ELIMINAR PALABRA");
        Console.WriteLine("================");

        int id = TextoUtil.LeerEnteroPositivo(
            "ID de la palabra que quieres eliminar: ");

        Palabra? palabra = Palabra.BuscarPorId(
            conexion,
            id);

        if (palabra == null)
        {
            Console.WriteLine(
                "No existe ninguna palabra con ese ID.");
        }
        else
        {
            Console.WriteLine("Palabra: " + palabra.Texto);
            Console.WriteLine("Pista: " + palabra.Pista);
            Console.WriteLine("Tema: " + palabra.Tema.Nombre);

            bool confirmar = TextoUtil.Confirmar(
                "¿Seguro que quieres eliminarla?");

            if (confirmar)
            {
                bool borrada = palabra.Borrar(conexion);

                if (borrada)
                {
                    Console.WriteLine(
                        "Palabra eliminada correctamente.");
                }
                else
                {
                    Console.WriteLine(
                        "No se ha podido eliminar la palabra.");
                }
            }
            else
            {
                Console.WriteLine("Operación cancelada.");
            }
        }

        TextoUtil.Pausar();
    }

    private Tema SeleccionarTema()
    {
        Tema? temaSeleccionado = null;

        while (temaSeleccionado == null)
        {
            Console.WriteLine();
            Console.WriteLine("TEMAS");
            Console.WriteLine("-----");

            List<Tema> temas = Tema.Listar(conexion);

            foreach (Tema tema in temas)
            {
                Console.WriteLine(
                    tema.Id + ". " + tema.Nombre);
            }

            int id = TextoUtil.LeerEnteroPositivo(
                "Selecciona el ID del tema: ");

            temaSeleccionado = Tema.BuscarPorId(
                conexion,
                id);

            if (temaSeleccionado == null)
            {
                Console.WriteLine(
                    "No existe ningún tema con ese ID.");
            }
        }

        return temaSeleccionado;
    }

    private string LeerPalabraValida(string mensaje)
    {
        string texto = "";
        bool valida = false;

        while (!valida)
        {
            texto = TextoUtil.LeerTextoObligatorio(mensaje);
            valida = TextoUtil.EsPalabraValida(texto);

            if (!valida)
            {
                Console.WriteLine(
                    "Solo se permiten letras, espacios y guiones.");
            }
        }

        return texto;
    }

    private string LeerPalabraOpcional(string textoActual)
    {
        string nuevoTexto = "";
        bool valida = false;

        while (!valida)
        {
            Console.WriteLine(
                "Palabra actual: " + textoActual);
            Console.Write(
                "Nueva palabra (Enter para conservarla): ");

            nuevoTexto = (Console.ReadLine() ?? "").Trim();

            if (nuevoTexto == "")
            {
                nuevoTexto = textoActual;
                valida = true;
            }
            else
            {
                valida = TextoUtil.EsPalabraValida(nuevoTexto);

                if (!valida)
                {
                    Console.WriteLine(
                        "Solo se permiten letras, espacios y guiones.");
                }
            }
        }

        return nuevoTexto;
    }

    private void MostrarLista(List<Palabra> palabras)
    {
        if (palabras.Count == 0)
        {
            Console.WriteLine(
                "No se han encontrado palabras.");
        }
        else
        {
            foreach (Palabra palabra in palabras)
            {
                Console.WriteLine(palabra.ToString());
            }

            Console.WriteLine();
            Console.WriteLine("Total: " + palabras.Count + " palabras.");
        }
    }
}

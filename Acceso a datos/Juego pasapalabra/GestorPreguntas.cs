using Microsoft.Data.Sqlite;

class GestorPreguntas
{
    private SqliteConnection conexion;

    public GestorPreguntas(SqliteConnection conexion)
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
            Console.WriteLine("=== GESTOR DE PREGUNTAS ===");
            Console.WriteLine("1. Crear pregunta");
            Console.WriteLine("2. Mostrar preguntas");
            Console.WriteLine("3. Modificar pregunta");
            Console.WriteLine("4. Eliminar pregunta");
            Console.WriteLine("0. Volver al menú principal");
            Console.Write("Seleccione una opción: ");

            string opcion = Console.ReadLine() ?? "";

            switch (opcion)
            {
                case "1":
                    InsertarPregunta();
                    break;

                case "2":
                    MostrarPreguntas();
                    break;

                case "3":
                    ModificarPregunta();
                    break;

                case "4":
                    EliminarPregunta();
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

    private void MostrarPreguntas()
    {
        Console.Clear();
        Console.WriteLine("=== LISTA DE PREGUNTAS ===");
        List<Pregunta> preguntas = Pregunta.Listar(conexion);
        MostrarLista(preguntas);
    }

    private void MostrarLista(List<Pregunta> preguntas)
    {
        if (preguntas.Count == 0)
        {
            Console.WriteLine("No hay preguntas disponibles.");
            return;
        }

        foreach (Pregunta pregunta in preguntas)
        {
            Console.WriteLine(pregunta.ToString());
        }

        Console.WriteLine();
    }

    private void InsertarPregunta()
    {
        Console.Clear();
        Console.WriteLine("=== INSERTAR PREGUNTA ===");
        Console.WriteLine("========================");

        Tema? tema = SeleccionarTema();
        if (tema == null)
        {
            return;
        }

        char letra = TextoUtil.LeerLetraRosco("Introduce la letra de la pregunta: ");
        string respuesta = LeerRespuestaValida(letra);

        if (Pregunta.Existe(conexion, respuesta, tema.Id))
        {
            Console.WriteLine("Ya existe una pregunta con esa respuesta dentro del tema seleccionado.");
            return;
        }

        string definicion = TextoUtil.LeerTextoObligatorio("Introduce la definición: ");

        Pregunta nuevaPregunta = new Pregunta(letra, respuesta, definicion, tema);
        if (nuevaPregunta.Insertar(conexion))
        {
            Console.WriteLine("Pregunta insertada correctamente.");
        }
        else
        {
            Console.WriteLine("Error al insertar la pregunta.");
        }
    }

    private void EliminarPregunta()
    {
        Console.Clear();
        Console.WriteLine("=== ELIMINAR PREGUNTA ===");
        Console.WriteLine("=========================");

        List<Pregunta> preguntas = Pregunta.Listar(conexion);
        MostrarLista(preguntas);

        if (preguntas.Count == 0)
        {
            return;
        }

        int id = TextoUtil.LeerEnteroPositivo("Introduce el ID de la pregunta a eliminar: ");

        Pregunta? pregunta = BuscarPorId(preguntas, id);
        if (pregunta == null)
        {
            Console.WriteLine("No se encontró una pregunta con ese ID.");
            return;
        }

        bool confirmacion = TextoUtil.Confirmar(
            $"¿Estás seguro de que deseas eliminar la pregunta '{pregunta.Respuesta}'?");

        if (confirmacion)
        {
            if (pregunta.Borrar(conexion))
            {
                Console.WriteLine("Pregunta eliminada correctamente.");
            }
            else
            {
                Console.WriteLine("Error al eliminar la pregunta.");
            }
        }
        else
        {
            Console.WriteLine("Operación cancelada. La pregunta no se ha eliminado.");
        }
    }

    private void ModificarPregunta()
    {
        Console.Clear();
        Console.WriteLine("=== MODIFICAR PREGUNTA ===");
        Console.WriteLine("==========================");

        List<Pregunta> preguntas = Pregunta.Listar(conexion);
        MostrarLista(preguntas);

        if (preguntas.Count == 0)
        {
            return;
        }

        int id = TextoUtil.LeerEnteroPositivo("Introduce el ID de la pregunta a modificar: ");

        Pregunta? pregunta = BuscarPorId(preguntas, id);
        if (pregunta == null)
        {
            Console.WriteLine("No se encontró una pregunta con ese ID.");
            return;
        }

        char nuevaLetra = LeerNuevaLetra(pregunta.Letra);
        string nuevaRespuesta = LeerNuevaRespuesta(pregunta.Respuesta, nuevaLetra);
        string nuevaDefinicion = LeerNuevoTexto(
            "Definición actual: ",
            "Nueva definición (Enter para conservar): ",
            pregunta.Definicion);

        Tema? nuevoTema = SeleccionarNuevoTema(pregunta.Tema);
        if (nuevoTema == null)
        {
            return;
        }

        if (Pregunta.Existe(conexion, nuevaRespuesta, nuevoTema.Id, pregunta.Id))
        {
            Console.WriteLine("Ya existe una pregunta con esa respuesta dentro del tema seleccionado.");
            return;
        }

        pregunta.Letra = nuevaLetra;
        pregunta.Respuesta = nuevaRespuesta;
        pregunta.Definicion = nuevaDefinicion;
        pregunta.Tema = nuevoTema;

        if (pregunta.Actualizar(conexion))
        {
            Console.WriteLine("Pregunta modificada correctamente.");
        }
        else
        {
            Console.WriteLine("Error al modificar la pregunta.");
        }
    }

    private Tema? SeleccionarTema()
    {
        List<Tema> temas = Tema.Listar(conexion);

        if (temas.Count == 0)
        {
            Console.WriteLine("No hay temas disponibles.");
            Console.WriteLine("Primero debes crear al menos un tema.");
            return null;
        }

        Console.WriteLine();
        Console.WriteLine("Temas disponibles:");
        foreach (Tema tema in temas)
        {
            Console.WriteLine(tema.ToString());
        }

        int idTema = TextoUtil.LeerEnteroPositivo("Introduce el ID del tema: ");
        Tema? temaSeleccionado = Tema.BuscarPorId(conexion, idTema);

        if (temaSeleccionado == null)
        {
            Console.WriteLine("No se encontró un tema con ese ID.");
        }

        return temaSeleccionado;
    }

    private Tema? SeleccionarNuevoTema(Tema temaActual)
    {
        List<Tema> temas = Tema.Listar(conexion);

        Console.WriteLine();
        Console.WriteLine("Tema actual: " + temaActual.Nombre);
        Console.WriteLine("Temas disponibles:");
        foreach (Tema tema in temas)
        {
            Console.WriteLine(tema.ToString());
        }

        Console.Write("Nuevo ID de tema (Enter para conservar): ");
        string entrada = Console.ReadLine()?.Trim() ?? "";

        if (entrada == "")
        {
            return temaActual;
        }

        int idTema;
        if (!int.TryParse(entrada, out idTema) || idTema < 0)
        {
            Console.WriteLine("El ID introducido no es válido.");
            return null;
        }

        Tema? nuevoTema = Tema.BuscarPorId(conexion, idTema);
        if (nuevoTema == null)
        {
            Console.WriteLine("No se encontró un tema con ese ID.");
        }

        return nuevoTema;
    }

    private string LeerRespuestaValida(char letra)
    {
        while (true)
        {
            string respuesta = TextoUtil.LeerTextoObligatorio("Introduce la respuesta: ");

            if (!TextoUtil.EsRespuestaValida(respuesta))
            {
                Console.WriteLine("La respuesta solo puede contener letras, espacios y guiones.");
            }
            else if (!TextoUtil.RespuestaContieneLetra(respuesta, letra))
            {
                Console.WriteLine($"La respuesta debe contener la letra '{letra}'.");
            }
            else
            {
                return respuesta;
            }
        }
    }

    private char LeerNuevaLetra(char letraActual)
    {
        while (true)
        {
            Console.WriteLine("Letra actual: " + letraActual);
            Console.Write("Nueva letra (Enter para conservar): ");
            string entrada = Console.ReadLine()?.Trim().ToUpper() ?? "";

            if (entrada == "")
            {
                return letraActual;
            }

            if (entrada.Length == 1 && TextoUtil.EsLetraDelRosco(entrada[0]))
            {
                return entrada[0];
            }

            Console.WriteLine("Introduce una sola letra válida del rosco (A-Z, incluyendo Ñ).");
        }
    }

    private string LeerNuevaRespuesta(string respuestaActual, char letra)
    {
        while (true)
        {
            Console.WriteLine("Respuesta actual: " + respuestaActual);
            Console.Write("Nueva respuesta (Enter para conservar): ");
            string respuesta = Console.ReadLine()?.Trim() ?? "";

            if (respuesta == "")
            {
                respuesta = respuestaActual;
            }

            if (!TextoUtil.EsRespuestaValida(respuesta))
            {
                Console.WriteLine("La respuesta solo puede contener letras, espacios y guiones.");
            }
            else if (!TextoUtil.RespuestaContieneLetra(respuesta, letra))
            {
                Console.WriteLine($"La respuesta debe contener la letra '{letra}'.");
            }
            else
            {
                return respuesta;
            }
        }
    }

    private string LeerNuevoTexto(string mensajeActual, string mensajeNuevo, string textoActual)
    {
        Console.WriteLine(mensajeActual + textoActual);
        Console.Write(mensajeNuevo);
        string nuevoTexto = Console.ReadLine()?.Trim() ?? "";

        if (nuevoTexto == "")
        {
            return textoActual;
        }

        return nuevoTexto;
    }

    private Pregunta? BuscarPorId(List<Pregunta> preguntas, int id)
    {
        foreach (Pregunta pregunta in preguntas)
        {
            if (pregunta.Id == id)
            {
                return pregunta;
            }
        }

        return null;
    }
}

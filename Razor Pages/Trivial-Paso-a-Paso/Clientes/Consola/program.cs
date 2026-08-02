// Importa las extensiones necesarias para convertir automáticamente
// las respuestas JSON de la API en objetos de C#.
using System.Net.Http.Json;

// Si se proporciona una dirección como argumento, se utiliza esa dirección.
// En caso contrario, se utiliza la dirección local predeterminada.
string urlApi = args.Length > 0
    ? args[0]
    : "http://localhost:5000/api/";

// HttpClient permite realizar peticiones HTTP a la API.
// La instrucción using garantiza que se liberen sus recursos al finalizar.
using HttpClient cliente = new()
{
    // BaseAddress contiene la parte común de todas las peticiones.
    // Después solo será necesario indicar rutas como "categorias".
    BaseAddress = new Uri(urlApi)
};

// Mostramos la cabecera de la aplicación.
Console.WriteLine("JUEGO DEL TRIVIAL");
Console.WriteLine("=================");
Console.WriteLine($"Conectando con la API en {urlApi}\n");

try
{
    // Solicitamos las categorías a GET /api/categorias.
    // GetFromJsonAsync convierte el JSON recibido en una lista de CategoriaDto.
    List<CategoriaDto>? categorias =
        await cliente.GetFromJsonAsync<List<CategoriaDto>>("categorias");

    // Aunque normalmente la API devolverá una lista, comprobamos que
    // el contenido recibido no sea nulo antes de utilizarlo.
    if (categorias is null)
    {
        Console.WriteLine("No se han recibido categorías de la API.");
        return;
    }

    // Mostramos todas las categorías disponibles y sus identificadores.
    Console.WriteLine("Categorías disponibles:");

    foreach (CategoriaDto categoria in categorias)
    {
        Console.WriteLine($"{categoria.Id} - {categoria.Nombre}");
    }

    Console.WriteLine();

    // Esta variable almacenará el identificador elegido por el usuario.
    int categoriaId;

    // Repetimos la pregunta hasta que el usuario introduzca un valor válido.
    while (true)
    {
        Console.Write(
            "Seleccione una categoría por su ID (o 0 para cualquiera): ");

        string? input = Console.ReadLine();

        // TryParse intenta convertir el texto en un número entero.
        // También comprobamos que sea 0 o que corresponda a una categoría.
        if (int.TryParse(input, out categoriaId) &&
            (categoriaId == 0 ||
             categorias.Exists(categoria => categoria.Id == categoriaId)))
        {
            break;
        }

        Console.WriteLine(
            "ID de categoría no válido. Intente de nuevo.");
    }

    // Solicitamos un máximo de diez preguntas.
    // categoriaId permite filtrar por categoría. El valor 0 indica que
    // pueden devolverse preguntas de cualquier categoría.
    List<PreguntaDto>? preguntas =
        await cliente.GetFromJsonAsync<List<PreguntaDto>>(
            $"preguntas?cantidad=10&categoriaId={categoriaId}");

    // Contador de respuestas correctas.
    int aciertos = 0;

    // Recorremos todas las preguntas recibidas.
    // El operador ?. evita acceder a Count si preguntas es null.
    for (int posicion = 0; posicion < preguntas?.Count; posicion++)
    {
        // Obtenemos la pregunta situada en la posición actual.
        PreguntaDto pregunta = preguntas[posicion];

        // Mostramos el número de pregunta y su categoría.
        Console.WriteLine(
            $"\nPregunta {posicion + 1} / {preguntas.Count}:");
        Console.WriteLine(
            $"(Categoría {pregunta.Categoria.Nombre})\n");

        // Mostramos el enunciado.
        Console.WriteLine(pregunta.Enunciado);

        // Recorremos las posibles respuestas.
        // El índice comienza en 0, pero se muestra desde el número 1.
        for (int i = 0; i < pregunta.Respuestas.Length; i++)
        {
            Console.WriteLine(
                $"{i + 1}. {pregunta.Respuestas[i]}");
        }

        // Guardará el número de respuesta elegido por el usuario.
        int respuestaUsuario;

        // Repetimos la lectura hasta obtener un número comprendido entre 1 y 4.
        while (true)
        {
            Console.Write("Seleccione su respuesta (1-4): ");
            string? input = Console.ReadLine();

            if (int.TryParse(input, out respuestaUsuario) &&
                respuestaUsuario >= 1 &&
                respuestaUsuario <= 4)
            {
                break;
            }

            Console.WriteLine(
                "Respuesta no válida. Intente de nuevo.");
        }

        // Comparamos la respuesta elegida con el número de la respuesta
        // correcta proporcionado por la API.
        if (respuestaUsuario == pregunta.RespuestaCorrecta)
        {
            Console.WriteLine("¡Correcto!");
            aciertos++;
        }
        else
        {
            // RespuestaCorrecta utiliza valores entre 1 y 4, mientras que
            // los índices del array están comprendidos entre 0 y 3.
            // Por ese motivo se resta 1.
            string respuestaCorrecta =
                pregunta.Respuestas[pregunta.RespuestaCorrecta - 1];

            Console.WriteLine(
                $"Incorrecto. La respuesta correcta era: " +
                $"{respuestaCorrecta}");
        }
    }

    // Mostramos el resultado final.
    Console.WriteLine(
        $"\nJuego terminado. " +
        $"Aciertos: {aciertos} de {preguntas?.Count}\n");
}
catch (Exception ex)
{
    // Aquí se capturan errores de conexión, direcciones incorrectas,
    // respuestas HTTP no válidas o problemas al convertir el JSON.
    Console.WriteLine(
        $"Error al conectar con la API: {ex.Message}");
}

// DTO utilizado para recibir una categoría desde la API.
// Un record resulta apropiado para representar datos que se transportan
// entre el servidor y el cliente.
public record CategoriaDto(
    int Id,
    string Nombre);

// DTO que representa una pregunta recibida desde la API.
public record PreguntaDto(
    // Identificador de la pregunta.
    int Id,

    // Texto de la pregunta.
    string Enunciado,

    // Array que contiene las cuatro respuestas posibles.
    string[] Respuestas,

    // Posición de la respuesta correcta, utilizando valores entre 1 y 4.
    int RespuestaCorrecta,

    // Información de la categoría a la que pertenece la pregunta.
    CategoriaDto Categoria);
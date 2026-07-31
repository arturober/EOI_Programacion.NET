using System.Net.Http.Json;

string urlApi = args.Length > 0 ? args[0] : "http://localhost:5000/api/";

using HttpClient cliente = new()
{
    BaseAddress = new Uri(urlApi)
};

Console.WriteLine("JUEGO DEL TRIVIAL");
Console.WriteLine("=================");
Console.WriteLine($"Conectando con la API en {urlApi}\n");

try
{
    List<CategoriaDto>? categorias =
        await cliente.GetFromJsonAsync<List<CategoriaDto>>("categorias");

    if (categorias is null)
    {
        Console.WriteLine("No se han recibido categorías de la API.");
        return;
    }

    Console.WriteLine("Categorías disponibles:");
    foreach (CategoriaDto categoria in categorias)
    {
        Console.WriteLine($"{categoria.Id} - {categoria.Nombre}");
    }

    Console.WriteLine();

    int categoriaId;

    while (true)
    {
        Console.Write("Seleccione una categoría por su ID (o 0 para cualquiera): ");
        string? input = Console.ReadLine();

        if (int.TryParse(input, out categoriaId) &&
            (categoriaId == 0 || categorias.Exists(c => c.Id == categoriaId)))
        {
            break;
        }

        Console.WriteLine("ID de categoría no válido. Intente de nuevo.");
    }

    List<PreguntaDto>? preguntas =
        await cliente.GetFromJsonAsync<List<PreguntaDto>>(
            $"preguntas?cantidad=10&categoriaId={categoriaId}");

    int aciertos = 0;

    for (int posicion = 0; posicion < preguntas?.Count; posicion++)
    {
        PreguntaDto pregunta = preguntas[posicion];

        Console.WriteLine($"\nPregunta {posicion + 1} / {preguntas.Count}:");
        Console.WriteLine($"(Categoría {pregunta.Categoria.Nombre})\n");
        Console.WriteLine(pregunta.Enunciado);
        for (int i = 0; i < pregunta.Respuestas.Length; i++)
        {
            Console.WriteLine($"{i + 1}. {pregunta.Respuestas[i]}");
        }

        int respuestaUsuario;
        while (true)
        {
            Console.Write("Seleccione su respuesta (1-4): ");
            string? input = Console.ReadLine();

            if (int.TryParse(input, out respuestaUsuario) &&
                respuestaUsuario >= 1 && respuestaUsuario <= 4)
            {
                break;
            }

            Console.WriteLine("Respuesta no válida. Intente de nuevo.");
        }

        if (respuestaUsuario == pregunta.RespuestaCorrecta)
        {
            Console.WriteLine("¡Correcto!");
            aciertos++;
        }
        else
        {
            Console.WriteLine($"Incorrecto. La respuesta correcta era: {pregunta.Respuestas[pregunta.RespuestaCorrecta - 1]}");
        }
    }

    Console.WriteLine($"\nJuego terminado. Aciertos: {aciertos} de {preguntas?.Count}\n");
}
catch (Exception ex)
{
    Console.WriteLine($"Error al conectar con la API: {ex.Message}");
}

public record CategoriaDto(int Id, string Nombre);

public record PreguntaDto(
    int Id,
    string Enunciado,
    string[] Respuestas,
    int RespuestaCorrecta,
    CategoriaDto Categoria);
using System.Net;
using System.Text.Json;
using TrivialApi.DTOs;
using Xunit;
using Xunit.Abstractions;

namespace TrivialApi.Tests;

[Collection(ApiTestCollection.Nombre)]
// Comprueba la consulta de una pregunta concreta mediante su identificador.
public class PreguntasDetalleApiTests(
    ApiServerFixture servidor,
    ITestOutputHelper salida)
    : ApiTestBase(servidor, salida)
{
    private const string RutaPreguntas = "/api/preguntas";

    [Fact(DisplayName =
        "GET /api/preguntas/{id} devuelve una pregunta existente")]
    public async Task ObtenerPorId_PreguntaExistente_DevuelveDatosEsperados()
    {
        MostrarInicio("Obtener la primera pregunta insertada para las pruebas");

        HttpResponseMessage respuesta =
            await EnviarGetAsync($"{RutaPreguntas}/1");
        PreguntaDto pregunta = await LeerJsonAsync<PreguntaDto>(respuesta);

        MostrarComprobacion("La pregunta contiene los valores conocidos");
        Assert.Equal(1, pregunta.Id);
        Assert.Equal("¿Quién pintó Las Meninas?", pregunta.Enunciado);
        Assert.Equal(
            new[]
            {
                "Diego Velázquez",
                "Francisco de Goya",
                "Pablo Picasso",
                "El Greco"
            },
            pregunta.Respuestas);
        Assert.Equal(1, pregunta.RespuestaCorrecta);
        Assert.Equal("Arte", pregunta.Categoria.Nombre);

        MostrarFin();
    }

    [Theory(DisplayName =
        "GET /api/preguntas/{id} devuelve 404 para identificadores inexistentes")]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(999999)]
    public async Task ObtenerPorId_IdInexistente_Devuelve404(int id)
    {
        MostrarInicio($"Buscar la pregunta inexistente con identificador {id}");

        HttpResponseMessage respuesta =
            await EnviarGetAsync($"{RutaPreguntas}/{id}");

        MostrarComprobacion("La API devuelve 404 Not Found");
        Assert.Equal(HttpStatusCode.NotFound, respuesta.StatusCode);

        MostrarFin();
    }

    [Fact(DisplayName =
        "GET /api/preguntas/texto devuelve 404")]
    public async Task ObtenerPorId_IdNoNumerico_Devuelve404()
    {
        MostrarInicio("Enviar un identificador de pregunta no numérico");

        HttpResponseMessage respuesta =
            await EnviarGetAsync($"{RutaPreguntas}/texto");

        MostrarComprobacion("La ruta no coincide y devuelve 404 Not Found");
        Assert.Equal(HttpStatusCode.NotFound, respuesta.StatusCode);

        MostrarFin();
    }

    [Fact(DisplayName =
        "GET /api/preguntas/{id} no expone propiedades internas del modelo")]
    public async Task ObtenerPorId_NoExponePropiedadesInternas()
    {
        MostrarInicio("Comprobar el contrato JSON público de una pregunta");

        HttpResponseMessage respuesta =
            await EnviarGetAsync($"{RutaPreguntas}/1");
        respuesta.EnsureSuccessStatusCode();

        await using Stream contenido =
            await respuesta.Content.ReadAsStreamAsync();
        using JsonDocument documento = await JsonDocument.ParseAsync(contenido);
        JsonElement pregunta = documento.RootElement;

        MostrarComprobacion("El JSON contiene el array público respuestas");
        Assert.True(pregunta.TryGetProperty("respuestas", out JsonElement respuestas));
        Assert.Equal(4, respuestas.GetArrayLength());

        MostrarComprobacion(
            "El JSON no contiene las propiedades internas Respuesta1-Respuesta4");
        Assert.False(pregunta.TryGetProperty("respuesta1", out _));
        Assert.False(pregunta.TryGetProperty("respuesta2", out _));
        Assert.False(pregunta.TryGetProperty("respuesta3", out _));
        Assert.False(pregunta.TryGetProperty("respuesta4", out _));
        Assert.False(pregunta.TryGetProperty("categoriaId", out _));

        MostrarFin();
    }

    [Fact(DisplayName =
        "GET /api/preguntas/{id} devuelve contenido JSON")]
    public async Task ObtenerPorId_DevuelveContenidoJson()
    {
        MostrarInicio("Comprobar el tipo de contenido de una pregunta");

        HttpResponseMessage respuesta =
            await EnviarGetAsync($"{RutaPreguntas}/1");

        MostrarComprobacion("El tipo de contenido es application/json");
        Assert.Equal(
            "application/json",
            respuesta.Content.Headers.ContentType?.MediaType);

        MostrarFin();
    }
}

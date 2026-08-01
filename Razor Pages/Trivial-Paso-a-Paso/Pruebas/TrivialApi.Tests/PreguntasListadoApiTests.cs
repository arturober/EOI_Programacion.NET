using System.Net;
using System.Net.Http.Json;
using TrivialApi.DTOs;
using Xunit;
using Xunit.Abstractions;

namespace TrivialApi.Tests;

[Collection(ApiTestCollection.Nombre)]
// Comprueba el listado de preguntas, sus filtros y sus límites.
public class PreguntasListadoApiTests(
    ApiServerFixture servidor,
    ITestOutputHelper salida)
    : ApiTestBase(servidor, salida)
{
    private const string RutaPreguntas = "/api/preguntas";

    [Fact(DisplayName =
        "GET /api/preguntas usa diez elementos de forma predeterminada")]
    public async Task ObtenerTodas_SinCantidad_DevuelveDiezPreguntas()
    {
        MostrarInicio("Comprobar la cantidad predeterminada de preguntas");

        HttpResponseMessage respuesta = await EnviarGetAsync(RutaPreguntas);
        List<PreguntaDto> preguntas =
            await LeerJsonAsync<List<PreguntaDto>>(respuesta);

        MostrarComprobacion("Se devuelven diez preguntas");
        Assert.Equal(10, preguntas.Count);

        MostrarFin();
    }

    [Theory(DisplayName =
        "GET /api/preguntas respeta la cantidad solicitada")]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(5)]
    [InlineData(12)]
    public async Task ObtenerTodas_CantidadValida_RespetaCantidad(int cantidad)
    {
        MostrarInicio($"Solicitar {cantidad} pregunta o preguntas");

        HttpResponseMessage respuesta =
            await EnviarGetAsync($"{RutaPreguntas}?cantidad={cantidad}");
        List<PreguntaDto> preguntas =
            await LeerJsonAsync<List<PreguntaDto>>(respuesta);

        MostrarComprobacion($"Se reciben exactamente {cantidad} elementos");
        Assert.Equal(cantidad, preguntas.Count);

        MostrarFin();
    }

    [Theory(DisplayName =
        "GET /api/preguntas limita las cantidades menores que uno")]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public async Task ObtenerTodas_CantidadMenorQueUno_DevuelveUnaPregunta(
        int cantidad)
    {
        MostrarInicio($"Solicitar una cantidad no válida: {cantidad}");

        HttpResponseMessage respuesta =
            await EnviarGetAsync($"{RutaPreguntas}?cantidad={cantidad}");
        List<PreguntaDto> preguntas =
            await LeerJsonAsync<List<PreguntaDto>>(respuesta);

        MostrarComprobacion("La cantidad se limita a un elemento");
        Assert.Single(preguntas);

        MostrarFin();
    }

    [Fact(DisplayName =
        "GET /api/preguntas limita las cantidades superiores a mil")]
    public async Task ObtenerTodas_CantidadSuperiorAlMaximo_NoProduceError()
    {
        MostrarInicio("Solicitar una cantidad superior al máximo permitido");

        HttpResponseMessage respuesta =
            await EnviarGetAsync($"{RutaPreguntas}?cantidad=5000");
        List<PreguntaDto> preguntas =
            await LeerJsonAsync<List<PreguntaDto>>(respuesta);

        MostrarComprobacion(
            "Se devuelven las doce preguntas disponibles sin duplicarlas");
        Assert.Equal(12, preguntas.Count);
        Assert.Equal(
            preguntas.Count,
            preguntas.Select(pregunta => pregunta.Id).Distinct().Count());

        MostrarFin();
    }

    [Fact(DisplayName =
        "GET /api/preguntas?cantidad=texto devuelve 400 Bad Request")]
    public async Task ObtenerTodas_CantidadNoNumerica_Devuelve400()
    {
        MostrarInicio("Enviar una cantidad que no se puede convertir a número");

        HttpResponseMessage respuesta =
            await EnviarGetAsync($"{RutaPreguntas}?cantidad=texto");

        MostrarComprobacion("El enlace de modelos devuelve 400 Bad Request");
        Assert.Equal(HttpStatusCode.BadRequest, respuesta.StatusCode);

        MostrarFin();
    }

    [Fact(DisplayName =
        "GET /api/preguntas filtra por categoría")]
    public async Task ObtenerTodas_CategoriaExistente_DevuelveSoloSuCategoria()
    {
        MostrarInicio("Filtrar las preguntas por la categoría Ciencia");

        HttpResponseMessage listadoCategorias =
            await EnviarGetAsync("/api/categorias");
        List<CategoriaDto> categorias =
            await LeerJsonAsync<List<CategoriaDto>>(listadoCategorias);
        CategoriaDto ciencia =
            Assert.Single(categorias, categoria => categoria.Nombre == "Ciencia");

        HttpResponseMessage respuesta = await EnviarGetAsync(
            $"{RutaPreguntas}?categoriaId={ciencia.Id}&cantidad=100");
        List<PreguntaDto> preguntas =
            await LeerJsonAsync<List<PreguntaDto>>(respuesta);

        MostrarComprobacion("Se reciben las cuatro preguntas de Ciencia");
        Assert.Equal(4, preguntas.Count);
        Assert.All(
            preguntas,
            pregunta => Assert.Equal(ciencia.Id, pregunta.Categoria.Id));

        MostrarFin();
    }

    [Fact(DisplayName =
        "GET /api/preguntas devuelve una lista vacía para una categoría inexistente")]
    public async Task ObtenerTodas_CategoriaInexistente_DevuelveListaVacia()
    {
        MostrarInicio("Filtrar por una categoría que no existe");

        HttpResponseMessage respuesta = await EnviarGetAsync(
            $"{RutaPreguntas}?categoriaId=999999&cantidad=100");
        List<PreguntaDto> preguntas =
            await LeerJsonAsync<List<PreguntaDto>>(respuesta);

        MostrarComprobacion("El resultado es una lista vacía");
        Assert.Empty(preguntas);

        MostrarFin();
    }

    [Theory(DisplayName =
        "GET /api/preguntas devuelve una lista vacía para categoriaId no positivo")]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task ObtenerTodas_CategoriaNoPositiva_DevuelveListaVacia(
        int categoriaId)
    {
        MostrarInicio(
            $"Comprobar el comportamiento de categoriaId={categoriaId}");

        HttpResponseMessage respuesta = await EnviarGetAsync(
            $"{RutaPreguntas}?categoriaId={categoriaId}");
        List<PreguntaDto> preguntas =
            await LeerJsonAsync<List<PreguntaDto>>(respuesta);

        MostrarComprobacion(
            "El filtro se aplica y no encuentra preguntas con esa categoría");
        Assert.Empty(preguntas);

        MostrarFin();
    }

    [Fact(DisplayName =
        "GET /api/preguntas?categoriaId=texto devuelve 400 Bad Request")]
    public async Task ObtenerTodas_CategoriaNoNumerica_Devuelve400()
    {
        MostrarInicio(
            "Enviar un identificador de categoría que no es numérico");

        HttpResponseMessage respuesta = await EnviarGetAsync(
            $"{RutaPreguntas}?categoriaId=texto");

        MostrarComprobacion("El enlace de modelos devuelve 400 Bad Request");
        Assert.Equal(HttpStatusCode.BadRequest, respuesta.StatusCode);

        MostrarFin();
    }

    [Fact(DisplayName =
        "GET /api/preguntas devuelve el contrato público esperado")]
    public async Task ObtenerTodas_DevuelveFormatoEsperado()
    {
        MostrarInicio("Comprobar la estructura de cada pregunta devuelta");

        HttpResponseMessage respuesta =
            await EnviarGetAsync($"{RutaPreguntas}?cantidad=12");
        List<PreguntaDto> preguntas =
            await LeerJsonAsync<List<PreguntaDto>>(respuesta);

        MostrarComprobacion("Cada pregunta tiene datos completos y coherentes");
        Assert.All(preguntas, pregunta =>
        {
            Assert.True(pregunta.Id > 0);
            Assert.False(string.IsNullOrWhiteSpace(pregunta.Enunciado));
            Assert.Equal(4, pregunta.Respuestas.Length);
            Assert.All(
                pregunta.Respuestas,
                respuesta => Assert.False(string.IsNullOrWhiteSpace(respuesta)));
            Assert.InRange(pregunta.RespuestaCorrecta, 1, 4);
            Assert.True(pregunta.Categoria.Id > 0);
            Assert.False(string.IsNullOrWhiteSpace(pregunta.Categoria.Nombre));
        });

        MostrarFin();
    }

    [Fact(DisplayName =
        "GET /api/preguntas no repite preguntas en una misma respuesta")]
    public async Task ObtenerTodas_NoDevuelvePreguntasDuplicadas()
    {
        MostrarInicio("Comprobar que el listado no contiene duplicados");

        HttpResponseMessage respuesta =
            await EnviarGetAsync($"{RutaPreguntas}?cantidad=12");
        List<PreguntaDto> preguntas =
            await LeerJsonAsync<List<PreguntaDto>>(respuesta);

        MostrarComprobacion("Todos los identificadores son diferentes");
        Assert.Equal(
            preguntas.Count,
            preguntas.Select(pregunta => pregunta.Id).Distinct().Count());

        MostrarFin();
    }

    [Fact(DisplayName =
        "GET /api/preguntas devuelve contenido JSON")]
    public async Task ObtenerTodas_DevuelveContenidoJson()
    {
        MostrarInicio("Comprobar el tipo de contenido del listado de preguntas");

        HttpResponseMessage respuesta = await EnviarGetAsync(RutaPreguntas);

        MostrarComprobacion("El tipo de contenido es application/json");
        Assert.Equal(
            "application/json",
            respuesta.Content.Headers.ContentType?.MediaType);

        MostrarFin();
    }

    [Fact(DisplayName =
        "POST /api/preguntas devuelve 405 Method Not Allowed")]
    public async Task CrearPregunta_MetodoNoPermitido_Devuelve405()
    {
        MostrarInicio("Intentar crear una pregunta en una API de solo lectura");
        MostrarComprobacion("Se envía una petición POST no admitida");

        HttpResponseMessage respuesta =
            await Cliente.PostAsJsonAsync(RutaPreguntas, new { });

        MostrarComprobacion(
            $"La API responde {(int)respuesta.StatusCode} {respuesta.StatusCode}");
        Assert.Equal(HttpStatusCode.MethodNotAllowed, respuesta.StatusCode);

        MostrarFin();
    }
}

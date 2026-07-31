using System.Net;
using System.Net.Http.Json;
using TrivialApi.DTOs;
using Xunit;

namespace TrivialApi.Tests;

// IClassFixture crea una única fábrica compartida por las pruebas de la clase.
public class ApiTrivialTests(FabricaApi fabrica)
    : IClassFixture<FabricaApi>
{
    private readonly HttpClient _cliente = fabrica.CreateClient();

    [Fact]
    public async Task Categorias_DevuelveLasCategorias()
    {
        // Actuar: realizamos una petición HTTP real al servidor de pruebas.
        HttpResponseMessage respuesta =
            await _cliente.GetAsync("/api/categorias");

        // Comprobar: la petición debe ser correcta y contener los datos
        // insertados por FabricaApi.
        respuesta.EnsureSuccessStatusCode();

        List<CategoriaDto>? categorias =
            await respuesta.Content
                .ReadFromJsonAsync<List<CategoriaDto>>();

        Assert.NotNull(categorias);
        Assert.Equal(2, categorias.Count);
        Assert.Contains(categorias, c => c.Nombre == "Ciencia");
        Assert.Contains(categorias, c => c.Nombre == "Cultura");
    }

    [Fact]
    public async Task Preguntas_RespetaLaCantidadSolicitada()
    {
        List<PreguntaDto>? preguntas =
            await _cliente.GetFromJsonAsync<List<PreguntaDto>>(
                "/api/preguntas?cantidad=2");

        Assert.NotNull(preguntas);
        Assert.Equal(2, preguntas.Count);
    }

    [Fact]
    public async Task Preguntas_DevuelveElFormatoEsperado()
    {
        List<PreguntaDto>? preguntas =
            await _cliente.GetFromJsonAsync<List<PreguntaDto>>(
                "/api/preguntas?cantidad=1");

        Assert.NotNull(preguntas);
        PreguntaDto pregunta = Assert.Single(preguntas);

        Assert.NotEmpty(pregunta.Enunciado);
        Assert.Equal(4, pregunta.Respuestas.Length);
        Assert.InRange(pregunta.RespuestaCorrecta, 1, 4);
        Assert.NotNull(pregunta.Categoria);
    }

    [Fact]
    public async Task Preguntas_FiltraPorCategoria()
    {
        List<CategoriaDto>? categorias =
            await _cliente.GetFromJsonAsync<List<CategoriaDto>>(
                "/api/categorias");

        Assert.NotNull(categorias);
        CategoriaDto ciencia =
            Assert.Single(categorias, c => c.Nombre == "Ciencia");

        List<PreguntaDto>? preguntas =
            await _cliente.GetFromJsonAsync<List<PreguntaDto>>(
                $"/api/preguntas?categoriaId={ciencia.Id}&cantidad=100");

        Assert.NotNull(preguntas);
        Assert.Equal(2, preguntas.Count);
        Assert.All(
            preguntas,
            pregunta => Assert.Equal(ciencia.Id, pregunta.CategoriaId));
    }

    [Fact]
    public async Task Preguntas_CantidadCeroSeLimitaAUna()
    {
        List<PreguntaDto>? preguntas =
            await _cliente.GetFromJsonAsync<List<PreguntaDto>>(
                "/api/preguntas?cantidad=0");

        Assert.NotNull(preguntas);
        Assert.Single(preguntas);
    }

    [Fact]
    public async Task PreguntaInexistente_Devuelve404()
    {
        HttpResponseMessage respuesta =
            await _cliente.GetAsync("/api/preguntas/999999");

        Assert.Equal(HttpStatusCode.NotFound, respuesta.StatusCode);
    }
}

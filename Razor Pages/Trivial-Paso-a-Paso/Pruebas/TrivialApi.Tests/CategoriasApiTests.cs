using System.Net;
using System.Net.Http.Json;
using TrivialApi.DTOs;
using Xunit;
using Xunit.Abstractions;

namespace TrivialApi.Tests;

// Comprueba los endpoints de lectura de categorías.
public class CategoriasApiTests(
    CustomWebApplicationFactory aplicacion,
    ITestOutputHelper salida)
    : ApiTestBase(aplicacion, salida),
      IClassFixture<CustomWebApplicationFactory>
{
    private const string RutaCategorias = "/api/categorias";

    [Fact(DisplayName =
        "GET /api/categorias devuelve las categorías ordenadas")]
    public async Task ObtenerTodas_DevuelveCategoriasOrdenadas()
    {
        MostrarInicio("Obtener todas las categorías ordenadas por nombre");

        HttpResponseMessage respuesta = await EnviarGetAsync(RutaCategorias);
        List<CategoriaDto> categorias =
            await LeerJsonAsync<List<CategoriaDto>>(respuesta);

        MostrarComprobacion("Se reciben exactamente tres categorías");
        Assert.Equal(3, categorias.Count);

        MostrarComprobacion("El orden es Arte, Ciencia y Cultura");
        Assert.Collection(
            categorias,
            categoria => Assert.Equal("Arte", categoria.Nombre),
            categoria => Assert.Equal("Ciencia", categoria.Nombre),
            categoria => Assert.Equal("Cultura", categoria.Nombre));

        MostrarFin();
    }

    [Fact(DisplayName =
        "GET /api/categorias devuelve identificadores válidos y únicos")]
    public async Task ObtenerTodas_DevuelveIdentificadoresValidosYUnicos()
    {
        MostrarInicio("Comprobar los identificadores de las categorías");

        HttpResponseMessage respuesta = await EnviarGetAsync(RutaCategorias);
        List<CategoriaDto> categorias =
            await LeerJsonAsync<List<CategoriaDto>>(respuesta);

        MostrarComprobacion("Todos los identificadores son mayores que cero");
        Assert.All(categorias, categoria => Assert.True(categoria.Id > 0));

        MostrarComprobacion("No existen identificadores repetidos");
        Assert.Equal(
            categorias.Count,
            categorias.Select(categoria => categoria.Id).Distinct().Count());

        MostrarFin();
    }

    [Fact(DisplayName =
        "GET /api/categorias/{id} devuelve una categoría existente")]
    public async Task ObtenerPorId_CategoriaExistente_DevuelveCategoria()
    {
        MostrarInicio("Obtener una categoría mediante su identificador");

        HttpResponseMessage listado = await EnviarGetAsync(RutaCategorias);
        List<CategoriaDto> categorias =
            await LeerJsonAsync<List<CategoriaDto>>(listado);
        CategoriaDto ciencia =
            Assert.Single(categorias, categoria => categoria.Nombre == "Ciencia");

        HttpResponseMessage respuesta =
            await EnviarGetAsync($"{RutaCategorias}/{ciencia.Id}");
        CategoriaDto categoria = await LeerJsonAsync<CategoriaDto>(respuesta);

        MostrarComprobacion("La categoría recuperada es Ciencia");
        Assert.Equal(ciencia.Id, categoria.Id);
        Assert.Equal("Ciencia", categoria.Nombre);

        MostrarFin();
    }

    [Theory(DisplayName =
        "GET /api/categorias/{id} devuelve 404 para identificadores inexistentes")]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(999999)]
    public async Task ObtenerPorId_IdInexistente_Devuelve404(int id)
    {
        MostrarInicio($"Buscar la categoría inexistente con identificador {id}");

        HttpResponseMessage respuesta =
            await EnviarGetAsync($"{RutaCategorias}/{id}");

        MostrarComprobacion("La API devuelve 404 Not Found");
        Assert.Equal(HttpStatusCode.NotFound, respuesta.StatusCode);

        MostrarFin();
    }

    [Fact(DisplayName =
        "GET /api/categorias/texto devuelve 404")]
    public async Task ObtenerPorId_IdNoNumerico_Devuelve404()
    {
        MostrarInicio("Enviar un identificador de categoría no numérico");

        HttpResponseMessage respuesta =
            await EnviarGetAsync($"{RutaCategorias}/texto");

        MostrarComprobacion("La ruta no coincide y devuelve 404 Not Found");
        Assert.Equal(HttpStatusCode.NotFound, respuesta.StatusCode);

        MostrarFin();
    }

    [Fact(DisplayName =
        "GET /api/categorias devuelve contenido JSON")]
    public async Task ObtenerTodas_DevuelveContenidoJson()
    {
        MostrarInicio("Comprobar el tipo de contenido del listado de categorías");

        HttpResponseMessage respuesta = await EnviarGetAsync(RutaCategorias);

        MostrarComprobacion("El tipo de contenido es application/json");
        Assert.Equal(
            "application/json",
            respuesta.Content.Headers.ContentType?.MediaType);

        MostrarFin();
    }

    [Fact(DisplayName =
        "POST /api/categorias devuelve 405 Method Not Allowed")]
    public async Task CrearCategoria_MetodoNoPermitido_Devuelve405()
    {
        MostrarInicio("Intentar crear una categoría en una API de solo lectura");
        MostrarComprobacion("Se envía una petición POST no admitida");

        HttpResponseMessage respuesta =
            await Cliente.PostAsJsonAsync(RutaCategorias, new { nombre = "Nueva" });

        MostrarComprobacion(
            $"La API responde {(int)respuesta.StatusCode} {respuesta.StatusCode}");
        Assert.Equal(HttpStatusCode.MethodNotAllowed, respuesta.StatusCode);

        MostrarFin();
    }
}

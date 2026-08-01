using System.Net;
using Xunit;
using Xunit.Abstractions;

namespace TrivialApi.Tests;

[Collection(ApiTestCollection.Nombre)]
// Comprueba comportamientos generales del enrutamiento de la aplicación.
public class RutasApiTests(
    ApiServerFixture servidor,
    ITestOutputHelper salida)
    : ApiTestBase(servidor, salida)
{
    [Fact(DisplayName =
        "Una ruta de API inexistente devuelve 404 Not Found")]
    public async Task RutaInexistente_Devuelve404()
    {
        MostrarInicio("Solicitar una ruta de API que no está registrada");

        HttpResponseMessage respuesta =
            await EnviarGetAsync("/api/ruta-inexistente");

        MostrarComprobacion("La aplicación devuelve 404 Not Found");
        Assert.Equal(HttpStatusCode.NotFound, respuesta.StatusCode);

        MostrarFin();
    }

    [Fact(DisplayName =
        "La página principal de la aplicación sigue disponible")]
    public async Task PaginaPrincipal_Devuelve200()
    {
        MostrarInicio("Comprobar que las pruebas no impiden servir Razor Pages");

        HttpResponseMessage respuesta = await EnviarGetAsync("/");

        MostrarComprobacion("La página principal devuelve 200 OK");
        Assert.Equal(HttpStatusCode.OK, respuesta.StatusCode);

        MostrarFin();
    }
}

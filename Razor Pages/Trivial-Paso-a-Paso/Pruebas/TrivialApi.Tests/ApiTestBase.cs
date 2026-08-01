using System.Net.Http.Json;
using Xunit;
using Xunit.Abstractions;

namespace TrivialApi.Tests;

// Contiene la infraestructura común de las clases de pruebas de la API.
public abstract class ApiTestBase
{
    private readonly ITestOutputHelper _salida;

    protected ApiTestBase(
        CustomWebApplicationFactory aplicacion,
        ITestOutputHelper salida)
    {
        Cliente = aplicacion.CreateClient();
        _salida = salida;
    }

    protected HttpClient Cliente { get; }

    // Muestra el objetivo de la prueba antes de realizar la petición.
    protected void MostrarInicio(string descripcion)
    {
        _salida.WriteLine("");
        _salida.WriteLine(new string('=', 70));
        _salida.WriteLine($"INICIO: {descripcion}");
    }

    // Realiza una petición GET y muestra la ruta y el código HTTP recibido.
    protected async Task<HttpResponseMessage> EnviarGetAsync(string ruta)
    {
        _salida.WriteLine($"PETICIÓN: GET {ruta}");

        HttpResponseMessage respuesta = await Cliente.GetAsync(ruta);

        _salida.WriteLine(
            $"RESPUESTA: {(int)respuesta.StatusCode} {respuesta.StatusCode}");

        if (respuesta.Content.Headers.ContentType is not null)
        {
            _salida.WriteLine(
                $"CONTENIDO: {respuesta.Content.Headers.ContentType.MediaType}");
        }

        return respuesta;
    }

    // Deserializa el JSON después de comprobar que la petición ha sido correcta.
    protected async Task<T> LeerJsonAsync<T>(HttpResponseMessage respuesta)
    {
        respuesta.EnsureSuccessStatusCode();

        T? contenido = await respuesta.Content.ReadFromJsonAsync<T>();
        return Assert.IsType<T>(contenido);
    }

    // Muestra una comprobación relevante realizada por la prueba.
    protected void MostrarComprobacion(string descripcion)
    {
        _salida.WriteLine($"COMPROBACIÓN: {descripcion}");
    }

    // Indica que todas las aserciones de la prueba han finalizado correctamente.
    protected void MostrarFin()
    {
        _salida.WriteLine("RESULTADO: prueba superada");
    }
}

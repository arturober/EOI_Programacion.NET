using System.Net.Http.Json;
using TrivialApi.Testing;
using Xunit;
using Xunit.Abstractions;

namespace TrivialApi.Tests;

// Contiene las operaciones comunes de las pruebas HTTP.
public abstract class ApiTestBase
{
    private readonly InformeConsola _informe;

    protected ApiTestBase(ApiServerFixture servidor, ITestOutputHelper salida)
    {
        Cliente = servidor.Server.Client;
        _informe = new InformeConsola(salida.WriteLine);
    }

    protected HttpClient Cliente { get; }

    protected void MostrarInicio(string descripcion) =>
        _informe.Inicio(descripcion);

    protected async Task<HttpResponseMessage> EnviarGetAsync(string ruta)
    {
        _informe.Peticion("GET", ruta);
        HttpResponseMessage respuesta = await Cliente.GetAsync(ruta);
        _informe.Respuesta((int)respuesta.StatusCode, respuesta.StatusCode.ToString());

        if (respuesta.Content.Headers.ContentType?.MediaType is string tipo)
        {
            _informe.Paso($"Contenido recibido: {tipo}");
        }

        return respuesta;
    }

    protected static async Task<T> LeerJsonAsync<T>(HttpResponseMessage respuesta)
    {
        respuesta.EnsureSuccessStatusCode();
        T? contenido = await respuesta.Content.ReadFromJsonAsync<T>();
        return Assert.IsType<T>(contenido);
    }

    protected void MostrarComprobacion(string descripcion) =>
        _informe.Comprobacion(descripcion);

    protected void MostrarFin() => _informe.Exito();
}

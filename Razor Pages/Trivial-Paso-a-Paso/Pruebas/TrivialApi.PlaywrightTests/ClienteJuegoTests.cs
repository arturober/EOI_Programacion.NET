using System.Text.Json;
using Microsoft.Playwright;
using Xunit;
using Xunit.Abstractions;
using static Microsoft.Playwright.Assertions;

namespace TrivialApi.PlaywrightTests;

[Collection(PlaywrightTestCollection.Nombre)]
public sealed class ClienteJuegoTests(
    PlaywrightFixture aplicacion,
    ITestOutputHelper salida)
    : PlaywrightTestBase(aplicacion, salida)
{
    [Fact(DisplayName = "Playwright: el cliente carga categorías desde la API real")]
    public async Task Cliente_CargaCategoriasReales()
    {
        Informe.Inicio("Abrir el juego y consultar la API real de categorías");
        await AbrirAsync("/cliente/index.html");

        ILocator opciones = Page.Locator("#categoria option");
        await Expect(opciones).ToHaveCountAsync(4);
        Informe.Comprobacion("El selector contiene el texto inicial y tres categorías");
        Informe.Exito();
    }

    [Fact(DisplayName = "Playwright: al empezar se oculta la conexión y aparece la pregunta")]
    public async Task EmpezarJuego_MuestraPrimeraPregunta()
    {
        Informe.Inicio("Iniciar una partida con respuestas controladas");
        await PrepararPartidaAsync(DosPreguntas());
        await AbrirAsync("/cliente/index.html");

        await Page.Locator("#categoria").SelectOptionAsync("1");
        await Page.Locator("#jugar").ClickAsync();

        Informe.Comprobacion("La zona inicial se oculta y se muestran cuatro respuestas");
        await Expect(Page.Locator("#inicio")).ToBeHiddenAsync();
        await Expect(Page.Locator("#juego")).ToBeVisibleAsync();
        await Expect(Page.Locator("#progreso")).ToHaveTextAsync("Pregunta 1 de 2");
        await Expect(Page.Locator("#respuestas button")).ToHaveCountAsync(4);
        Informe.Exito();
    }

    [Fact(DisplayName = "Playwright: una respuesta incorrecta muestra la respuesta correcta")]
    public async Task RespuestaIncorrecta_MuestraExplicacion()
    {
        Informe.Inicio("Responder mal y comprobar el mensaje de SweetAlert");
        await PrepararPartidaAsync(DosPreguntas());
        await AbrirAsync("/cliente/index.html");
        await Page.Locator("#jugar").ClickAsync();

        await Page.GetByRole(AriaRole.Button, new() { Name = "Madrid" }).ClickAsync();

        Informe.Comprobacion("El aviso incluye la pregunta y la respuesta correcta");
        await Expect(Page.Locator(".swal2-title")).ToHaveTextAsync("¡Has fallado!");
        await Expect(Page.Locator(".swal2-html-container")).ToContainTextAsync("París");
        await Expect(Page.Locator(".swal2-html-container")).ToContainTextAsync("capital de Francia");
        Informe.Exito();
    }

    [Fact(DisplayName = "Playwright: una respuesta correcta avanza a la siguiente pregunta")]
    public async Task RespuestaCorrecta_AvanzaPregunta()
    {
        Informe.Inicio("Acertar la primera pregunta y continuar");
        await PrepararPartidaAsync(DosPreguntas());
        await AbrirAsync("/cliente/index.html");
        await Page.Locator("#jugar").ClickAsync();

        await Page.GetByRole(AriaRole.Button, new() { Name = "París" }).ClickAsync();
        await Expect(Page.Locator(".swal2-title")).ToHaveTextAsync("¡Correcto!");
        await Page.Locator(".swal2-confirm").ClickAsync();

        Informe.Comprobacion("El progreso cambia a la pregunta 2 de 2");
        await Expect(Page.Locator("#progreso")).ToHaveTextAsync("Pregunta 2 de 2");
        Informe.Exito();
    }

    [Fact(DisplayName = "Playwright: al terminar se muestra el resultado y se puede volver a jugar")]
    public async Task FinalPartida_MuestraResultadoYVuelveAlInicio()
    {
        Informe.Inicio("Completar una partida de una sola pregunta");
        await PrepararPartidaAsync([DosPreguntas()[0]]);
        await AbrirAsync("/cliente/index.html");
        await Page.Locator("#jugar").ClickAsync();

        await Page.GetByRole(AriaRole.Button, new() { Name = "París" }).ClickAsync();
        await Page.Locator(".swal2-confirm").ClickAsync();

        Informe.Comprobacion("El resumen indica un acierto de una pregunta");
        await Expect(Page.Locator(".swal2-title")).ToHaveTextAsync("Partida terminada");
        await Expect(Page.Locator(".swal2-html-container")).ToContainTextAsync("1 de 1");
        await Page.Locator(".swal2-confirm").ClickAsync();

        await Expect(Page.Locator("#inicio")).ToBeVisibleAsync();
        await Expect(Page.Locator("#juego")).ToBeHiddenAsync();
        Informe.Exito();
    }

    private async Task PrepararPartidaAsync(object[] preguntas)
    {
        string categorias = JsonSerializer.Serialize(new[]
        {
            new { id = 1, nombre = "General" }
        });
        string cuerpoPreguntas = JsonSerializer.Serialize(preguntas);

        await Page.RouteAsync("**/api/categorias", async ruta =>
            await ruta.FulfillAsync(new RouteFulfillOptions
            {
                Status = 200,
                ContentType = "application/json",
                Body = categorias
            }));

        await Page.RouteAsync("**/api/preguntas**", async ruta =>
            await ruta.FulfillAsync(new RouteFulfillOptions
            {
                Status = 200,
                ContentType = "application/json",
                Body = cuerpoPreguntas
            }));
    }

    private static object[] DosPreguntas()
    {
        return
        [
            new
            {
                id = 101,
                enunciado = "¿Cuál es la capital de Francia?",
                respuestas = new[] { "París", "Madrid", "Roma", "Lisboa" },
                respuestaCorrecta = 1,
                categoria = new { id = 1, nombre = "General" }
            },
            new
            {
                id = 102,
                enunciado = "¿Cuánto es dos más dos?",
                respuestas = new[] { "Tres", "Cinco", "Cuatro", "Seis" },
                respuestaCorrecta = 3,
                categoria = new { id = 1, nombre = "General" }
            }
        ];
    }
}

using Microsoft.Playwright;
using Xunit;
using Xunit.Abstractions;
using static Microsoft.Playwright.Assertions;

namespace TrivialApi.PlaywrightTests;

[Collection(PlaywrightTestCollection.Nombre)]
public sealed class TemasTests(
    PlaywrightFixture aplicacion,
    ITestOutputHelper salida)
    : PlaywrightTestBase(aplicacion, salida)
{
    [Fact(DisplayName = "Playwright: el tema oscuro se aplica y persiste")]
    public async Task TemaOscuro_SeGuardaEnLocalStorage()
    {
        Informe.Inicio("Seleccionar Bootstrap oscuro y recargar la página");
        await AbrirAsync("/");

        await Page.Locator("#selectorTema").SelectOptionAsync("bootstrap-dark");
        await Expect(Page.Locator("html")).ToHaveAttributeAsync("data-bs-theme", "dark");

        string? guardado = await Page.EvaluateAsync<string?>(
            "localStorage.getItem('temaTrivial')");
        Assert.Equal("bootstrap-dark", guardado);

        Informe.Paso("Recargando para comprobar la persistencia");
        await Page.ReloadAsync();
        await Expect(Page.Locator("html")).ToHaveAttributeAsync("data-bs-theme", "dark");
        Informe.Exito();
    }

    [Fact(DisplayName = "Playwright: un tema Bootswatch cambia la hoja de estilos")]
    public async Task Bootswatch_CambiaLaHojaDeEstilos()
    {
        Informe.Inicio("Seleccionar el tema Bootswatch Darkly");
        await AbrirAsync("/");

        await Page.Locator("#selectorTema").SelectOptionAsync("bootswatch-darkly");
        ILocator hoja = Page.Locator("#temaCss");

        Informe.Comprobacion("La dirección de la hoja contiene el tema darkly");
        await Expect(hoja).ToHaveAttributeAsync(
            "href",
            new System.Text.RegularExpressions.Regex(
                "darkly/bootstrap\\.min\\.css"));

        string? guardado = await Page.EvaluateAsync<string?>(
            "localStorage.getItem('temaTrivial')");
        Assert.Equal("bootswatch-darkly", guardado);
        Informe.Exito();
    }

    [Fact(DisplayName = "Playwright: el cliente comparte el tema guardado")]
    public async Task ClienteJuego_AplicaElTemaDeLaAdministracion()
    {
        Informe.Inicio("Guardar el tema oscuro y abrir el cliente del juego");
        await AbrirAsync("/");
        await Page.Locator("#selectorTema").SelectOptionAsync("bootstrap-dark");

        await AbrirAsync("/cliente/index.html");
        Informe.Comprobacion("El cliente también utiliza data-bs-theme=dark");
        await Expect(Page.Locator("html")).ToHaveAttributeAsync("data-bs-theme", "dark");
        Informe.Exito();
    }
}

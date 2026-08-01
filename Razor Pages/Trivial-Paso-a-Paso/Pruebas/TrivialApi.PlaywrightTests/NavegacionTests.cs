using Microsoft.Playwright;
using Xunit;
using Xunit.Abstractions;
using static Microsoft.Playwright.Assertions;

namespace TrivialApi.PlaywrightTests;

[Collection(PlaywrightTestCollection.Nombre)]
public sealed class NavegacionTests(
    PlaywrightFixture aplicacion,
    ITestOutputHelper salida)
    : PlaywrightTestBase(aplicacion, salida)
{
    [Fact(DisplayName = "Playwright: la página principal y la navegación cargan")]
    public async Task PaginaPrincipal_MuestraNavegacionPrincipal()
    {
        Informe.Inicio("Abrir la administración y comprobar sus enlaces principales");
        await AbrirAsync("/");

        Informe.Comprobacion("La página contiene enlaces a categorías, preguntas y juego");
        ILocator menu = Page.Locator("#menuPrincipal");
        await Expect(menu.Locator("a[href='/Categorias']")).ToBeVisibleAsync();
        await Expect(menu.Locator("a[href='/Preguntas']")).ToBeVisibleAsync();
        await Expect(menu.Locator("a[href='/cliente/index.html']")).ToBeVisibleAsync();
        Informe.Exito();
    }

    [Fact(DisplayName = "Playwright: los enlaces de la barra abren cada sección")]
    public async Task BarraNavegacion_AbreCategoriasYPreguntas()
    {
        Informe.Inicio("Recorrer las páginas principales desde la barra de navegación");
        await AbrirAsync("/");

        Informe.Paso("Abriendo Categorías");
        await Page.Locator("#menuPrincipal a[href='/Categorias']").ClickAsync();
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Categorías" })).ToBeVisibleAsync();

        Informe.Paso("Abriendo Preguntas");
        await Page.Locator("#menuPrincipal a[href='/Preguntas']").ClickAsync();
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Preguntas" })).ToBeVisibleAsync();
        Informe.Exito();
    }

    [Fact(DisplayName = "Playwright: la barra de navegación es sticky")]
    public async Task BarraNavegacion_PermaneceFijaArriba()
    {
        Informe.Inicio("Comprobar la posición sticky de la barra de navegación");
        await AbrirAsync("/Preguntas");

        ILocator barra = Page.Locator("nav.navbar");
        string? posicion = await barra.EvaluateAsync<string>(
            "elemento => getComputedStyle(elemento).position");
        double parteSuperior = await barra.EvaluateAsync<double>(
            "elemento => parseFloat(getComputedStyle(elemento).top) || 0");

        Informe.Comprobacion("La barra utiliza position: sticky y top: 0");
        Assert.Equal("sticky", posicion);
        Assert.Equal(0, parteSuperior);
        Informe.Exito();
    }

    [Fact(DisplayName = "Playwright: el menú hamburguesa funciona en móvil")]
    public async Task MenuMovil_SeExpandeAlPulsarElBoton()
    {
        Informe.Inicio("Comprobar la navegación con una anchura de teléfono");
        await Page.SetViewportSizeAsync(390, 844);
        await AbrirAsync("/");

        ILocator boton = Page.Locator("button.navbar-toggler");
        await Expect(boton).ToBeVisibleAsync();
        await boton.ClickAsync();

        Informe.Comprobacion("Los enlaces quedan visibles tras desplegar el menú");
        ILocator menu = Page.Locator("#menuPrincipal");
        await Expect(menu.Locator("a[href='/Categorias']")).ToBeVisibleAsync();
        await Expect(menu.Locator("a[href='/Preguntas']")).ToBeVisibleAsync();
        Informe.Exito();
    }

    [Theory(DisplayName = "Playwright: las páginas principales no desbordan horizontalmente en móvil")]
    [InlineData("/")]
    [InlineData("/Categorias")]
    [InlineData("/Preguntas")]
    [InlineData("/cliente/index.html")]
    public async Task PaginaMovil_NoTieneDesbordamientoHorizontal(string ruta)
    {
        Informe.Inicio($"Comprobar el diseño adaptable de {ruta}");
        await Page.SetViewportSizeAsync(390, 844);
        await AbrirAsync(ruta);

        bool desborda = await Page.EvaluateAsync<bool>(
            "document.documentElement.scrollWidth > document.documentElement.clientWidth + 1");

        Informe.Comprobacion("El ancho del documento no supera el ancho visible");
        Assert.False(desborda);
        Informe.Exito();
    }
}

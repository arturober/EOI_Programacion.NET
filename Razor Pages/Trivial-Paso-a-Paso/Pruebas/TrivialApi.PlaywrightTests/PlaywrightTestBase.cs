using Microsoft.Playwright;
using Microsoft.Playwright.Xunit;
using TrivialApi.Testing;
using Xunit.Abstractions;

namespace TrivialApi.PlaywrightTests;

// Proporciona la página del navegador, la dirección del servidor y el informe.
public abstract class PlaywrightTestBase : PageTest
{
    protected PlaywrightTestBase(
        PlaywrightFixture aplicacion,
        ITestOutputHelper salida)
    {
        Aplicacion = aplicacion;
        Informe = new InformeConsola(salida.WriteLine);
    }

    protected PlaywrightFixture Aplicacion { get; }

    protected InformeConsola Informe { get; }

    public override BrowserNewContextOptions ContextOptions()
    {
        return new BrowserNewContextOptions
        {
            Locale = "es-ES",
            ViewportSize = new ViewportSize
            {
                Width = 1280,
                Height = 900
            }
        };
    }

    protected async Task AbrirAsync(string ruta)
    {
        Informe.Paso($"Abriendo {ruta}");

        IResponse? respuesta = await Page.GotoAsync(
            new Uri(Aplicacion.Server.BaseAddress, ruta).ToString(),
            new PageGotoOptions
            {
                WaitUntil = WaitUntilState.DOMContentLoaded
            });

        if (respuesta is not null)
        {
            Informe.Respuesta(respuesta.Status, respuesta.StatusText);
        }
    }

    protected async Task CerrarSweetAlertSiExisteAsync()
    {
        ILocator confirmar = Page.Locator(".swal2-confirm");

        try
        {
            await confirmar.WaitForAsync(new LocatorWaitForOptions
            {
                State = WaitForSelectorState.Visible,
                Timeout = 2_000
            });
            await confirmar.ClickAsync();
        }
        catch (TimeoutException)
        {
            // Algunas terminales o redes pueden tardar en cargar el CDN.
            // Si no aparece el aviso, la prueba continúa con la página resultante.
        }
    }

    protected static string NombreUnico(string prefijo) =>
        $"{prefijo} {Guid.NewGuid():N}"[..Math.Min(prefijo.Length + 9, prefijo.Length + 33)];
}

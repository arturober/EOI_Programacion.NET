using TrivialApi.Testing;
using Xunit;

namespace TrivialApi.PlaywrightTests;

// Instala Chromium cuando todavía no está disponible e inicia la API real.
// La instalación es la proporcionada por Playwright, sin scripts propios.
public sealed class PlaywrightFixture : IAsyncLifetime
{
    public TrivialTestServer Server { get; } = new();

    public async Task InitializeAsync()
    {
        int resultado = Microsoft.Playwright.Program.Main(["install", "chromium"]);

        if (resultado != 0)
        {
            throw new InvalidOperationException(
                "Playwright no ha podido instalar o localizar Chromium.");
        }

        await Server.StartAsync();
    }

    public async Task DisposeAsync() => await Server.DisposeAsync();
}

[CollectionDefinition("Navegador Playwright", DisableParallelization = true)]
public sealed class PlaywrightTestCollection : ICollectionFixture<PlaywrightFixture>
{
    public const string Nombre = "Navegador Playwright";
}

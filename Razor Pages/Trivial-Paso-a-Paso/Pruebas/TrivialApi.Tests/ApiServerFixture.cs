using TrivialApi.Testing;
using Xunit;

namespace TrivialApi.Tests;

// xUnit crea una instancia para toda la colección de pruebas de la API.
public sealed class ApiServerFixture : IAsyncLifetime
{
    public TrivialTestServer Server { get; } = new();

    public Task InitializeAsync() => Server.StartAsync();

    public async Task DisposeAsync() => await Server.DisposeAsync();
}

[CollectionDefinition("API de Trivial", DisableParallelization = true)]
public sealed class ApiTestCollection : ICollectionFixture<ApiServerFixture>
{
    public const string Nombre = "API de Trivial";
}

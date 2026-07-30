using OpenFoodFacts.Modelos;

namespace OpenFoodFacts.Servicios;

// Define las operaciones externas disponibles para las Razor Pages.
public interface IOpenFoodFactsServicio
{
    Task<ResultadoProductos> ObtenerDestacadosAsync(
        int pagina = 1,
        CancellationToken cancellationToken = default);

    Task<ResultadoProductos> BuscarAsync(
        string texto,
        int pagina = 1,
        CancellationToken cancellationToken = default);

    Task<ResultadoProductos> FiltrarCategoriaAsync(
        string categoria,
        int pagina = 1,
        CancellationToken cancellationToken = default);

    Task<ResultadoProductos> FiltrarNutriScoreAsync(
        string puntuacion,
        int pagina = 1,
        CancellationToken cancellationToken = default);

    Task<ProductoDetalle> ObtenerProductoAsync(
        string codigo,
        CancellationToken cancellationToken = default);
}

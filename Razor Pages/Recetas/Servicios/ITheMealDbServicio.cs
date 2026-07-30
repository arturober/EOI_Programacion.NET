using Recetas.Modelos;

namespace Recetas.Servicios;

// Permite consumir TheMealDB sin que las páginas conozcan HttpClient.
public interface ITheMealDbServicio
{
    Task<IReadOnlyList<RecetaResumen>> BuscarAsync(
        string texto,
        CancellationToken cancellationToken = default);

    Task<RecetaDetalle> ObtenerDetalleAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<RecetaDetalle> ObtenerAleatoriaAsync(
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<RecetaResumen>> FiltrarCategoriaAsync(
        string categoria,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<RecetaResumen>> FiltrarAreaAsync(
        string area,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<CategoriaReceta>> ObtenerCategoriasAsync(
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<string>> ObtenerAreasAsync(
        CancellationToken cancellationToken = default);
}

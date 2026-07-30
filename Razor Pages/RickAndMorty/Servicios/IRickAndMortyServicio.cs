using RickAndMorty.DTOs;

namespace RickAndMorty.Servicios;

// Define las consultas externas disponibles para los PageModel.
public interface IRickAndMortyServicio
{
    Task<PaginaApiDto<PersonajeDto>> BuscarPersonajesAsync(
        string? nombre,
        string? estado,
        string? especie,
        string? genero,
        int pagina,
        CancellationToken cancellationToken = default);

    Task<PersonajeDto> ObtenerPersonajeAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<PaginaApiDto<EpisodioDto>> BuscarEpisodiosAsync(
        string? nombre,
        string? codigo,
        int pagina,
        CancellationToken cancellationToken = default);

    Task<EpisodioDto> ObtenerEpisodioAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<PaginaApiDto<LocalizacionDto>> BuscarLocalizacionesAsync(
        string? nombre,
        string? tipo,
        string? dimension,
        int pagina,
        CancellationToken cancellationToken = default);

    Task<LocalizacionDto> ObtenerLocalizacionAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<EpisodioDto>> ObtenerEpisodiosPorUrlsAsync(
        IEnumerable<string> urls,
        int maximo = 60,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<PersonajeDto>> ObtenerPersonajesPorUrlsAsync(
        IEnumerable<string> urls,
        int maximo = 40,
        CancellationToken cancellationToken = default);
}

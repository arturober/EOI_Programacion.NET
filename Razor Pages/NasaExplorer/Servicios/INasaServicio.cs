using NasaExplorer.DTOs;

namespace NasaExplorer.Servicios;

// Agrupa las operaciones que ofrecen las distintas APIs oficiales del portal.
public interface INasaServicio
{
    Task<ApodDto> ObtenerApodAsync(DateOnly fecha);

    Task<MediaRespuestaDto> BuscarMultimediaAsync(
        string busqueda,
        string tipo,
        int? anioDesde,
        int? anioHasta,
        int pagina);

    Task<MediaItemDto?> ObtenerMultimediaAsync(string nasaId);

    Task<List<string>> ObtenerArchivosMultimediaAsync(string nasaId);

    Task<List<EpicImagenDto>> ObtenerEpicAsync(string coleccion, DateOnly? fecha);

    Task<List<EonetEventoDto>> ObtenerEventosNaturalesAsync(
        string estado,
        string? categoria,
        int dias);

    Task<AsteroidesResultado> ObtenerAsteroidesAsync(
        DateOnly fechaInicio,
        DateOnly fechaFin);

    Task<List<DonkiEventoVista>> ObtenerClimaEspacialAsync(
        string tipo,
        DateOnly fechaInicio,
        DateOnly fechaFin);

    Task<List<ExoplanetaDto>> BuscarExoplanetasAsync(
        string? busqueda,
        string? metodo,
        int? anioDesde,
        int limite);
}

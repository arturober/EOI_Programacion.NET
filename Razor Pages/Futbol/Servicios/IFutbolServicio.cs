using Futbol.DTOs;

namespace Futbol.Servicios;

// Define las operaciones externas que puede utilizar la interfaz.
public interface IFutbolServicio
{
    bool EstaConfigurada { get; }

    Task<IReadOnlyList<CompeticionDto>> ObtenerCompeticionesAsync(
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<PartidoDto>> ObtenerPartidosPorFechaAsync(
        DateOnly fecha,
        CancellationToken cancellationToken = default);

    Task<ClasificacionRespuestaDto> ObtenerClasificacionAsync(
        string codigo,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<PartidoDto>> ObtenerPartidosCompeticionAsync(
        string codigo,
        CancellationToken cancellationToken = default);

    Task<GoleadoresRespuestaDto> ObtenerGoleadoresAsync(
        string codigo,
        CancellationToken cancellationToken = default);

    Task<EquiposRespuestaDto> ObtenerEquiposAsync(
        string codigo,
        CancellationToken cancellationToken = default);

    Task<EquipoDetalleDto> ObtenerEquipoAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<PartidoDto>> ObtenerPartidosEquipoAsync(
        int id,
        string estado,
        CancellationToken cancellationToken = default);
}

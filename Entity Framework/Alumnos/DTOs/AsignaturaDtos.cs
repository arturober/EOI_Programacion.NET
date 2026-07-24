using System.ComponentModel.DataAnnotations;

namespace Alumnos.DTOs;

public record AsignaturaSummaryDto(
    Guid Id,
    string Nombre,
    string Codigo,
    int Creditos,
    int AlumnosCount = 0
);

public record AsignaturaDetailDto(
    Guid Id,
    string Nombre,
    string Codigo,
    int Creditos,
    IReadOnlyList<AlumnoSummaryDto> Alumnos
);

public record CreateAsignaturaInput
{
    [Required(ErrorMessage = "El nombre de la asignatura es obligatorio.")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 100 caracteres.")]
    public required string Nombre { get; init; }

    [Required(ErrorMessage = "El código es obligatorio.")]
    [StringLength(20, MinimumLength = 2, ErrorMessage = "El código debe tener entre 2 y 20 caracteres.")]
    public required string Codigo { get; init; }

    [Range(1, 60, ErrorMessage = "Los créditos deben estar entre 1 y 60.")]
    public int Creditos { get; init; } = 6;
}

public record UpdateAsignaturaInput
{
    [Required(ErrorMessage = "El nombre de la asignatura es obligatorio.")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 100 caracteres.")]
    public required string Nombre { get; init; }

    [Required(ErrorMessage = "El código es obligatorio.")]
    [StringLength(20, MinimumLength = 2, ErrorMessage = "El código debe tener entre 2 y 20 caracteres.")]
    public required string Codigo { get; init; }

    [Range(1, 60, ErrorMessage = "Los créditos deben estar entre 1 y 60.")]
    public int Creditos { get; init; }
}

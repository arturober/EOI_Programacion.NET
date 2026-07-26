using System.ComponentModel.DataAnnotations;

namespace Alumnos.DTOs;

public record AlumnoSummaryDto(
    Guid Id,
    string Nombre,
    string Email,
    string Dni,
    int AsignaturasCount = 0
);

public record AlumnoDetailDto(
    Guid Id,
    string Nombre,
    string Email,
    string Dni,
    IReadOnlyList<AsignaturaSummaryDto> Asignaturas
);

public record CreateAlumnoInput
{
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 100 caracteres.")]
    public required string Nombre { get; init; }

    [Required(ErrorMessage = "El email es obligatorio.")]
    [EmailAddress(ErrorMessage = "Formato de email no válido.")]
    public required string Email { get; init; }

    [Required(ErrorMessage = "El DNI es obligatorio.")]
    [RegularExpression(@"^[0-9]{8}[A-Za-z]$", ErrorMessage = "El DNI debe contener 8 números y 1 letra.")]
    public required string Dni { get; init; }
}

public record UpdateAlumnoInput
{
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 100 caracteres.")]
    public required string Nombre { get; init; }

    [Required(ErrorMessage = "El email es obligatorio.")]
    [EmailAddress(ErrorMessage = "Formato de email no válido.")]
    public required string Email { get; init; }

    [Required(ErrorMessage = "El DNI es obligatorio.")]
    [RegularExpression(@"^[0-9]{8}[A-Za-z]$", ErrorMessage = "El DNI debe contener 8 números y 1 letra.")]
    public required string Dni { get; init; }
}

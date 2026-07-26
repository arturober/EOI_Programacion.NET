using System.ComponentModel.DataAnnotations;

namespace Equipos.DTOs;

public record JugadorDto(
    Guid Id, 
    string Nickname, 
    string NombreCompleto, 
    string Rol, 
    Guid EquipoId, 
    string NombreEquipo);

public record CreateJugadorInput
{
    [Required(ErrorMessage = "El Nickname es obligatorio.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "El Nickname debe tener entre 2 y 50 caracteres.")]
    public required string Nickname { get; init; }

    [Required(ErrorMessage = "El nombre completo es obligatorio.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 100 caracteres.")]
    public required string NombreCompleto { get; init; }

    [Required(ErrorMessage = "El rol es obligatorio.")]
    [StringLength(50, ErrorMessage = "El rol no puede exceder 50 caracteres.")]
    public required string Rol { get; init; }

    [Required(ErrorMessage = "Debe seleccionar un equipo.")]
    public Guid EquipoId { get; init; }
}

public record UpdateJugadorInput
{
    [Required]
    public Guid Id { get; init; }

    [Required(ErrorMessage = "El Nickname es obligatorio.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "El Nickname debe tener entre 2 y 50 caracteres.")]
    public required string Nickname { get; init; }

    [Required(ErrorMessage = "El nombre completo es obligatorio.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 100 caracteres.")]
    public required string NombreCompleto { get; init; }

    [Required(ErrorMessage = "El rol es obligatorio.")]
    [StringLength(50, ErrorMessage = "El rol no puede exceder 50 caracteres.")]
    public required string Rol { get; init; }

    [Required(ErrorMessage = "Debe seleccionar un equipo.")]
    public Guid EquipoId { get; init; }
}

using System.ComponentModel.DataAnnotations;

namespace Equipos.DTOs;

public record EquipoDto(
    Guid Id, 
    string Nombre, 
    string Juego, 
    string? LogoUrl, 
    DateTime FechaCreacion, 
    int CantidadJugadores);

public record EquipoDetailDto(
    Guid Id, 
    string Nombre, 
    string Juego, 
    string? LogoUrl, 
    DateTime FechaCreacion, 
    IReadOnlyCollection<JugadorDto> Jugadores);

public record CreateEquipoInput
{
    [Required(ErrorMessage = "El nombre del equipo es obligatorio.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 100 caracteres.")]
    public required string Nombre { get; init; }

    [Required(ErrorMessage = "El nombre del juego es obligatorio.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "El juego debe tener entre 2 y 100 caracteres.")]
    public required string Juego { get; init; }

    [Url(ErrorMessage = "Debe proporcionar una URL de imagen válida.")]
    [StringLength(500, ErrorMessage = "La URL no puede exceder los 500 caracteres.")]
    public string? LogoUrl { get; init; }
}

public record UpdateEquipoInput
{
    [Required]
    public Guid Id { get; init; }

    [Required(ErrorMessage = "El nombre del equipo es obligatorio.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 100 caracteres.")]
    public required string Nombre { get; init; }

    [Required(ErrorMessage = "El nombre del juego es obligatorio.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "El juego debe tener entre 2 y 100 caracteres.")]
    public required string Juego { get; init; }

    [Url(ErrorMessage = "Debe proporcionar una URL de imagen válida.")]
    [StringLength(500, ErrorMessage = "La URL no puede exceder los 500 caracteres.")]
    public string? LogoUrl { get; init; }
}

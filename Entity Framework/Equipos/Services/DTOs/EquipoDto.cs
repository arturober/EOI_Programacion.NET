using System.ComponentModel.DataAnnotations;

namespace Equipos.Services.DTOs;

public record EquipoDto (int Id, string Nombre, int NumJugadores);
public record EquipoJugadoresDto (int Id, string Nombre, IReadOnlyList<JugadorDto> Jugadores);
public record CrearEquipoInput(
    [Required(ErrorMessage = "El nombre del equipo es obligatorio.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 100 caracteres.")]
    string Nombre
);

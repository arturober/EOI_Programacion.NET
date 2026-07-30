using System.ComponentModel.DataAnnotations;

namespace Futbol.Modelos;

// Guarda una copia mínima del equipo para mostrar favoritos sin consumir la API.
public class EquipoFavorito
{
    public int Id { get; set; }

    [Required]
    public string UsuarioId { get; set; } = "";

    public Usuario? Usuario { get; set; }

    public int EquipoId { get; set; }

    [Required, StringLength(150)]
    public string Nombre { get; set; } = "";

    [StringLength(100)]
    public string NombreCorto { get; set; } = "";

    [StringLength(500)]
    public string? EscudoUrl { get; set; }

    [StringLength(150)]
    public string? Competicion { get; set; }

    public DateTime GuardadoUtc { get; set; } = DateTime.UtcNow;
}

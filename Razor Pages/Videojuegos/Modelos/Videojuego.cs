using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Videojuegos.Modelos;

// Guarda una copia breve de RAWG para mostrar la biblioteca sin llamar a la API.
public class Videojuego
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int RawgId { get; set; }

    [Required]
    [StringLength(300)]
    public string Nombre { get; set; } = "";

    [StringLength(300)]
    public string Slug { get; set; } = "";

    [StringLength(1000)]
    public string? ImagenUrl { get; set; }

    public DateTime? FechaLanzamiento { get; set; }
    public double? PuntuacionRawg { get; set; }
    public int? Metacritic { get; set; }
    public DateTime ActualizadoUtc { get; set; }

    public ICollection<VideojuegoUsuario> Usuarios { get; set; } = [];
}

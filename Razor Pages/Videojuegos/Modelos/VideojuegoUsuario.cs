using System.ComponentModel.DataAnnotations;

namespace Videojuegos.Modelos;

// Relaciona un usuario con un videojuego y sus datos personales.
public class VideojuegoUsuario
{
    public string UsuarioId { get; set; } = "";
    public Usuario Usuario { get; set; } = null!;

    public int VideojuegoId { get; set; }
    public Videojuego Videojuego { get; set; } = null!;

    public EstadoVideojuego Estado { get; set; } = EstadoVideojuego.Pendiente;

    [Range(1, 10)]
    public int? PuntuacionPersonal { get; set; }

    [StringLength(500)]
    public string Comentario { get; set; } = "";

    public DateTime FechaAgregadoUtc { get; set; }
    public DateTime FechaActualizadoUtc { get; set; }
}

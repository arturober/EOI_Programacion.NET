using System.ComponentModel.DataAnnotations;

namespace NasaExplorer.Modelos;

// Un favorito genérico sirve para APOD, multimedia, eventos y exoplanetas.
public class Favorito
{
    public int Id { get; set; }

    [Required]
    public string UsuarioId { get; set; } = string.Empty;

    [Required, MaxLength(40)]
    public string Tipo { get; set; } = string.Empty;

    [Required, MaxLength(250)]
    public string Referencia { get; set; } = string.Empty;

    [Required, MaxLength(300)]
    public string Titulo { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Subtitulo { get; set; }

    [MaxLength(2000)]
    public string? ImagenUrl { get; set; }

    [MaxLength(2000)]
    public string? UrlDetalle { get; set; }

    public DateTime GuardadoUtc { get; set; } = DateTime.UtcNow;

    // Esta navegación permite llegar al propietario desde Entity Framework.
    public Usuario? Usuario { get; set; }
}

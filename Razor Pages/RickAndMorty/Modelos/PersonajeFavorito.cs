using System.ComponentModel.DataAnnotations;

namespace RickAndMorty.Modelos;

// Guarda una copia mínima para mostrar favoritos sin consultar la API.
public class PersonajeFavorito
{
    public int Id { get; set; }

    [Required]
    public string UsuarioId { get; set; } = "";

    public Usuario? Usuario { get; set; }

    public int PersonajeId { get; set; }

    [Required, StringLength(150)]
    public string Nombre { get; set; } = "";

    [StringLength(50)]
    public string Estado { get; set; } = "";

    [StringLength(100)]
    public string Especie { get; set; } = "";

    [StringLength(500)]
    public string ImagenUrl { get; set; } = "";

    public DateTime GuardadoUtc { get; set; } = DateTime.UtcNow;
}

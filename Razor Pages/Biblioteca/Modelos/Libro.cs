using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Biblioteca.Modelos;

// Guarda una copia breve del libro para no depender de la API en favoritos.
public class Libro
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    [StringLength(30)]
    public string OpenLibraryId { get; set; } = "";

    [Required]
    [StringLength(500)]
    public string Titulo { get; set; } = "";

    [StringLength(600)]
    public string Autores { get; set; } = "";

    public long? PortadaId { get; set; }

    public int? PrimeraPublicacion { get; set; }

    public double? Puntuacion { get; set; }

    public DateTime ActualizadoUtc { get; set; }

    public ICollection<Favorito> Favoritos { get; set; } = [];
}

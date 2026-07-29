using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Peliculas.Modelos;

// Conserva en SQLite una copia breve de una película marcada como favorita.
public class Pelicula
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int TmdbId { get; set; }

    [Required]
    [StringLength(300)]
    public string Titulo { get; set; } = "";

    [StringLength(300)]
    public string TituloOriginal { get; set; } = "";

    [StringLength(300)]
    public string? RutaPoster { get; set; }

    public DateOnly? FechaEstreno { get; set; }

    public double Puntuacion { get; set; }

    public DateTime ActualizadaUtc { get; set; }

    public ICollection<Favorito> Favoritos { get; set; } = [];
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Recetas.Modelos;

// Guarda una copia breve para no depender de la API en cada visita.
public class Receta
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int TheMealDbId { get; set; }

    [Required]
    [StringLength(300)]
    public string Nombre { get; set; } = "";

    [StringLength(1000)]
    public string? ImagenUrl { get; set; }

    [StringLength(100)]
    public string Categoria { get; set; } = "";

    [StringLength(100)]
    public string Area { get; set; } = "";

    // El JSON conserva nombres y cantidades sin crear veinte columnas.
    public string IngredientesJson { get; set; } = "[]";

    public DateTime ActualizadaUtc { get; set; }

    public ICollection<Favorito> Favoritos { get; set; } = [];
    public ICollection<MenuSemanal> DiasMenu { get; set; } = [];
}

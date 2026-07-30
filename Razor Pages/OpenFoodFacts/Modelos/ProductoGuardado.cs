using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OpenFoodFacts.Modelos;

// Conserva una copia de los datos necesarios para favoritos y comparaciones.
public class ProductoGuardado
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    [StringLength(24)]
    public string Codigo { get; set; } = "";

    [Required]
    [StringLength(300)]
    public string Nombre { get; set; } = "";

    [StringLength(300)]
    public string Marca { get; set; } = "";

    [StringLength(1000)]
    public string? ImagenUrl { get; set; }

    [StringLength(100)]
    public string Cantidad { get; set; } = "";

    [StringLength(10)]
    public string NutriScore { get; set; } = "";

    public int? GrupoNova { get; set; }

    [StringLength(20)]
    public string GreenScore { get; set; } = "";

    public double? EnergiaKcal100g { get; set; }
    public double? Grasas100g { get; set; }
    public double? GrasasSaturadas100g { get; set; }
    public double? Hidratos100g { get; set; }
    public double? Azucares100g { get; set; }
    public double? Fibra100g { get; set; }
    public double? Proteinas100g { get; set; }
    public double? Sal100g { get; set; }
    public DateTime ActualizadoUtc { get; set; }

    public ICollection<Favorito> Favoritos { get; set; } = [];
}

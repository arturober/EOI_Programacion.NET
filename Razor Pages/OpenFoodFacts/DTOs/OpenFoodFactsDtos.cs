using System.Text.Json;
using System.Text.Json.Serialization;

namespace OpenFoodFacts.DTOs;

// La consulta por código devuelve el producto dentro de product.
public class RespuestaProductoDto
{
    [JsonPropertyName("code")]
    public string Codigo { get; set; } = "";

    [JsonPropertyName("product")]
    public ProductoDto? Producto { get; set; }

    [JsonPropertyName("status")]
    public int? Estado { get; set; }
}

// Las búsquedas añaden recuento, página y una colección de productos.
public class RespuestaBusquedaDto
{
    [JsonPropertyName("count")]
    public long Total { get; set; }

    [JsonPropertyName("page")]
    public int Pagina { get; set; } = 1;

    [JsonPropertyName("page_size")]
    public int TamanoPagina { get; set; } = 12;

    [JsonPropertyName("products")]
    public List<ProductoDto>? Productos { get; set; }
}

public class ProductoDto
{
    [JsonPropertyName("code")]
    public string Codigo { get; set; } = "";

    [JsonPropertyName("product_name")]
    public string Nombre { get; set; } = "";

    [JsonPropertyName("generic_name")]
    public string? NombreGenerico { get; set; }

    [JsonPropertyName("brands")]
    public string? Marcas { get; set; }

    [JsonPropertyName("quantity")]
    public string? Cantidad { get; set; }

    [JsonPropertyName("image_front_url")]
    public string? ImagenUrl { get; set; }

    [JsonPropertyName("image_front_small_url")]
    public string? ImagenPequenaUrl { get; set; }

    [JsonPropertyName("nutrition_grades")]
    public string? NutriScore { get; set; }

    [JsonPropertyName("nutriscore_grade")]
    public string? NutriScoreActual { get; set; }

    // El grupo NOVA puede llegar como número o como texto.
    [JsonPropertyName("nova_group")]
    public JsonElement GrupoNova { get; set; }

    [JsonPropertyName("ecoscore_grade")]
    public string? EcoScore { get; set; }

    [JsonPropertyName("environmental_score_grade")]
    public string? GreenScore { get; set; }

    [JsonPropertyName("ingredients_text")]
    public string? Ingredientes { get; set; }

    [JsonPropertyName("allergens")]
    public string? Alergenos { get; set; }

    [JsonPropertyName("traces")]
    public string? Trazas { get; set; }

    [JsonPropertyName("categories")]
    public string? Categorias { get; set; }

    [JsonPropertyName("countries")]
    public string? Paises { get; set; }

    [JsonPropertyName("labels")]
    public string? Etiquetas { get; set; }

    [JsonPropertyName("packaging")]
    public string? Envase { get; set; }

    [JsonPropertyName("serving_size")]
    public string? TamanoRacion { get; set; }

    [JsonPropertyName("additives_n")]
    public int? NumeroAditivos { get; set; }

    [JsonPropertyName("additives_tags")]
    public List<string>? Aditivos { get; set; }

    [JsonPropertyName("nutriments")]
    public NutrimentosDto? Nutrimentos { get; set; }
}

public class NutrimentosDto
{
    [JsonPropertyName("energy-kcal_100g")]
    public double? EnergiaKcal100g { get; set; }

    [JsonPropertyName("fat_100g")]
    public double? Grasas100g { get; set; }

    [JsonPropertyName("saturated-fat_100g")]
    public double? GrasasSaturadas100g { get; set; }

    [JsonPropertyName("carbohydrates_100g")]
    public double? Hidratos100g { get; set; }

    [JsonPropertyName("sugars_100g")]
    public double? Azucares100g { get; set; }

    [JsonPropertyName("fiber_100g")]
    public double? Fibra100g { get; set; }

    [JsonPropertyName("proteins_100g")]
    public double? Proteinas100g { get; set; }

    [JsonPropertyName("salt_100g")]
    public double? Sal100g { get; set; }

    [JsonPropertyName("sodium_100g")]
    public double? Sodio100g { get; set; }
}

using System.Text.Json;
using System.Text.Json.Serialization;

namespace Recetas.DTOs;

// La API devuelve las recetas dentro de una propiedad meals.
public class RespuestaRecetasDto
{
    [JsonPropertyName("meals")]
    public List<RecetaDto>? Recetas { get; set; }
}

public class RecetaDto
{
    [JsonPropertyName("idMeal")]
    public string Id { get; set; } = "";

    [JsonPropertyName("strMeal")]
    public string Nombre { get; set; } = "";

    [JsonPropertyName("strMealThumb")]
    public string? ImagenUrl { get; set; }

    [JsonPropertyName("strCategory")]
    public string Categoria { get; set; } = "";

    [JsonPropertyName("strArea")]
    public string Area { get; set; } = "";

    [JsonPropertyName("strInstructions")]
    public string Instrucciones { get; set; } = "";

    [JsonPropertyName("strTags")]
    public string? Etiquetas { get; set; }

    [JsonPropertyName("strYoutube")]
    public string? UrlYoutube { get; set; }

    [JsonPropertyName("strSource")]
    public string? Fuente { get; set; }

    // Captura strIngredient1...20 y strMeasure1...20 sin 40 propiedades.
    [JsonExtensionData]
    public Dictionary<string, JsonElement> OtrosCampos { get; set; } = [];
}

public class RespuestaCategoriasDto
{
    [JsonPropertyName("categories")]
    public List<CategoriaDto>? Categorias { get; set; }
}

public class CategoriaDto
{
    [JsonPropertyName("strCategory")]
    public string Nombre { get; set; } = "";

    [JsonPropertyName("strCategoryThumb")]
    public string ImagenUrl { get; set; } = "";

    [JsonPropertyName("strCategoryDescription")]
    public string Descripcion { get; set; } = "";
}

public class RespuestaAreasDto
{
    [JsonPropertyName("meals")]
    public List<AreaDto>? Areas { get; set; }
}

public class AreaDto
{
    [JsonPropertyName("strArea")]
    public string Nombre { get; set; } = "";
}

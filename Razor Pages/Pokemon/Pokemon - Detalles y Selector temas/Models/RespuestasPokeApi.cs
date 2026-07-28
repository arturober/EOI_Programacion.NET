using System.Text.Json.Serialization;

namespace Pokemon.Models;

// Clases utilizadas por el endpoint de la lista.
public class RespuestaListaPokemonApi
{
    [JsonPropertyName("results")]
    public List<PokemonListaApi> Resultados { get; set; } = [];
}

public class PokemonListaApi
{
    [JsonPropertyName("name")]
    public string Nombre { get; set; } = string.Empty;

    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;

    public int Id
    {
        get
        {
            string ultimoFragmento = Url.TrimEnd('/').Split('/').Last();
            return int.TryParse(ultimoFragmento, out int id) ? id : 0;
        }
    }

    public string ImagenUrl =>
        $"https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/pokemon/{Id}.png";
}

// Clases utilizadas por el endpoint de detalles.
public class PokemonDetalleApi
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Nombre { get; set; } = string.Empty;

    [JsonPropertyName("height")]
    public int AlturaDecimetros { get; set; }

    [JsonPropertyName("weight")]
    public int PesoHectogramos { get; set; }

    [JsonPropertyName("sprites")]
    public PokemonSpritesApi Imagenes { get; set; } = new();

    [JsonPropertyName("types")]
    public List<PokemonTipoSlotApi> Tipos { get; set; } = [];

    [JsonPropertyName("abilities")]
    public List<PokemonHabilidadSlotApi> Habilidades { get; set; } = [];

    public decimal AlturaMetros => AlturaDecimetros / 10m;
    public decimal PesoKilogramos => PesoHectogramos / 10m;
    public string ImagenUrl => Imagenes.Frontal ??
        $"https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/pokemon/{Id}.png";
}

public class PokemonSpritesApi
{
    [JsonPropertyName("front_default")]
    public string? Frontal { get; set; }
}

public class PokemonTipoSlotApi
{
    [JsonPropertyName("type")]
    public NombreApi Tipo { get; set; } = new();
}

public class PokemonHabilidadSlotApi
{
    [JsonPropertyName("ability")]
    public NombreApi Habilidad { get; set; } = new();
}

// PokeAPI utiliza esta misma estructura para tipos y habilidades.
public class NombreApi
{
    [JsonPropertyName("name")]
    public string Nombre { get; set; } = string.Empty;
}

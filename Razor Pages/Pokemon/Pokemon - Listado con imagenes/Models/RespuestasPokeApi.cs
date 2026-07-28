using System.Text.Json.Serialization;

namespace Pokemon.Models;

// Representa la respuesta del endpoint que devuelve la lista de Pokémon.
public class RespuestaListaPokemonApi
{
    [JsonPropertyName("results")]
    public List<PokemonListaApi> Resultados { get; set; } = [];
}

// Representa cada Pokémon de la lista.
public class PokemonListaApi
{
    [JsonPropertyName("name")]
    public string Nombre { get; set; } = string.Empty;

    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;

    // La URL de PokeAPI termina con el identificador: .../pokemon/25/
    public int Id
    {
        get
        {
            string ultimoFragmento = Url.TrimEnd('/').Split('/').Last();
            return int.TryParse(ultimoFragmento, out int id) ? id : 0;
        }
    }

    // Podemos obtener la imagen sin realizar una petición adicional por Pokémon.
    public string ImagenUrl =>
        $"https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/pokemon/{Id}.png";
}

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
}

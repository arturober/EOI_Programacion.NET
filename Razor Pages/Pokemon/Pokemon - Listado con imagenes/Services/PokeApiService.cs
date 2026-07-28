using System.Net.Http.Json;
using Pokemon.Models;

namespace Pokemon.Services;

// Centraliza todas las llamadas a PokeAPI.
public class PokeApiService
{
    private readonly HttpClient _cliente;

    public PokeApiService(HttpClient cliente)
    {
        _cliente = cliente;
    }

    public async Task<List<PokemonListaApi>> ObtenerListaAsync()
    {
        RespuestaListaPokemonApi? respuesta =
            await _cliente.GetFromJsonAsync<RespuestaListaPokemonApi>(
                "pokemon?limit=151&offset=0");

        return respuesta?.Resultados ?? [];
    }
}

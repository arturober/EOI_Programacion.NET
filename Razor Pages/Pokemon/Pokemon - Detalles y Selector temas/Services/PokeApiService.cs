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

    public async Task<PokemonDetalleApi?> ObtenerDetalleAsync(string nombre)
    {
        string nombreSeguro =
            Uri.EscapeDataString(nombre.Trim().ToLowerInvariant());

        using HttpResponseMessage respuesta =
            await _cliente.GetAsync($"pokemon/{nombreSeguro}");

        if (!respuesta.IsSuccessStatusCode)
        {
            return null;
        }

        return await respuesta.Content
            .ReadFromJsonAsync<PokemonDetalleApi>();
    }
}

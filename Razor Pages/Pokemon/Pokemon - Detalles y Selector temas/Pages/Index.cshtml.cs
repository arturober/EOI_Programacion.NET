using Microsoft.AspNetCore.Mvc.RazorPages;
using Pokemon.Models;
using Pokemon.Services;

namespace Pokemon.Pages;

public class IndexModel : PageModel
{
    private readonly PokeApiService _pokeApi;

    public List<PokemonListaApi> Pokemons { get; private set; } = [];

    public IndexModel(PokeApiService pokeApi)
    {
        _pokeApi = pokeApi;
    }

    public async Task OnGetAsync()
    {
        Pokemons = await _pokeApi.ObtenerListaAsync();
    }
}

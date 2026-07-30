using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Recetas.Modelos;
using Recetas.Servicios;

namespace Recetas.Pages.Menu;

// Prepara el calendario y agrupa sus ingredientes para la compra.
[Authorize]
public class IndexModel : PageModel
{
    private readonly IColeccionServicio _coleccion;
    private readonly UserManager<Usuario> _userManager;

    public IndexModel(
        IColeccionServicio coleccion,
        UserManager<Usuario> userManager)
    {
        _coleccion = coleccion;
        _userManager = userManager;
    }

    public Dictionary<DiaMenu, MenuSemanal> Menu { get; private set; } = [];
    public IReadOnlyList<ElementoCompra> ListaCompra { get; private set; } = [];

    public async Task OnGetAsync()
    {
        string usuarioId = _userManager.GetUserId(User)!;
        IReadOnlyList<MenuSemanal> elementos =
            await _coleccion.ObtenerMenuAsync(
                usuarioId,
                HttpContext.RequestAborted);

        Menu = elementos.ToDictionary(elemento => elemento.Dia);
        ListaCompra = CrearListaCompra(elementos);
    }

    private static IReadOnlyList<ElementoCompra> CrearListaCompra(
        IReadOnlyList<MenuSemanal> menu)
    {
        Dictionary<string, List<string>> agrupados =
            new(StringComparer.CurrentCultureIgnoreCase);

        foreach (MenuSemanal elemento in menu)
        {
            List<IngredienteReceta> ingredientes;

            try
            {
                ingredientes =
                    JsonSerializer.Deserialize<List<IngredienteReceta>>(
                        elemento.Receta.IngredientesJson) ?? [];
            }
            catch (JsonException)
            {
                ingredientes = [];
            }

            foreach (IngredienteReceta ingrediente in ingredientes)
            {
                if (!agrupados.TryGetValue(
                    ingrediente.Nombre,
                    out List<string>? medidas))
                {
                    medidas = [];
                    agrupados[ingrediente.Nombre] = medidas;
                }

                if (!string.IsNullOrWhiteSpace(ingrediente.Medida))
                {
                    medidas.Add(ingrediente.Medida);
                }
            }
        }

        return agrupados
            .OrderBy(elemento => elemento.Key)
            .Select(elemento => new ElementoCompra
            {
                Ingrediente = elemento.Key,
                Medidas = elemento.Value.AsReadOnly()
            })
            .ToList()
            .AsReadOnly();
    }
}

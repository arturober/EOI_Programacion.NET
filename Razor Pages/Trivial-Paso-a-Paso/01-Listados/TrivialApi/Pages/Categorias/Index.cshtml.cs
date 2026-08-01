using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TrivialApi.Data;
using TrivialApi.Models;

namespace TrivialApi.Pages.Categorias;

// Esta página únicamente consulta y muestra categorías; todavía no las modifica.
public class IndexModel(TrivialContext contexto) : PageModel
{
    // La lista se inicializa vacía para que la vista siempre pueda recorrerla.
    public List<Categoria> Categorias { get; set; } = [];

    public async Task OnGetAsync()
    {
        // Include carga las preguntas relacionadas para poder mostrar su cantidad.
        // OrderBy ordena las categorías antes de ejecutar la consulta.
        Categorias = await contexto.Categorias
            .Include(categoria => categoria.Preguntas)
            .OrderBy(categoria => categoria.Nombre)
            .ToListAsync();
    }
}


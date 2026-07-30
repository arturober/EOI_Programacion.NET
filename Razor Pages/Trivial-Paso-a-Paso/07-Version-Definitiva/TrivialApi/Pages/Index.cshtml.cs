using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TrivialApi.Data;

namespace TrivialApi.Pages;

public class IndexModel(TrivialContext context) : PageModel
{
    public int TotalPreguntas { get; set; }
    public int TotalCategorias { get; set; }

    public async Task OnGetAsync()
    {
        TotalPreguntas = await context.Preguntas.CountAsync();
        TotalCategorias = await context.Categorias.CountAsync();
    }
}
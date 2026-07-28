using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TrivialApi.Data;
using TrivialApi.Models;

namespace TrivialApi.Pages.Categorias;

public class IndexModel(TrivialContext context) : PageModel
{
    public List<Categoria> Categorias { get; set; } = [];

    public async Task OnGetAsync()
    {
        Categorias = await context.Categorias
            .Include(c => c.Preguntas)
            .OrderBy(c => c.Nombre)
            .ToListAsync();
    }
}
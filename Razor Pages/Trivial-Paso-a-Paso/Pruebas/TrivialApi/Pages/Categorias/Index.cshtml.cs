using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TrivialApi.Data;
using TrivialApi.Models;

namespace TrivialApi.Pages.Categorias;

public class IndexModel(TrivialContext contexto) : PageModel
{
    public List<Categoria> Categorias { get; set; } = [];

    public async Task OnGetAsync()
    {
        Categorias = await contexto.Categorias
            .Include(c => c.Preguntas)
            .OrderBy(c => c.Nombre)
            .ToListAsync();
    }

    public async Task<IActionResult> OnPostEliminarAsync(int id)
    {
        Categoria? categoria = await contexto.Categorias.FindAsync(id);
        if (categoria != null)
        {
            contexto.Categorias.Remove(categoria);
            await contexto.SaveChangesAsync();
            TempData["Mensaje"] = $"Categoría '{categoria.Nombre}' eliminada correctamente.";
        }

        return RedirectToPage();
    }
}
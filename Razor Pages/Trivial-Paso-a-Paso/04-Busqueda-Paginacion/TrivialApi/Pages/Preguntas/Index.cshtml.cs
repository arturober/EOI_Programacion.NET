using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TrivialApi.Data;
using TrivialApi.Models;

namespace TrivialApi.Pages.Preguntas;

public class IndexModel(TrivialContext contexto) : PageModel
{
    public List<Pregunta> Preguntas { get; set; } = [];

    public async Task OnGetAsync()
    {
        IQueryable<Pregunta> query = contexto.Preguntas
            .Include(p => p.Categoria);

        Preguntas = await query
            .OrderBy(p => p.Enunciado)
            .Take(1010)
            .ToListAsync();
    }

    public async Task<IActionResult> OnPostEliminarAsync(int id)
    {
        Pregunta? pregunta = await contexto.Preguntas.FindAsync(id);
        if (pregunta != null)
        {
            contexto.Preguntas.Remove(pregunta);
            await contexto.SaveChangesAsync();
            TempData["Mensaje"] = $"Pregunta '{pregunta.Enunciado}' eliminada correctamente.";
        }

        return RedirectToPage();
    }
}
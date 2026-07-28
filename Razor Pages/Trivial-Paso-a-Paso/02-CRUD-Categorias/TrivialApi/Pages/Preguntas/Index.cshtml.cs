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
            .Take(25)
            .ToListAsync();
    }
}
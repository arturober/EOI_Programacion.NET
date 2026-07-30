using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TrivialApi.Data;
using TrivialApi.Models;

namespace TrivialApi.Pages.Preguntas;

public class IndexModel(TrivialContext contexto) : PageModel
{
    public List<Pregunta> Preguntas { get; set; } = [];

    public SelectList Categorias { get; set; } = default!;

    [BindProperty(SupportsGet = true)]
    public string? Busqueda { get; set; }

    [BindProperty(SupportsGet = true)]
    public int? CategoriaId { get; set; }

    public async Task OnGetAsync()
    {
        IQueryable<Pregunta> consulta = contexto.Preguntas
            .Include(p => p.Categoria);

        if (CategoriaId.HasValue && CategoriaId.Value > 0)
        {
            consulta = consulta.Where(p => p.CategoriaId == CategoriaId.Value);
        }

        List<Pregunta> resultados = await consulta.ToListAsync();    

        if (!string.IsNullOrEmpty(Busqueda))
        {
            string busquedaNormalizada = Normalizar(Busqueda);

            resultados = resultados
                .Where(p => Normalizar(p.Enunciado).Contains(busquedaNormalizada))
                .ToList();  
        }

        Preguntas = resultados
            .OrderBy(p => p.Enunciado)
            .Take(10000)
            .ToList();

        List<Categoria> categorias = await contexto.Categorias
            .OrderBy(categoria => categoria.Nombre)
            .ToListAsync();

        ViewData["Categorias"] = 
            new SelectList(categorias, "Id", "Nombre");    
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

    private static string Normalizar(string? texto)
    {
        if (string.IsNullOrEmpty(texto))
        {
            return string.Empty;
        }

        return texto.ToLower().Trim();
    }
}
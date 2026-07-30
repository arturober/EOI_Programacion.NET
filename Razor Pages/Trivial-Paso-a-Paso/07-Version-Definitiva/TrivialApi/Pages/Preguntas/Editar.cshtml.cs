using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TrivialApi.Data;
using TrivialApi.Models;

namespace TrivialApi.Pages.Preguntas;

public class EditarModel(TrivialContext contexto) : PageModel
{
    [BindProperty]      
    public Pregunta Pregunta { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        Pregunta? pregunta = await contexto.Preguntas.FindAsync(id);

        if (pregunta is null)
        {
            return NotFound();
        }

        Pregunta = pregunta;

        await CargarCategoriasAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        bool categoriaValida = await contexto.Categorias
            .AnyAsync(categoria => categoria.Id == Pregunta.CategoriaId);

        if (!categoriaValida)
        {
            ModelState.AddModelError(
                "Pregunta.CategoriaId", 
                "La categoría seleccionada no es válida.");
        }

        if (!ModelState.IsValid)
        {
            await CargarCategoriasAsync();
            return Page();
        }

        contexto.Attach(Pregunta).State = EntityState.Modified;
        await contexto.SaveChangesAsync();

        TempData["Mensaje"] = "Pregunta editada correctamente.";
        return RedirectToPage("Index");
    }

    private async Task CargarCategoriasAsync()
    {
        List<Categoria> categorias = await contexto.Categorias
            .OrderBy(categoria => categoria.Nombre)
            .ToListAsync();

        ViewData["Categorias"] = 
            new SelectList(categorias, "Id", "Nombre");
    }
}
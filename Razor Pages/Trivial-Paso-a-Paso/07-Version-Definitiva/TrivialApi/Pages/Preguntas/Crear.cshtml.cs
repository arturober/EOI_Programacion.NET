using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TrivialApi.Data;
using TrivialApi.Models;

namespace TrivialApi.Pages.Preguntas;

public class CrearModel(TrivialContext contexto) : PageModel
{
    [BindProperty]
    public Pregunta Pregunta { get; set; } = new();

    public async Task OnGetAsync()
    {
        await CargarCategoriasAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        bool categoriaValida = await contexto.Categorias
            .AnyAsync(categoria => categoria.Id == Pregunta.CategoriaId);

        if (!categoriaValida)
        {
            ModelState.AddModelError("Pregunta.CategoriaId", "La categoría seleccionada no es válida.");
        }

        if (!ModelState.IsValid)
        {
            await CargarCategoriasAsync();
            return Page();
        }

        contexto.Preguntas.Add(Pregunta);
        await contexto.SaveChangesAsync();

        TempData["Mensaje"] = "Pregunta creada correctamente.";
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
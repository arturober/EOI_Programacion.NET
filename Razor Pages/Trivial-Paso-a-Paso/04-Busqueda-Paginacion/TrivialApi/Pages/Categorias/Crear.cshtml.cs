using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TrivialApi.Data;
using TrivialApi.Models;

namespace TrivialApi.Pages.Categorias;

public class CrearModel(TrivialContext contexto) : PageModel
{
    [BindProperty]
    public Categoria Categoria { get; set; } = new();

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        bool existeCategoria = await contexto.Categorias
            .AnyAsync(c => c.Nombre.ToLower() == Categoria.Nombre.ToLower());

        if (existeCategoria)
        {
            ModelState.AddModelError("Categoria.Nombre", "Ya existe una categoría con ese nombre.");
            return Page();
        }

        contexto.Categorias.Add(Categoria);
        await contexto.SaveChangesAsync();

        TempData["Mensaje"] = $"Categoría '{Categoria.Nombre}' creada correctamente.";

        return RedirectToPage("./Index");
    }
}
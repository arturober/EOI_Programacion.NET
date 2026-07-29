using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TrivialApi.Data;
using TrivialApi.Models;

namespace TrivialApi.Pages.Categorias;

public class EditarModel(TrivialContext contexto) : PageModel
{
    [BindProperty]
    public Categoria Categoria { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        Categoria? categoria = await contexto.Categorias.FindAsync(id);

        if (categoria == null)
        {
            return NotFound();
        }

        Categoria = categoria;

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        bool categoriaExistente = await contexto.Categorias.AnyAsync(c => c.Id != Categoria.Id && c.Nombre.ToLower() == Categoria.Nombre.ToLower());

        if (categoriaExistente)
        {
            ModelState.AddModelError("Categoria.Nombre", "Ya existe una categoría con el mismo nombre.");
            return Page();
        }


        contexto.Attach(Categoria).State = EntityState.Modified;
        await contexto.SaveChangesAsync();

        TempData["Mensaje"] = "Categoría editada correctamente.";

        return RedirectToPage("./Index");
    }
}
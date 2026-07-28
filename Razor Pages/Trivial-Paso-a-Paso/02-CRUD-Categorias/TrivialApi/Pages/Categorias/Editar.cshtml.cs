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

        contexto.Categorias.Add(Categoria);
        await contexto.SaveChangesAsync();

        return RedirectToPage("./Index");
    }
}
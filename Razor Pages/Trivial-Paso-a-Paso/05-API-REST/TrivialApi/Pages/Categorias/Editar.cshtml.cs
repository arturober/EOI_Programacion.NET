using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TrivialApi.Data;
using TrivialApi.Models;

namespace TrivialApi.Pages.Categorias;

// Esta página carga una categoría y guarda posteriormente sus cambios.
public class EditarModel(TrivialContext contexto) : PageModel
{
    // La misma propiedad sirve para mostrar y para recibir el formulario.
    [BindProperty]
    public Categoria Categoria { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        // FindAsync busca directamente por la clave primaria.
        Categoria? categoria = await contexto.Categorias.FindAsync(id);

        if (categoria is null)
        {
            // NotFound produce una respuesta HTTP 404 si el Id no existe.
            return NotFound();
        }

        // Entregamos la entidad encontrada a los Tag Helpers de la vista.
        Categoria = categoria;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        // Excluimos la propia fila al comprobar si el nombre ya está ocupado.
        bool nombreRepetido = await contexto.Categorias.AnyAsync(categoria =>
            categoria.Id != Categoria.Id &&
            categoria.Nombre.ToLower() == Categoria.Nombre.ToLower());

        if (nombreRepetido)
        {
            ModelState.AddModelError(
                "Categoria.Nombre",
                "Ya existe otra categoría con ese nombre.");
            return Page();
        }

        // Attach conecta la entidad recibida con el contexto.
        // Modified indica que Entity Framework debe generar un UPDATE.
        contexto.Attach(Categoria).State = EntityState.Modified;
        await contexto.SaveChangesAsync();

        TempData["Mensaje"] = "La categoría se ha actualizado correctamente.";
        return RedirectToPage("Index");
    }
}


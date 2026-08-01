using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TrivialApi.Data;
using TrivialApi.Models;

namespace TrivialApi.Pages.Preguntas;

// La edición tiene un GET para cargar datos y un POST para guardarlos.
public class EditarModel(TrivialContext contexto) : PageModel
{
    [BindProperty]
    public Pregunta Pregunta { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        // Recuperamos la pregunta indicada en el segmento de la dirección.
        Pregunta? pregunta = await contexto.Preguntas.FindAsync(id);

        if (pregunta is null)
        {
            return NotFound();
        }

        Pregunta = pregunta;

        // El formulario necesita la lista incluso cuando ya hay una opción seleccionada.
        await CargarCategoriasAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        bool categoriaExiste = await contexto.Categorias
            .AnyAsync(categoria => categoria.Id == Pregunta.CategoriaId);

        if (!categoriaExiste)
        {
            ModelState.AddModelError(
                "Pregunta.CategoriaId",
                "Selecciona una categoría válida.");
        }

        if (!ModelState.IsValid)
        {
            await CargarCategoriasAsync();
            return Page();
        }

        // La entidad procede del formulario y el contexto todavía no la seguía.
        // Attach la conecta y Modified solicita actualizar todas sus propiedades.
        contexto.Attach(Pregunta).State = EntityState.Modified;
        await contexto.SaveChangesAsync();

        TempData["Mensaje"] = "La pregunta se ha actualizado correctamente.";
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


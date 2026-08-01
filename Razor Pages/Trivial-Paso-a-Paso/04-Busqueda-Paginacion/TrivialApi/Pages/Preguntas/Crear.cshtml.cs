using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TrivialApi.Data;
using TrivialApi.Models;

namespace TrivialApi.Pages.Preguntas;

// Esta página crea una pregunta y la relaciona con una categoría existente.
public class CrearModel(TrivialContext contexto) : PageModel
{
    // BindProperty recibe todos los controles cuyo nombre comienza por Pregunta.
    [BindProperty]
    public Pregunta Pregunta { get; set; } = new();

    public async Task OnGetAsync()
    {
        // El desplegable necesita las categorías antes de representar la página.
        await CargarCategoriasAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        // Range comprueba que CategoriaId sea positivo, pero además verificamos
        // que la categoría exista realmente en la base de datos.
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
            // Al repetir el formulario debemos reconstruir las opciones del select.
            await CargarCategoriasAsync();
            return Page();
        }

        // Entity Framework insertará la pregunta con su CategoriaId.
        contexto.Preguntas.Add(Pregunta);
        await contexto.SaveChangesAsync();

        TempData["Mensaje"] = "La pregunta se ha creado correctamente.";
        return RedirectToPage("Index");
    }

    private async Task CargarCategoriasAsync()
    {
        // Primero ejecutamos la consulta ordenada y después construimos la SelectList.
        List<Categoria> categorias = await contexto.Categorias
            .OrderBy(categoria => categoria.Nombre)
            .ToListAsync();

        // "Id" será el value de cada option y "Nombre" será su texto visible.
        ViewData["Categorias"] =
            new SelectList(categorias, "Id", "Nombre");
    }
}


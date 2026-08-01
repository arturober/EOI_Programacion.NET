using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TrivialApi.Data;
using TrivialApi.Models;

namespace TrivialApi.Pages.Categorias;

// Esta página recibe los datos necesarios para insertar una categoría.
public class CrearModel(TrivialContext contexto) : PageModel
{
    // BindProperty copia los valores del formulario dentro de esta propiedad.
    [BindProperty]
    public Categoria Categoria { get; set; } = new();

    public async Task<IActionResult> OnPostAsync()
    {
        // Los atributos del modelo alimentan automáticamente ModelState.
        if (!ModelState.IsValid)
        {
            // Page vuelve a mostrar el mismo formulario y conserva los errores.
            return Page();
        }

        // Comprobamos el nombre antes de insertar para ofrecer un mensaje comprensible.
        bool nombreRepetido = await contexto.Categorias.AnyAsync(categoria =>
            categoria.Nombre.ToLower() == Categoria.Nombre.ToLower());

        if (nombreRepetido)
        {
            ModelState.AddModelError(
                "Categoria.Nombre",
                "Ya existe una categoría con ese nombre.");
            return Page();
        }

        // Add comienza a seguir la nueva entidad en estado Added.
        contexto.Categorias.Add(Categoria);

        // SaveChangesAsync traduce el cambio pendiente a una sentencia INSERT.
        await contexto.SaveChangesAsync();

        // El mensaje se mostrará después de la redirección.
        TempData["Mensaje"] = "La categoría se ha creado correctamente.";

        // Aplicamos Post/Redirect/Get para que F5 no repita el alta.
        return RedirectToPage("Index");
    }
}


using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TrivialApi.Data;
using TrivialApi.Models;

namespace TrivialApi.Pages.Categorias;

// Esta página únicamente consulta y muestra categorías; todavía no las modifica.
public class IndexModel(TrivialContext contexto) : PageModel
{
    // La lista se inicializa vacía para que la vista siempre pueda recorrerla.
    public List<Categoria> Categorias { get; set; } = [];

    public async Task OnGetAsync()
    {
        // Include carga las preguntas relacionadas para poder mostrar su cantidad.
        // OrderBy ordena las categorías antes de ejecutar la consulta.
        Categorias = await contexto.Categorias
            .Include(categoria => categoria.Preguntas)
            .OrderBy(categoria => categoria.Nombre)
            .ToListAsync();
    }

    public async Task<IActionResult> OnPostEliminarAsync(int id)
    {
        // FindAsync busca la categoría utilizando su clave primaria.
        Categoria? categoria = await contexto.Categorias.FindAsync(id);

        // Comprobamos el resultado porque alguien podría enviar un Id inexistente.
        if (categoria is not null)
        {
            // Remove marca la entidad para que se elimine en el siguiente guardado.
            contexto.Categorias.Remove(categoria);

            // SaveChangesAsync ejecuta el DELETE. La relación configurada en el
            // contexto hace que también se borren las preguntas dependientes.
            await contexto.SaveChangesAsync();

            // TempData sobrevive a la redirección y el layout mostrará el mensaje.
            TempData["Mensaje"] =
                "La categoría y sus preguntas se han eliminado correctamente.";
        }

        // RedirectToPage vuelve a ejecutar el GET y evita repetir el POST al actualizar.
        return RedirectToPage();
    }
}

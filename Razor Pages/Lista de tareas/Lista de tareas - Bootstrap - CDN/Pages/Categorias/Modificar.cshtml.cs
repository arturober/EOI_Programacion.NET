using ListaTareas.Datos;
using ListaTareas.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.Sqlite;

namespace ListaTareas.Pages.Categorias;

public class ModificarCategoriaModel : PageModel
{
    [BindProperty]
    public Categoria Categoria { get; set; } = new Categoria();

    public IActionResult OnGet(int id)
    {
        using SqliteConnection conexion = BaseDatos.Inicializar();

        Categoria? categoriaEncontrada =
            ListaTareas.Models.Categoria.BuscarPorId(conexion, id);

        if (categoriaEncontrada == null)
        {
            TempData["Error"] = "La categoría ya no existe.";
            return RedirectToPage("Index");
        }

        Categoria = categoriaEncontrada;
        return Page();
    }

    public IActionResult OnPost()
    {
        using SqliteConnection conexion = BaseDatos.Inicializar();

        if (ListaTareas.Models.Categoria.BuscarPorId(
            conexion, Categoria.Id) == null)
        {
            TempData["Error"] = "La categoría ya no existe.";
            return RedirectToPage("Index");
        }

        if (ModelState.IsValid &&
            ListaTareas.Models.Categoria.ExisteNombre(
                conexion, Categoria.Nombre, Categoria.Id))
        {
            ModelState.AddModelError(
                "Categoria.Nombre", "Ya existe otra categoría con ese nombre.");
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }

        if (!Categoria.Actualizar(conexion))
        {
            TempData["Error"] = "La categoría ya no existe.";
            return RedirectToPage("Index");
        }

        TempData["Mensaje"] = "La categoría se ha modificado correctamente.";
        return RedirectToPage("Index");
    }
}

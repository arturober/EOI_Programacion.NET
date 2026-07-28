using ListaTareas.Datos;
using ListaTareas.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.Sqlite;

namespace ListaTareas.Pages.Categorias;

public class CrearCategoriaModel : PageModel
{
    [BindProperty]
    public Categoria Categoria { get; set; } = new Categoria();

    public IActionResult OnPost()
    {
        using SqliteConnection conexion = BaseDatos.Inicializar();

        if (ModelState.IsValid &&
            ListaTareas.Models.Categoria.ExisteNombre(
                conexion, Categoria.Nombre))
        {
            ModelState.AddModelError(
                "Categoria.Nombre", "Ya existe una categoría con ese nombre.");
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }

        if (!Categoria.Insertar(conexion))
        {
            ModelState.AddModelError("", "No se ha podido guardar la categoría.");
            return Page();
        }

        TempData["Mensaje"] = "La categoría se ha creado correctamente.";
        return RedirectToPage("Index");
    }
}

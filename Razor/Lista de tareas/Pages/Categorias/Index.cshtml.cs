using ListaTareas.Datos;
using ListaTareas.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.Sqlite;

namespace ListaTareas.Pages.Categorias;

public class CategoriasIndexModel : PageModel
{
    public List<Categoria> Categorias { get; private set; } = new List<Categoria>();

    public void OnGet()
    {
        using SqliteConnection conexion = BaseDatos.Inicializar();
        Categorias = Categoria.Listar(conexion);
    }

    public IActionResult OnPostEliminar(int id)
    {
        using SqliteConnection conexion = BaseDatos.Inicializar();

        Categoria? categoria = Categoria.BuscarPorId(conexion, id);

        if (categoria == null)
        {
            TempData["Error"] = "La categoría ya no existe.";
        }
        else if (categoria.ContarTareas(conexion) > 0)
        {
            TempData["Error"] =
                "No se puede eliminar una categoría que todavía contiene tareas.";
        }
        else if (categoria.Borrar(conexion))
        {
            TempData["Mensaje"] = "La categoría se ha eliminado correctamente.";
        }
        else
        {
            TempData["Error"] = "No se ha podido eliminar la categoría.";
        }

        return RedirectToPage();
    }
}

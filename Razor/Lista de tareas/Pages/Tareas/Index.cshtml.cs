using ListaTareas.Datos;
using ListaTareas.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.Sqlite;

namespace ListaTareas.Pages.Tareas;

public class TareasIndexModel : PageModel
{
    public List<Tarea> Tareas { get; private set; } = new List<Tarea>();
    public List<Categoria> Categorias { get; private set; } = new List<Categoria>();

    [BindProperty(SupportsGet = true)]
    public int? CategoriaId { get; set; }

    public void OnGet()
    {
        using SqliteConnection conexion = BaseDatos.Inicializar();

        Categorias = Categoria.Listar(conexion);
        Tareas = Tarea.Listar(conexion, CategoriaId);
    }

    public IActionResult OnPostEliminar(int id)
    {
        using SqliteConnection conexion = BaseDatos.Inicializar();

        Tarea? tarea = Tarea.BuscarPorId(conexion, id);

        if (tarea == null || !tarea.Borrar(conexion))
        {
            TempData["Error"] = "La tarea ya no existe.";
        }
        else
        {
            TempData["Mensaje"] = "La tarea se ha eliminado correctamente.";
        }

        return RedirectToPage(new { categoriaId = CategoriaId });
    }
}

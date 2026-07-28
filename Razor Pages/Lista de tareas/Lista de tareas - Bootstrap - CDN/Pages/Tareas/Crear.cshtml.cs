using ListaTareas.Datos;
using ListaTareas.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.Sqlite;

namespace ListaTareas.Pages.Tareas;

public class CrearTareaModel : PageModel
{
    [BindProperty]
    public Tarea Tarea { get; set; } = new Tarea();

    [BindProperty]
    public int CategoriaId { get; set; }

    public List<Categoria> Categorias { get; private set; } = new List<Categoria>();

    public void OnGet()
    {
        using SqliteConnection conexion = BaseDatos.Inicializar();
        Categorias = Categoria.Listar(conexion);
    }

    public IActionResult OnPost()
    {
        using SqliteConnection conexion = BaseDatos.Inicializar();

        Categoria? categoria = Categoria.BuscarPorId(conexion, CategoriaId);

        if (categoria == null)
        {
            ModelState.AddModelError(
                nameof(CategoriaId), "Debes seleccionar una categoría válida.");
        }

        if (!ModelState.IsValid)
        {
            Categorias = Categoria.Listar(conexion);
            return Page();
        }

        Tarea.Categoria = categoria!;

        if (!Tarea.Insertar(conexion))
        {
            ModelState.AddModelError("", "No se ha podido guardar la tarea.");
            Categorias = Categoria.Listar(conexion);
            return Page();
        }

        TempData["Mensaje"] = "La tarea se ha creado correctamente.";
        return RedirectToPage("Index");
    }
}

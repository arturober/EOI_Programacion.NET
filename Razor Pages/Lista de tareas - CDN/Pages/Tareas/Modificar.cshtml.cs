using ListaTareas.Datos;
using ListaTareas.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.Sqlite;

namespace ListaTareas.Pages.Tareas;

public class ModificarTareaModel : PageModel
{
    [BindProperty]
    public Tarea Tarea { get; set; } = new Tarea();

    [BindProperty]
    public int CategoriaId { get; set; }

    public List<Categoria> Categorias { get; private set; } = new List<Categoria>();

    public IActionResult OnGet(int id)
    {
        using SqliteConnection conexion = BaseDatos.Inicializar();

        Tarea? tareaEncontrada = ListaTareas.Models.Tarea.BuscarPorId(
            conexion, id);

        if (tareaEncontrada == null)
        {
            TempData["Error"] = "La tarea ya no existe.";
            return RedirectToPage("Index");
        }

        Tarea = tareaEncontrada;
        CategoriaId = Tarea.Categoria.Id;
        Categorias = Categoria.Listar(conexion);

        return Page();
    }

    public IActionResult OnPost()
    {
        using SqliteConnection conexion = BaseDatos.Inicializar();

        Tarea? tareaGuardada = ListaTareas.Models.Tarea.BuscarPorId(
            conexion, Tarea.Id);
        Categoria? categoria = Categoria.BuscarPorId(conexion, CategoriaId);

        if (tareaGuardada == null)
        {
            TempData["Error"] = "La tarea ya no existe.";
            return RedirectToPage("Index");
        }

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

        if (!Tarea.Actualizar(conexion))
        {
            TempData["Error"] = "La tarea ya no existe.";
            return RedirectToPage("Index");
        }

        TempData["Mensaje"] = "La tarea se ha modificado correctamente.";
        return RedirectToPage("Index");
    }
}

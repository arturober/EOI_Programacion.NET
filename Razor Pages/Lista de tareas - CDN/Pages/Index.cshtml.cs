using ListaTareas.Datos;
using ListaTareas.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.Sqlite;

namespace ListaTareas.Pages;

public class IndexModel : PageModel
{
    public int TotalTareas { get; private set; }
    public int TareasPendientes { get; private set; }
    public int TotalCategorias { get; private set; }

    public void OnGet()
    {
        using SqliteConnection conexion = BaseDatos.Inicializar();

        List<Tarea> tareas = Tarea.Listar(conexion);

        TotalTareas = tareas.Count;
        TareasPendientes = tareas.Count(tarea => !tarea.Completada);
        TotalCategorias = Categoria.Listar(conexion).Count;
    }
}

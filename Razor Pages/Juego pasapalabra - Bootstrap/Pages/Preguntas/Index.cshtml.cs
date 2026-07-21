using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.Sqlite;

namespace PasapalabraRazor.Pages.Preguntas;

public class IndexModel : PageModel
{
    [BindProperty(SupportsGet = true)]
    public string Buscar { get; set; } = "";

    public List<Pregunta> Preguntas { get; set; } = new List<Pregunta>();

    public void OnGet()
    {
        using SqliteConnection conexion = BaseDatos.Inicializar();
        Preguntas = Pregunta.Listar(conexion, Buscar);
    }

    public IActionResult OnPostBorrar(int id)
    {
        using SqliteConnection conexion = BaseDatos.Inicializar();
        Pregunta? pregunta = Pregunta.BuscarPorId(conexion, id);

        if (pregunta == null)
        {
            TempData["Error"] = "La pregunta no existe.";
        }
        else
        {
            pregunta.Borrar(conexion);
            TempData["Mensaje"] = "Pregunta eliminada correctamente.";
        }

        return RedirectToPage(new { buscar = Buscar });
    }
}

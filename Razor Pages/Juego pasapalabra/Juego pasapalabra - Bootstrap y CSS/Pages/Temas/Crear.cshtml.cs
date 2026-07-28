using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.Sqlite;

namespace PasapalabraRazor.Pages.Temas;

public class CrearModel : PageModel
{
    [BindProperty]
    public Tema TemaFormulario { get; set; } = new Tema();

    public void OnGet()
    {
    }

    public IActionResult OnPost()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        try
        {
            using SqliteConnection conexion = BaseDatos.Inicializar();
            TemaFormulario.Insertar(conexion);
            TempData["Mensaje"] = "Tema creado correctamente.";
            return RedirectToPage("Index");
        }
        catch (SqliteException excepcion) when (excepcion.SqliteErrorCode == 19)
        {
            ModelState.AddModelError("", "Ya existe un tema con ese nombre.");
            return Page();
        }
    }
}

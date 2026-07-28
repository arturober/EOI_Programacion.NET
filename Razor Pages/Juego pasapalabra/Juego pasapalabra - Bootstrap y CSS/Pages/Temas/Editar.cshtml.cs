using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.Sqlite;

namespace PasapalabraRazor.Pages.Temas;

public class EditarModel : PageModel
{
    [BindProperty]
    public Tema TemaFormulario { get; set; } = new Tema();

    public IActionResult OnGet(int id)
    {
        using SqliteConnection conexion = BaseDatos.Inicializar();
        Tema? temaEncontrado = Tema.BuscarPorId(conexion, id);

        if (temaEncontrado == null)
        {
            TempData["Error"] = "El tema no existe.";
            return RedirectToPage("Index");
        }

        TemaFormulario = temaEncontrado;
        return Page();
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
            Tema? temaEncontrado = Tema.BuscarPorId(
                conexion, TemaFormulario.Id);

            if (temaEncontrado == null)
            {
                TempData["Error"] = "El tema no existe.";
                return RedirectToPage("Index");
            }

            TemaFormulario.Actualizar(conexion);
            TempData["Mensaje"] = "Tema modificado correctamente.";
            return RedirectToPage("Index");
        }
        catch (SqliteException excepcion) when (excepcion.SqliteErrorCode == 19)
        {
            ModelState.AddModelError("", "Ya existe otro tema con ese nombre.");
            return Page();
        }
    }
}

using AgendaTelefonos.Datos;
using AgendaTelefonos.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.Sqlite;

namespace AgendaTelefonos.Pages;

public class CrearModel : PageModel
{
    [BindProperty]
    public Persona Persona { get; set; } = new Persona();

    [BindProperty]
    public IFormFile? ImagenSubida { get; set; }

    public IActionResult OnPost()
    {
        string? errorImagen = Persona.ProcesarImagen(ImagenSubida);

        if (errorImagen != null)
        {
            ModelState.AddModelError(nameof(ImagenSubida), errorImagen);
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }

        using SqliteConnection conexion = BaseDatos.Inicializar();

        if (!Persona.Insertar(conexion))
        {
            ModelState.AddModelError("", "No se ha podido guardar el contacto.");
            return Page();
        }

        TempData["Mensaje"] = "El contacto se ha añadido correctamente.";
        return RedirectToPage("/Index");
    }
}

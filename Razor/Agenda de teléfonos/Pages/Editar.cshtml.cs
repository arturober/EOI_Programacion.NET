using AgendaTelefonos.Datos;
using AgendaTelefonos.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.Sqlite;

namespace AgendaTelefonos.Pages;

public class EditarModel : PageModel
{
    [BindProperty]
    public Persona Persona { get; set; } = new Persona();

    [BindProperty]
    public IFormFile? ImagenSubida { get; set; }

    [BindProperty]
    public bool EliminarImagen { get; set; }

    public IActionResult OnGet(int id)
    {
        using SqliteConnection conexion = BaseDatos.Inicializar();

        Persona? personaEncontrada = AgendaTelefonos.Models.Persona.BuscarPorId(
            conexion, id);

        if (personaEncontrada == null)
        {
            return RedirectToPage("/Index");
        }

        Persona = personaEncontrada;
        return Page();
    }

    public IActionResult OnPost()
    {
        using SqliteConnection conexion = BaseDatos.Inicializar();

        Persona? personaGuardada = AgendaTelefonos.Models.Persona.BuscarPorId(
            conexion, Persona.Id);

        if (personaGuardada == null)
        {
            TempData["Error"] = "El contacto ya no existe.";
            return RedirectToPage("/Index");
        }

        string? errorImagen = Persona.ProcesarImagen(ImagenSubida);

        if (errorImagen != null)
        {
            ModelState.AddModelError(nameof(ImagenSubida), errorImagen);
        }

        if (!ModelState.IsValid)
        {
            // Recuperamos la imagen para poder seguir mostrándola al repetir el formulario.
            Persona.ImagenBase64 = personaGuardada.ImagenBase64;
            return Page();
        }

        if ((ImagenSubida == null || ImagenSubida.Length == 0) && !EliminarImagen)
        {
            Persona.ImagenBase64 = personaGuardada.ImagenBase64;
        }

        if (!Persona.Actualizar(conexion))
        {
            TempData["Error"] = "El contacto ya no existe.";
            return RedirectToPage("/Index");
        }

        TempData["Mensaje"] = "El contacto se ha modificado correctamente.";
        return RedirectToPage("/Index");
    }
}

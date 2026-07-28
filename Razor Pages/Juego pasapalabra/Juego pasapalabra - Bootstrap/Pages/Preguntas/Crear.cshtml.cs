using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.Sqlite;

namespace PasapalabraRazor.Pages.Preguntas;

public class CrearModel : PageModel
{
    [BindProperty]
    public Pregunta PreguntaFormulario { get; set; } = new Pregunta { Letra = 'A' };

    [BindProperty]
    public int TemaId { get; set; }

    public List<Tema> Temas { get; set; } = new List<Tema>();

    public void OnGet()
    {
        CargarTemas();
    }

    public IActionResult OnPost()
    {
        CargarTemas();
        Tema? tema = Temas.FirstOrDefault(t => t.Id == TemaId);
        ValidarPregunta(tema);

        if (!ModelState.IsValid)
        {
            return Page();
        }

        try
        {
            using SqliteConnection conexion = BaseDatos.Inicializar();
            PreguntaFormulario.Tema = tema!;
            PreguntaFormulario.Insertar(conexion);
            TempData["Mensaje"] = "Pregunta creada correctamente.";
            return RedirectToPage("Index");
        }
        catch (SqliteException excepcion) when (excepcion.SqliteErrorCode == 19)
        {
            ModelState.AddModelError(
                "", "Esa respuesta ya existe dentro del tema seleccionado.");
            return Page();
        }
    }

    private void CargarTemas()
    {
        using SqliteConnection conexion = BaseDatos.Inicializar();
        Temas = Tema.Listar(conexion);
    }

    private void ValidarPregunta(Tema? tema)
    {
        if (tema == null)
        {
            ModelState.AddModelError("", "Selecciona un tema válido.");
        }

        if (!TextoUtil.EsLetraDelRosco(PreguntaFormulario.Letra))
        {
            ModelState.AddModelError("", "Selecciona una letra válida.");
        }

        if (!string.IsNullOrWhiteSpace(PreguntaFormulario.Respuesta) &&
            !TextoUtil.EsRespuestaValida(
                PreguntaFormulario.Respuesta, PreguntaFormulario.Letra))
        {
            ModelState.AddModelError(
                "", "La respuesta solo puede contener letras, espacios o guiones " +
                    "y debe incluir la letra elegida.");
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.Sqlite;

namespace PasapalabraRazor.Pages.Preguntas;

public class EditarModel : PageModel
{
    [BindProperty]
    public Pregunta PreguntaFormulario { get; set; } = new Pregunta();

    [BindProperty]
    public int TemaId { get; set; }

    public List<Tema> Temas { get; set; } = new List<Tema>();

    public IActionResult OnGet(int id)
    {
        using SqliteConnection conexion = BaseDatos.Inicializar();
        Pregunta? preguntaEncontrada = Pregunta.BuscarPorId(conexion, id);

        if (preguntaEncontrada == null)
        {
            TempData["Error"] = "La pregunta no existe.";
            return RedirectToPage("Index");
        }

        PreguntaFormulario = preguntaEncontrada;
        TemaId = PreguntaFormulario.Tema.Id;
        Temas = Tema.Listar(conexion);
        return Page();
    }

    public IActionResult OnPost()
    {
        using SqliteConnection conexion = BaseDatos.Inicializar();
        Temas = Tema.Listar(conexion);
        Tema? tema = Temas.FirstOrDefault(t => t.Id == TemaId);
        ValidarPregunta(tema);

        if (!ModelState.IsValid)
        {
            return Page();
        }

        try
        {
            Pregunta? preguntaEncontrada = Pregunta.BuscarPorId(
                conexion, PreguntaFormulario.Id);

            if (preguntaEncontrada == null)
            {
                TempData["Error"] = "La pregunta no existe.";
                return RedirectToPage("Index");
            }

            PreguntaFormulario.Tema = tema!;
            PreguntaFormulario.Actualizar(conexion);
            TempData["Mensaje"] = "Pregunta modificada correctamente.";
            return RedirectToPage("Index");
        }
        catch (SqliteException excepcion) when (excepcion.SqliteErrorCode == 19)
        {
            ModelState.AddModelError(
                "", "Esa respuesta ya existe en el tema seleccionado.");
            return Page();
        }
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

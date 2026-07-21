using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.Sqlite;

namespace PasapalabraRazor.Pages.Temas;

public class IndexModel : PageModel
{
    public List<TemaResumen> Temas { get; set; } = new List<TemaResumen>();

    public void OnGet()
    {
        using SqliteConnection conexion = BaseDatos.Inicializar();
        foreach (Tema tema in Tema.Listar(conexion))
        {
            Temas.Add(new TemaResumen
            {
                Tema = tema,
                Preguntas = Pregunta.ContarPorTema(conexion, tema.Id),
                Letras = Pregunta.ContarLetras(conexion, tema.Id)
            });
        }
    }

    public IActionResult OnPostBorrar(int id)
    {
        using SqliteConnection conexion = BaseDatos.Inicializar();
        Tema? tema = Tema.BuscarPorId(conexion, id);

        if (tema == null)
        {
            TempData["Error"] = "El tema no existe.";
        }
        else if (Pregunta.ContarPorTema(conexion, id) > 0)
        {
            TempData["Error"] = "No se puede eliminar un tema que contiene preguntas.";
        }
        else
        {
            tema.Borrar(conexion);
            TempData["Mensaje"] = "Tema eliminado correctamente.";
        }
        return RedirectToPage();
    }
}

public class TemaResumen
{
    public Tema Tema { get; set; } = new Tema();
    public int Preguntas { get; set; }
    public int Letras { get; set; }
}

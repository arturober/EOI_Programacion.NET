using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.Sqlite;

namespace PasapalabraRazor.Pages.Jugar;

public class IndexModel : PageModel
{
    public List<TemaConLetras> Temas { get; set; } = new List<TemaConLetras>();
    public int LetrasTotales { get; set; }
    public string Error { get; set; } = "";

    public void OnGet()
    {
        CargarTemas();
    }

    public IActionResult OnPost(int temaId)
    {
        using SqliteConnection conexion = BaseDatos.Inicializar();

        if (temaId < 0 ||
            (temaId > 0 && Tema.BuscarPorId(conexion, temaId) == null))
        {
            Error = "El tema seleccionado no existe.";
            CargarTemas();
            return Page();
        }

        List<Pregunta> preguntas = Pregunta.ObtenerRosco(conexion, temaId);

        if (preguntas.Count != 27)
        {
            Error = "La selección todavía no contiene una pregunta para cada letra.";
            CargarTemas();
            return Page();
        }

        Partida partida = new Partida();
        foreach (Pregunta pregunta in preguntas)
        {
            partida.Preguntas.Add(new PreguntaPartida
            {
                Id = pregunta.Id,
                Letra = pregunta.Letra,
                Enunciado = pregunta.ObtenerEnunciado(),
                Respuesta = pregunta.Respuesta,
                Tema = pregunta.Tema.Nombre
            });
        }

        HttpContext.Session.SetString("partida", JsonSerializer.Serialize(partida));
        return RedirectToPage("/Jugar/Partida");
    }

    private void CargarTemas()
    {
        using SqliteConnection conexion = BaseDatos.Inicializar();
        Temas.Clear();
        LetrasTotales = Pregunta.ContarLetras(conexion, 0);

        foreach (Tema tema in Tema.Listar(conexion))
        {
            Temas.Add(new TemaConLetras
            {
                Tema = tema,
                Letras = Pregunta.ContarLetras(conexion, tema.Id)
            });
        }
    }
}

public class TemaConLetras
{
    public Tema Tema { get; set; } = new Tema();
    public int Letras { get; set; }
}

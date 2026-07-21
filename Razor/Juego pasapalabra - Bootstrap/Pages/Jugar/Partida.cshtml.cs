using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PasapalabraRazor.Pages.Jugar;

public class PartidaModel : PageModel
{
    public Partida? Partida { get; set; }
    public string Mensaje { get; set; } = "";
    public string TipoMensaje { get; set; } = "";

    public void OnGet()
    {
        Partida = LeerPartida();
    }

    public IActionResult OnPostResponder(string? respuesta)
    {
        Partida = LeerPartida();

        if (Partida == null)
        {
            return RedirectToPage("Index");
        }

        if (string.IsNullOrWhiteSpace(respuesta))
        {
            Mensaje = "Escribe una respuesta o pulsa Pasapalabra.";
            TipoMensaje = "aviso";
            return Page();
        }

        PreguntaPartida pregunta = Partida.PreguntaActual;
        if (TextoUtil.SonIguales(respuesta, pregunta.Respuesta))
        {
            pregunta.Estado = "correcta";
            Mensaje = "¡Respuesta correcta!";
            TipoMensaje = "correcto";
        }
        else
        {
            pregunta.Estado = "incorrecta";
            Mensaje = "La respuesta era: " + pregunta.Respuesta;
            TipoMensaje = "error";
        }

        Partida.Avanzar();
        GuardarPartida();
        return Page();
    }

    public IActionResult OnPostPasapalabra()
    {
        Partida = LeerPartida();

        if (Partida == null)
        {
            return RedirectToPage("Index");
        }

        Partida.Avanzar();
        GuardarPartida();
        return RedirectToPage();
    }

    public IActionResult OnPostAbandonar()
    {
        Partida = LeerPartida();

        if (Partida == null)
        {
            return RedirectToPage("Index");
        }

        Partida.Terminada = true;
        GuardarPartida();
        return RedirectToPage();
    }

    private Partida? LeerPartida()
    {
        string? json = HttpContext.Session.GetString("partida");
        return json == null ? null : JsonSerializer.Deserialize<Partida>(json);
    }

    private void GuardarPartida()
    {
        HttpContext.Session.SetString("partida", JsonSerializer.Serialize(Partida));
    }
}

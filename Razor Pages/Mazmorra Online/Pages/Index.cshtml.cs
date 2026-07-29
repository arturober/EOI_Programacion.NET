using MazmorraOnline.Dtos;
using MazmorraOnline.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MazmorraOnline.Pages;

// Permite escribir un nombre y entrar en la única partida disponible.
public class IndexModel : PageModel
{
    private readonly GestorJuego _gestorJuego;

    // BindProperty copia en Nombre el valor enviado por el formulario.
    [BindProperty]
    public string Nombre { get; set; } = "";

    public IndexModel(GestorJuego gestorJuego)
    {
        _gestorJuego = gestorJuego;
    }

    public IActionResult OnPost()
    {
        try
        {
            // El gestor valida el nombre y crea el jugador.
            AccesoJuegoRespuesta acceso =
                _gestorJuego.Entrar(Nombre);

            // El identificador se envía a la página del juego.
            return RedirectToPage(
                "/Juego",
                new { jugadorId = acceso.JugadorId });
        }
        catch (InvalidOperationException excepcion)
        {
            // El mensaje aparece debajo del campo Nombre.
            ModelState.AddModelError(
                nameof(Nombre), excepcion.Message);

            return Page();
        }
    }
}

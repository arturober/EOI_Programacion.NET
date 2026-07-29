using System.Text.Json;
using MazmorraOnline.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MazmorraOnline.Pages;

// Comprueba el jugador, prepara los mapas y permite abandonar la partida.
public class JuegoModel : PageModel
{
    private readonly GestorJuego _gestorJuego;

    // SupportsGet permite recibir jugadorId tanto por GET como por POST.
    [BindProperty(SupportsGet = true)]
    public string JugadorId { get; set; } = "";

    // Los mapas se insertan como JSON para dibujar los muros en el canvas.
    public string MapasJson { get; private set; } = "[]";

    public JuegoModel(GestorJuego gestorJuego)
    {
        _gestorJuego = gestorJuego;
    }

    public IActionResult OnGet()
    {
        // No se puede abrir el tablero sin un jugador válido.
        if (string.IsNullOrWhiteSpace(JugadorId)
            || !_gestorJuego.ExisteJugador(JugadorId))
        {
            return RedirectToPage("/Index");
        }

        // camelCase utiliza en JavaScript nombres como nombre y filas.
        MapasJson = JsonSerializer.Serialize(
            _gestorJuego.ObtenerMapas(),
            new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

        return Page();
    }

    public IActionResult OnPostSalir()
    {
        // El formulario del HUD llama a este método después de SweetAlert.
        if (!string.IsNullOrWhiteSpace(JugadorId))
        {
            _gestorJuego.EliminarJugador(JugadorId);
        }

        // Después de salir se vuelve a la página de entrada.
        return RedirectToPage("/Index");
    }
}

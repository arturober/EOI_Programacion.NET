using MazmorraOnline.Dtos;
using MazmorraOnline.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MazmorraOnline.Pages;

// Carga los mapas para que Razor pueda representarlos sin JavaScript.
public class MapasModel : PageModel
{
    private readonly GestorJuego _gestorJuego;

    // private set impide que otra clase sustituya accidentalmente la lista.
    public List<MapaDto> Mapas { get; private set; } = new();

    public MapasModel(GestorJuego gestorJuego)
    {
        _gestorJuego = gestorJuego;
    }

    public void OnGet()
    {
        // Razor utilizará esta lista mientras genera Mapas.cshtml.
        Mapas = _gestorJuego.ObtenerMapas();
    }
}

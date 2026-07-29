using MazmorraOnline.Dtos;
using MazmorraOnline.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MazmorraOnline.Pages;

// Muestra las últimas rondas utilizando HTML generado por Razor.
public class RondasModel : PageModel
{
    private readonly GestorJuego _gestorJuego;

    public List<ResultadoRondaDto> Resultados { get; private set; } = new();

    public RondasModel(GestorJuego gestorJuego)
    {
        _gestorJuego = gestorJuego;
    }

    public void OnGet()
    {
        // El gestor ya devuelve los resultados del más reciente al más antiguo.
        Resultados = _gestorJuego.ObtenerResultados();
    }
}

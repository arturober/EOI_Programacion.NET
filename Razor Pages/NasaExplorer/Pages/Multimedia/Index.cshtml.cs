using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NasaExplorer.DTOs;
using NasaExplorer.Servicios;

namespace NasaExplorer.Pages.Multimedia;

// Busca piezas del gran catálogo audiovisual público de NASA.
public class IndexModel(INasaServicio nasaServicio) : PageModel
{
    [BindProperty(SupportsGet = true)]
    public string Busqueda { get; set; } = "moon";

    [BindProperty(SupportsGet = true)]
    public string Tipo { get; set; } = "image";

    [BindProperty(SupportsGet = true)]
    public int? AnioDesde { get; set; }

    [BindProperty(SupportsGet = true)]
    public int? AnioHasta { get; set; }

    [BindProperty(SupportsGet = true)]
    public int Pagina { get; set; } = 1;

    public MediaRespuestaDto? Resultado { get; private set; }
    public string? Error { get; private set; }
    public int TotalPaginas =>
        Resultado is null ? 0 : (int)Math.Ceiling(Resultado.Coleccion.Metadatos.Total / 24d);

    public async Task OnGetAsync()
    {
        Busqueda = string.IsNullOrWhiteSpace(Busqueda) ? "moon" : Busqueda.Trim();
        Tipo = Tipo is "image" or "video" or "audio" or "todos" ? Tipo : "image";
        Pagina = Math.Max(1, Pagina);

        if (AnioDesde is not null && AnioHasta is not null && AnioDesde > AnioHasta)
        {
            Error = "El año inicial no puede ser posterior al año final.";
            return;
        }

        try
        {
            Resultado = await nasaServicio.BuscarMultimediaAsync(
                Busqueda,
                Tipo,
                AnioDesde,
                AnioHasta,
                Pagina);
        }
        catch (ApiExternaExcepcion excepcion)
        {
            Error = excepcion.Message;
        }
    }
}

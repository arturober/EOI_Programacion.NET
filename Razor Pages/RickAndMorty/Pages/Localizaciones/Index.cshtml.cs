using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RickAndMorty.DTOs;
using RickAndMorty.Modelos;
using RickAndMorty.Servicios;

namespace RickAndMorty.Pages.Localizaciones;

// Gestiona los filtros de nombre, tipo y dimensión.
public class IndexModel : PageModel
{
    private readonly IRickAndMortyServicio _servicio;

    public IndexModel(IRickAndMortyServicio servicio)
    {
        _servicio = servicio;
    }

    [BindProperty(SupportsGet = true)]
    public string? Nombre { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Tipo { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Dimension { get; set; }

    [BindProperty(SupportsGet = true)]
    public int Pagina { get; set; } = 1;

    public PaginaApiDto<LocalizacionDto> Resultado { get; private set; } =
        new();
    public PaginacionVista Paginacion { get; private set; } = new();
    public string? ErrorApi { get; private set; }

    public async Task OnGetAsync(
        CancellationToken cancellationToken)
    {
        Pagina = Math.Max(1, Pagina);

        try
        {
            Resultado = await _servicio.BuscarLocalizacionesAsync(
                Nombre,
                Tipo,
                Dimension,
                Pagina,
                cancellationToken);

            Paginacion = new PaginacionVista
            {
                PaginaRazor = "/Localizaciones/Index",
                PaginaActual = Pagina,
                TotalPaginas = Resultado.Informacion.TotalPaginas,
                Parametros = new Dictionary<string, string>
                {
                    ["nombre"] = Nombre ?? "",
                    ["tipo"] = Tipo ?? "",
                    ["dimension"] = Dimension ?? ""
                }
            };
        }
        catch (RickAndMortyApiExcepcion excepcion)
        {
            ErrorApi = excepcion.Message;
        }
    }
}

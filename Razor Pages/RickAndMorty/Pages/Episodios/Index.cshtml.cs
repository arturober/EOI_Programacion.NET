using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RickAndMorty.DTOs;
using RickAndMorty.Modelos;
using RickAndMorty.Servicios;

namespace RickAndMorty.Pages.Episodios;

// Permite buscar episodios por título o por código de temporada.
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
    public string? Codigo { get; set; }

    [BindProperty(SupportsGet = true)]
    public int Pagina { get; set; } = 1;

    public PaginaApiDto<EpisodioDto> Resultado { get; private set; } = new();
    public PaginacionVista Paginacion { get; private set; } = new();
    public string? ErrorApi { get; private set; }

    public async Task OnGetAsync(
        CancellationToken cancellationToken)
    {
        Pagina = Math.Max(1, Pagina);

        try
        {
            Resultado = await _servicio.BuscarEpisodiosAsync(
                Nombre,
                Codigo,
                Pagina,
                cancellationToken);

            Paginacion = new PaginacionVista
            {
                PaginaRazor = "/Episodios/Index",
                PaginaActual = Pagina,
                TotalPaginas = Resultado.Informacion.TotalPaginas,
                Parametros = new Dictionary<string, string>
                {
                    ["nombre"] = Nombre ?? "",
                    ["codigo"] = Codigo ?? ""
                }
            };
        }
        catch (RickAndMortyApiExcepcion excepcion)
        {
            ErrorApi = excepcion.Message;
        }
    }
}

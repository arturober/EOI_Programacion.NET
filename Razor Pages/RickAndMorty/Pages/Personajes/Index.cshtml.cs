using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RickAndMorty.DTOs;
using RickAndMorty.Modelos;
using RickAndMorty.Servicios;

namespace RickAndMorty.Pages.Personajes;

// Gestiona filtros y paginación del catálogo de personajes.
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
    public string? Estado { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Especie { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Genero { get; set; }

    [BindProperty(SupportsGet = true)]
    public int Pagina { get; set; } = 1;

    public PaginaApiDto<PersonajeDto> Resultado { get; private set; } = new();
    public PaginacionVista Paginacion { get; private set; } = new();
    public string? ErrorApi { get; private set; }

    public async Task OnGetAsync(
        CancellationToken cancellationToken)
    {
        Pagina = Math.Max(1, Pagina);

        try
        {
            Resultado = await _servicio.BuscarPersonajesAsync(
                Nombre,
                Estado,
                Especie,
                Genero,
                Pagina,
                cancellationToken);

            Paginacion = new PaginacionVista
            {
                PaginaRazor = "/Personajes/Index",
                PaginaActual = Pagina,
                TotalPaginas = Resultado.Informacion.TotalPaginas,
                Parametros = new Dictionary<string, string>
                {
                    ["nombre"] = Nombre ?? "",
                    ["estado"] = Estado ?? "",
                    ["especie"] = Especie ?? "",
                    ["genero"] = Genero ?? ""
                }
            };
        }
        catch (RickAndMortyApiExcepcion excepcion)
        {
            ErrorApi = excepcion.Message;
        }
    }
}

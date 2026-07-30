using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NasaExplorer.DTOs;
using NasaExplorer.Modelos;
using NasaExplorer.Servicios;

namespace NasaExplorer.Pages.Exoplanetas;

// Ejecuta consultas ADQL controladas contra la tabla actual pscomppars.
public class IndexModel(
    INasaServicio nasaServicio,
    IFavoritosServicio favoritosServicio,
    UserManager<Usuario> userManager) : PageModel
{
    [BindProperty(SupportsGet = true)]
    public string? Busqueda { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Metodo { get; set; }

    [BindProperty(SupportsGet = true)]
    public int? AnioDesde { get; set; }

    [BindProperty(SupportsGet = true)]
    public int Limite { get; set; } = 50;

    public List<ExoplanetaDto> Exoplanetas { get; private set; } = [];
    public HashSet<string> Favoritos { get; private set; } = [];
    public Dictionary<string, int> ResumenMetodos { get; private set; } = [];
    public string? Error { get; private set; }

    public async Task OnGetAsync()
    {
        Limite = Math.Clamp(Limite, 10, 100);

        try
        {
            Exoplanetas = await nasaServicio.BuscarExoplanetasAsync(
                Busqueda,
                Metodo,
                AnioDesde,
                Limite);

            ResumenMetodos = Exoplanetas
                .GroupBy(planeta => planeta.MetodoDescubrimiento ?? "Sin indicar")
                .OrderByDescending(grupo => grupo.Count())
                .ToDictionary(grupo => grupo.Key, grupo => grupo.Count());

            if (User.Identity?.IsAuthenticated == true)
            {
                string usuarioId = userManager.GetUserId(User)!;
                Favoritos = await favoritosServicio.ObtenerReferenciasAsync(
                    usuarioId,
                    "Exoplaneta");
            }
        }
        catch (ApiExternaExcepcion excepcion)
        {
            Error = excepcion.Message;
        }
    }

    public static string CrearUrlArchivo(string nombre)
    {
        return "https://exoplanetarchive.ipac.caltech.edu/overview/"
            + Uri.EscapeDataString(nombre);
    }
}

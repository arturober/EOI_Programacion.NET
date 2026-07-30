using Futbol.DTOs;
using Futbol.Modelos;
using Futbol.Servicios;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Futbol.Pages.Equipos;

// Reúne la ficha, los últimos resultados y los próximos partidos.
public class DetallesModel : PageModel
{
    private readonly IFutbolServicio _futbol;
    private readonly IFavoritosServicio _favoritos;
    private readonly UserManager<Usuario> _userManager;

    public DetallesModel(
        IFutbolServicio futbol,
        IFavoritosServicio favoritos,
        UserManager<Usuario> userManager)
    {
        _futbol = futbol;
        _favoritos = favoritos;
        _userManager = userManager;
    }

    [BindProperty(SupportsGet = true)]
    public int Id { get; set; }

    public EquipoDetalleDto? Equipo { get; private set; }
    public IReadOnlyList<PartidoDto> UltimosPartidos { get; private set; } = [];
    public IReadOnlyList<PartidoDto> ProximosPartidos { get; private set; } = [];
    public bool EsFavorito { get; private set; }
    public string? ErrorApi { get; private set; }

    // Solo devuelve direcciones HTTP válidas para el enlace externo.
    public string? SitioWebSeguro
    {
        get
        {
            if (Uri.TryCreate(
                Equipo?.SitioWeb,
                UriKind.Absolute,
                out Uri? direccion)
                && direccion.Scheme is "http" or "https")
            {
                return direccion.AbsoluteUri;
            }

            return null;
        }
    }

    public async Task<IActionResult> OnGetAsync(
        CancellationToken cancellationToken)
    {
        if (Id <= 0)
        {
            return RedirectToPage("/Competiciones/Index");
        }

        try
        {
            // Las llamadas son independientes y sus resultados quedan en caché.
            Task<EquipoDetalleDto> tareaEquipo =
                _futbol.ObtenerEquipoAsync(Id, cancellationToken);
            Task<IReadOnlyList<PartidoDto>> tareaUltimos =
                _futbol.ObtenerPartidosEquipoAsync(
                    Id,
                    "FINISHED",
                    cancellationToken);
            Task<IReadOnlyList<PartidoDto>> tareaProximos =
                _futbol.ObtenerPartidosEquipoAsync(
                    Id,
                    "SCHEDULED",
                    cancellationToken);

            await Task.WhenAll(tareaEquipo, tareaUltimos, tareaProximos);

            Equipo = await tareaEquipo;
            UltimosPartidos = await tareaUltimos;
            ProximosPartidos = await tareaProximos;

            string? usuarioId = _userManager.GetUserId(User);
            if (usuarioId is not null)
            {
                EsFavorito = await _favoritos.ContieneAsync(
                    usuarioId,
                    Id,
                    cancellationToken);
            }
        }
        catch (FutbolApiExcepcion excepcion)
        {
            ErrorApi = excepcion.Message;
        }

        return Page();
    }
}

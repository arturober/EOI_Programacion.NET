using Futbol.DTOs;
using Futbol.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Futbol.Pages.Partidos;

// Permite consultar los partidos de un día concreto.
public class IndexModel : PageModel
{
    private readonly IFutbolServicio _futbol;

    public IndexModel(IFutbolServicio futbol)
    {
        _futbol = futbol;
    }

    [BindProperty(SupportsGet = true)]
    public DateOnly? Fecha { get; set; }

    public IReadOnlyList<PartidoDto> Partidos { get; private set; } = [];

    public string? ErrorApi { get; private set; }

    public async Task OnGetAsync(
        CancellationToken cancellationToken)
    {
        Fecha ??= DateOnly.FromDateTime(DateTime.Today);

        // Se limita el calendario para evitar consultas accidentales absurdas.
        DateOnly minima = DateOnly.FromDateTime(
            DateTime.Today.AddYears(-5));
        DateOnly maxima = DateOnly.FromDateTime(
            DateTime.Today.AddYears(2));

        if (Fecha < minima || Fecha > maxima)
        {
            ErrorApi =
                "La fecha debe estar entre hace cinco años y dentro de dos años.";
            return;
        }

        try
        {
            Partidos = await _futbol.ObtenerPartidosPorFechaAsync(
                Fecha.Value,
                cancellationToken);
        }
        catch (FutbolApiExcepcion excepcion)
        {
            ErrorApi = excepcion.Message;
        }
    }
}

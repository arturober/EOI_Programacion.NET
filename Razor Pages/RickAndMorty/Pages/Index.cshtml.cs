using Microsoft.AspNetCore.Mvc.RazorPages;
using RickAndMorty.DTOs;
using RickAndMorty.Servicios;

namespace RickAndMorty.Pages;

// Prepara las cifras generales y una selección de personajes.
public class IndexModel : PageModel
{
    private readonly IRickAndMortyServicio _servicio;

    public IndexModel(IRickAndMortyServicio servicio)
    {
        _servicio = servicio;
    }

    public IReadOnlyList<PersonajeDto> Personajes { get; private set; } = [];
    public int TotalPersonajes { get; private set; }
    public int TotalEpisodios { get; private set; }
    public int TotalLocalizaciones { get; private set; }
    public string? ErrorApi { get; private set; }

    public async Task OnGetAsync(
        CancellationToken cancellationToken)
    {
        try
        {
            // Las tres consultas son independientes y se guardan en caché.
            Task<PaginaApiDto<PersonajeDto>> tareaPersonajes =
                _servicio.BuscarPersonajesAsync(
                    null, null, null, null, 1, cancellationToken);
            Task<PaginaApiDto<EpisodioDto>> tareaEpisodios =
                _servicio.BuscarEpisodiosAsync(
                    null, null, 1, cancellationToken);
            Task<PaginaApiDto<LocalizacionDto>> tareaLocalizaciones =
                _servicio.BuscarLocalizacionesAsync(
                    null, null, null, 1, cancellationToken);

            await Task.WhenAll(
                tareaPersonajes,
                tareaEpisodios,
                tareaLocalizaciones);

            PaginaApiDto<PersonajeDto> personajes =
                await tareaPersonajes;
            PaginaApiDto<EpisodioDto> episodios =
                await tareaEpisodios;
            PaginaApiDto<LocalizacionDto> localizaciones =
                await tareaLocalizaciones;

            Personajes = personajes.Resultados.Take(6).ToList().AsReadOnly();
            TotalPersonajes = personajes.Informacion.TotalResultados;
            TotalEpisodios = episodios.Informacion.TotalResultados;
            TotalLocalizaciones =
                localizaciones.Informacion.TotalResultados;
        }
        catch (RickAndMortyApiExcepcion excepcion)
        {
            ErrorApi = excepcion.Message;
        }
    }
}

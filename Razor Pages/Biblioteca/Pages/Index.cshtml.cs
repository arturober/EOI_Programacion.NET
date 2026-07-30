using Biblioteca.Configuracion;
using Biblioteca.Modelos;
using Biblioteca.Servicios;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace Biblioteca.Pages;

// Prepara los tres escaparates de la página de inicio.
public class IndexModel : PageModel
{
    private readonly IOpenLibraryServicio _openLibrary;
    private readonly IFavoritosServicio _favoritos;
    private readonly UserManager<Usuario> _userManager;
    private readonly OpenLibraryOpciones _opciones;

    public IndexModel(
        IOpenLibraryServicio openLibrary,
        IFavoritosServicio favoritos,
        UserManager<Usuario> userManager,
        IOptions<OpenLibraryOpciones> opciones)
    {
        _openLibrary = openLibrary;
        _favoritos = favoritos;
        _userManager = userManager;
        _opciones = opciones.Value;
    }

    public IReadOnlyList<LibroResumen> Tendencias { get; private set; } = [];
    public IReadOnlyList<LibroResumen> MejorValorados { get; private set; } = [];
    public IReadOnlyList<LibroResumen> Programacion { get; private set; } = [];
    public bool ContactoConfigurado => _opciones.TieneContactoReal;

    public async Task OnGetAsync()
    {
        try
        {
            // Las llamadas son secuenciales para respetar el límite de la API.
            PaginaLibros tendencias =
                await _openLibrary.ObtenerListadoAsync(
                    TipoListado.Tendencias,
                    cancellationToken: HttpContext.RequestAborted);

            PaginaLibros mejorValorados =
                await _openLibrary.ObtenerListadoAsync(
                    TipoListado.MejorValorados,
                    cancellationToken: HttpContext.RequestAborted);

            PaginaLibros programacion =
                await _openLibrary.ObtenerListadoAsync(
                    TipoListado.Programacion,
                    cancellationToken: HttpContext.RequestAborted);

            Tendencias = tendencias.Resultados.Take(6).ToList().AsReadOnly();
            MejorValorados =
                mejorValorados.Resultados.Take(6).ToList().AsReadOnly();
            Programacion =
                programacion.Resultados.Take(6).ToList().AsReadOnly();

            await MarcarFavoritosAsync(
                Tendencias.Concat(MejorValorados).Concat(Programacion));
        }
        catch (OpenLibraryExcepcion excepcion)
        {
            ViewData["Error"] = excepcion.Message;
        }
    }

    private async Task MarcarFavoritosAsync(
        IEnumerable<LibroResumen> libros)
    {
        string? usuarioId = _userManager.GetUserId(User);
        if (usuarioId is null)
        {
            return;
        }

        HashSet<string> ids = await _favoritos.ObtenerIdsAsync(
            usuarioId,
            HttpContext.RequestAborted);

        foreach (LibroResumen libro in libros)
        {
            libro.EsFavorito = ids.Contains(libro.Id);
        }
    }
}

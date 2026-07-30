using Biblioteca.Modelos;
using Biblioteca.Servicios;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Biblioteca.Pages.Libros;

// Muestra una de las colecciones disponibles en el menú Explorar.
public class IndexModel : PageModel
{
    private readonly IOpenLibraryServicio _openLibrary;
    private readonly IFavoritosServicio _favoritos;
    private readonly UserManager<Usuario> _userManager;

    public IndexModel(
        IOpenLibraryServicio openLibrary,
        IFavoritosServicio favoritos,
        UserManager<Usuario> userManager)
    {
        _openLibrary = openLibrary;
        _favoritos = favoritos;
        _userManager = userManager;
    }

    public TipoListado Tipo { get; private set; }
    public PaginaLibros Resultado { get; private set; } = new();

    public async Task OnGetAsync(string? tipo, int pagina = 1)
    {
        Tipo = TipoListadoExtensiones.DesdeTexto(tipo);

        try
        {
            Resultado = await _openLibrary.ObtenerListadoAsync(
                Tipo,
                pagina,
                HttpContext.RequestAborted);

            await MarcarFavoritosAsync();
        }
        catch (OpenLibraryExcepcion excepcion)
        {
            ViewData["Error"] = excepcion.Message;
        }
    }

    private async Task MarcarFavoritosAsync()
    {
        string? usuarioId = _userManager.GetUserId(User);
        if (usuarioId is null)
        {
            return;
        }

        HashSet<string> ids = await _favoritos.ObtenerIdsAsync(
            usuarioId,
            HttpContext.RequestAborted);

        foreach (LibroResumen libro in Resultado.Resultados)
        {
            libro.EsFavorito = ids.Contains(libro.Id);
        }
    }
}

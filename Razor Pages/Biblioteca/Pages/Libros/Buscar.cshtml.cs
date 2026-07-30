using Biblioteca.Modelos;
using Biblioteca.Servicios;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Biblioteca.Pages.Libros;

// Atiende las búsquedas por título, autor, ISBN u otros campos admitidos.
public class BuscarModel : PageModel
{
    private readonly IOpenLibraryServicio _openLibrary;
    private readonly IFavoritosServicio _favoritos;
    private readonly UserManager<Usuario> _userManager;

    public BuscarModel(
        IOpenLibraryServicio openLibrary,
        IFavoritosServicio favoritos,
        UserManager<Usuario> userManager)
    {
        _openLibrary = openLibrary;
        _favoritos = favoritos;
        _userManager = userManager;
    }

    public string Texto { get; private set; } = "";
    public PaginaLibros Resultado { get; private set; } = new();

    public async Task OnGetAsync(string? texto, int pagina = 1)
    {
        Texto = texto?.Trim() ?? "";

        if (Texto.Length < 2)
        {
            if (Texto.Length > 0)
            {
                ViewData["Error"] =
                    "La búsqueda debe contener al menos dos caracteres.";
            }

            return;
        }

        try
        {
            Resultado = await _openLibrary.BuscarAsync(
                Texto,
                pagina,
                HttpContext.RequestAborted);

            string? usuarioId = _userManager.GetUserId(User);
            if (usuarioId is not null)
            {
                HashSet<string> ids = await _favoritos.ObtenerIdsAsync(
                    usuarioId,
                    HttpContext.RequestAborted);

                foreach (LibroResumen libro in Resultado.Resultados)
                {
                    libro.EsFavorito = ids.Contains(libro.Id);
                }
            }
        }
        catch (OpenLibraryExcepcion excepcion)
        {
            ViewData["Error"] = excepcion.Message;
        }
    }
}

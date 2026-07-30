using System.Net;
using Biblioteca.Modelos;
using Biblioteca.Servicios;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Biblioteca.Pages.Libros;

// Obtiene la obra, sus metadatos y varias recomendaciones relacionadas.
public class DetallesModel : PageModel
{
    private readonly IOpenLibraryServicio _openLibrary;
    private readonly IFavoritosServicio _favoritos;
    private readonly UserManager<Usuario> _userManager;

    public DetallesModel(
        IOpenLibraryServicio openLibrary,
        IFavoritosServicio favoritos,
        UserManager<Usuario> userManager)
    {
        _openLibrary = openLibrary;
        _favoritos = favoritos;
        _userManager = userManager;
    }

    public LibroDetalle Libro { get; private set; } = new();

    public async Task<IActionResult> OnGetAsync(string id)
    {
        try
        {
            Libro = await _openLibrary.ObtenerDetalleAsync(
                id,
                HttpContext.RequestAborted);

            string? usuarioId = _userManager.GetUserId(User);
            if (usuarioId is not null)
            {
                HashSet<string> ids = await _favoritos.ObtenerIdsAsync(
                    usuarioId,
                    HttpContext.RequestAborted);

                Libro.EsFavorito = ids.Contains(Libro.Id);
                foreach (LibroResumen recomendado in Libro.Recomendaciones)
                {
                    recomendado.EsFavorito = ids.Contains(recomendado.Id);
                }
            }

            return Page();
        }
        catch (OpenLibraryExcepcion excepcion)
            when (excepcion.CodigoEstado == HttpStatusCode.NotFound)
        {
            return NotFound();
        }
        catch (OpenLibraryExcepcion excepcion)
        {
            ViewData["Error"] = excepcion.Message;
            return Page();
        }
    }
}

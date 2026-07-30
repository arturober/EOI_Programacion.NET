using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NasaExplorer.Modelos;
using NasaExplorer.Servicios;

namespace NasaExplorer.Pages.Favoritos;

// Recibe un elemento desde cualquier módulo y lo añade o quita de SQLite.
[Authorize]
public class AlternarModel(
    UserManager<Usuario> userManager,
    IFavoritosServicio favoritosServicio) : PageModel
{
    public IActionResult OnGet()
    {
        return RedirectToPage("/Favoritos/Index");
    }

    public async Task<IActionResult> OnPostAsync(
        string tipo,
        string referencia,
        string titulo,
        string? subtitulo,
        string? imagenUrl,
        string? urlDetalle,
        string? returnUrl)
    {
        string? usuarioId = userManager.GetUserId(User);
        if (usuarioId is null)
        {
            return Challenge();
        }

        string[] tiposPermitidos =
        [
            "APOD",
            "Multimedia",
            "EPIC",
            "EONET",
            "Asteroide",
            "DONKI",
            "Exoplaneta"
        ];

        if (!tiposPermitidos.Contains(tipo)
            || string.IsNullOrWhiteSpace(referencia)
            || string.IsNullOrWhiteSpace(titulo))
        {
            TempData["Mensaje"] = "El favorito no contiene datos válidos.";
            TempData["TipoMensaje"] = "error";
            return Volver(returnUrl);
        }

        Favorito favorito = new()
        {
            Tipo = Recortar(tipo, 40),
            Referencia = Recortar(referencia, 250),
            Titulo = Recortar(titulo, 300),
            Subtitulo = RecortarOpcional(subtitulo, 500),
            ImagenUrl = ValidarUrl(imagenUrl),
            UrlDetalle = ValidarUrlOEnlaceLocal(urlDetalle)
        };

        bool guardado = await favoritosServicio.AlternarAsync(usuarioId, favorito);
        TempData["Mensaje"] = guardado
            ? "Elemento añadido a tus favoritos."
            : "Elemento eliminado de tus favoritos.";
        TempData["TipoMensaje"] = guardado ? "success" : "info";
        return Volver(returnUrl);
    }

    private IActionResult Volver(string? returnUrl)
    {
        return Url.IsLocalUrl(returnUrl)
            ? LocalRedirect(returnUrl)
            : RedirectToPage("/Favoritos/Index");
    }

    private static string Recortar(string texto, int maximo)
    {
        string limpio = texto.Trim();
        return limpio.Length <= maximo ? limpio : limpio[..maximo];
    }

    private static string? RecortarOpcional(string? texto, int maximo)
    {
        return string.IsNullOrWhiteSpace(texto) ? null : Recortar(texto, maximo);
    }

    private static string? ValidarUrl(string? texto)
    {
        if (Uri.TryCreate(texto, UriKind.Absolute, out Uri? uri)
            && uri.Scheme is "https" or "http")
        {
            return Recortar(uri.ToString(), 2000);
        }

        return null;
    }

    private string? ValidarUrlOEnlaceLocal(string? texto)
    {
        return Url.IsLocalUrl(texto) ? Recortar(texto!, 2000) : ValidarUrl(texto);
    }
}

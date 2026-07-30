using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NasaExplorer.DTOs;
using NasaExplorer.Modelos;
using NasaExplorer.Servicios;

namespace NasaExplorer.Pages.Multimedia;

// Reúne metadatos y manifiesto de archivos de una pieza del catálogo.
public class DetalleModel(
    INasaServicio nasaServicio,
    IFavoritosServicio favoritosServicio,
    UserManager<Usuario> userManager) : PageModel
{
    public MediaItemDto? Elemento { get; private set; }
    public List<string> Archivos { get; private set; } = [];
    public string? Error { get; private set; }
    public bool EsFavorito { get; private set; }
    public string? ArchivoReproducible { get; private set; }
    public string? ArchivoOriginal { get; private set; }

    public async Task<IActionResult> OnGetAsync(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return RedirectToPage("/Multimedia/Index");
        }

        try
        {
            Elemento = await nasaServicio.ObtenerMultimediaAsync(id);
            if (Elemento is null)
            {
                Error = "No se ha encontrado esa pieza multimedia.";
                return Page();
            }

            Archivos = await nasaServicio.ObtenerArchivosMultimediaAsync(id);
            SeleccionarArchivos(Elemento.DatosPrincipales.TipoMedio);

            if (User.Identity?.IsAuthenticated == true)
            {
                string usuarioId = userManager.GetUserId(User)!;
                HashSet<string> referencias =
                    await favoritosServicio.ObtenerReferenciasAsync(usuarioId, "Multimedia");
                EsFavorito = referencias.Contains(id);
            }
        }
        catch (ApiExternaExcepcion excepcion)
        {
            Error = excepcion.Message;
        }

        return Page();
    }

    private void SeleccionarArchivos(string tipo)
    {
        ArchivoOriginal = Archivos.FirstOrDefault(archivo =>
            archivo.Contains("~orig", StringComparison.OrdinalIgnoreCase))
            ?? Archivos.FirstOrDefault();

        string[] extensiones = tipo switch
        {
            "video" => [".mp4", ".webm"],
            "audio" => [".mp3", ".wav", ".m4a"],
            _ => [".jpg", ".jpeg", ".png"]
        };

        ArchivoReproducible = Archivos.FirstOrDefault(archivo =>
            extensiones.Any(extension =>
                archivo.EndsWith(extension, StringComparison.OrdinalIgnoreCase)));
    }
}

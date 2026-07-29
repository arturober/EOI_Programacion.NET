using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Peliculas.Pages;

// Evita mostrar detalles internos cuando se produce un error en producción.
[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
public class ErrorModel : PageModel
{
    public string? IdentificadorPeticion { get; private set; }

    public bool MostrarIdentificador =>
        !string.IsNullOrWhiteSpace(IdentificadorPeticion);

    public void OnGet()
    {
        IdentificadorPeticion =
            Activity.Current?.Id ?? HttpContext.TraceIdentifier;
    }
}

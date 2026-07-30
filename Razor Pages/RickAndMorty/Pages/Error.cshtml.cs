using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RickAndMorty.Pages;

// Muestra un identificador útil sin revelar detalles internos.
[ResponseCache(
    Duration = 0,
    Location = ResponseCacheLocation.None,
    NoStore = true)]
public class ErrorModel : PageModel
{
    public string? RequestId { get; set; }

    public bool MostrarRequestId =>
        !string.IsNullOrWhiteSpace(RequestId);

    public void OnGet()
    {
        RequestId = Activity.Current?.Id
            ?? HttpContext.TraceIdentifier;
    }
}

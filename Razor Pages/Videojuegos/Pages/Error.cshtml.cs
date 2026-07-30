using System.Diagnostics;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Videojuegos.Pages;

// Ofrece una pantalla segura sin mostrar detalles técnicos al usuario.
[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
public class ErrorModel : PageModel
{
    public string? IdPeticion { get; private set; }
    public bool MostrarIdPeticion => !string.IsNullOrWhiteSpace(IdPeticion);

    public void OnGet()
    {
        IdPeticion = Activity.Current?.Id ?? HttpContext.TraceIdentifier;

        // La excepción se consulta solo para que el depurador pueda inspeccionarla.
        _ = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
    }
}

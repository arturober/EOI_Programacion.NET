using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PokeApiRazor.Pages;

// Esta página evita mostrar detalles técnicos de una excepción en producción.
public class ErrorModel : PageModel
{
    // El identificador permite localizar el error en los registros del servidor.
    public string? IdPeticion { get; private set; }

    // Indica si existe un identificador que merezca la pena mostrar.
    public bool MostrarIdPeticion => !string.IsNullOrEmpty(IdPeticion);

    // OnGet se ejecuta cuando el navegador abre la página de error.
    public void OnGet()
    {
        IdPeticion = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
    }
}

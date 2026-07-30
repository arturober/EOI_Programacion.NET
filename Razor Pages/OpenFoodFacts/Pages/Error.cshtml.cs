using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace OpenFoodFacts.Pages;

// Presenta un identificador que facilita localizar el error en los registros.
public class ErrorModel : PageModel
{
    public string? IdPeticion { get; private set; }
    public bool MostrarIdPeticion =>
        !string.IsNullOrWhiteSpace(IdPeticion);

    public void OnGet()
    {
        IdPeticion = Activity.Current?.Id
            ?? HttpContext.TraceIdentifier;
    }
}

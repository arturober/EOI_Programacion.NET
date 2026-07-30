using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace NasaExplorer.Pages;

// El identificador ayuda a localizar el error en los registros del servidor.
public class ErrorModel : PageModel
{
    public string? IdPeticion { get; set; }
    public bool MostrarId => !string.IsNullOrEmpty(IdPeticion);

    public void OnGet()
    {
        IdPeticion = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
    }
}

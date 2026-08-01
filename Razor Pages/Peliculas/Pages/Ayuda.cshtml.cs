using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Peliculas.Pages;

// Prepara la dirección base para mostrar enlaces válidos en cualquier servidor.
public class AyudaModel : PageModel
{
    public string UrlBase { get; private set; } = "";

    public void OnGet()
    {
        UrlBase = $"{Request.Scheme}://{Request.Host}";
    }
}

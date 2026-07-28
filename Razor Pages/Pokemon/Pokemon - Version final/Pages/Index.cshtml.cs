using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Pokemon.Pages;

// La portada no necesita consultar la API, por lo que su PageModel está vacío.
public class IndexModel : PageModel
{
    // OnGet se ejecuta cuando se abre la página de inicio.
    public void OnGet()
    {
    }
}

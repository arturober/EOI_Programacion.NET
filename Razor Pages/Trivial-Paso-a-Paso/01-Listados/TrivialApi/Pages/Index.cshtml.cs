using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TrivialApi.Data;

namespace TrivialApi.Pages;

// El PageModel contiene la lógica que prepara los datos de la página de inicio.
public class IndexModel(TrivialContext contexto) : PageModel
{
    // Estas propiedades se muestran después desde el archivo Index.cshtml.
    public int TotalPreguntas { get; set; }
    public int TotalCategorias { get; set; }

    public async Task OnGetAsync()
    {
        // CountAsync pide a SQLite que cuente las filas sin cargarlas en memoria.
        TotalPreguntas = await contexto.Preguntas.CountAsync();
        TotalCategorias = await contexto.Categorias.CountAsync();
    }
}


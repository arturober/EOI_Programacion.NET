using System.Globalization;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TrivialApi.Data;
using TrivialApi.Models;

namespace TrivialApi.Pages.Preguntas;

// Esta versión amplía el listado anterior con filtros y paginación.
public class IndexModel(TrivialContext contexto) : PageModel
{
    // Centralizamos el tamaño para no repetir el número en varios cálculos.
    private const int TamanoPagina = 25;

    // La vista recorrerá esta lista, que solo contiene la página actual.
    public List<Pregunta> Preguntas { get; set; } = [];

    // SelectList prepara las opciones del filtro de categorías.
    public SelectList Categorias { get; set; } = default!;

    // Estas propiedades permiten explicar al usuario el resultado de la consulta.
    public int TotalResultados { get; set; }
    public int TotalPaginas { get; set; }

    // SupportsGet obtiene los valores desde la cadena de consulta de la URL.
    // Por ejemplo: /Preguntas?Busqueda=historia&CategoriaId=2&Pagina=3.
    [BindProperty(SupportsGet = true)]
    public string? Busqueda { get; set; }

    [BindProperty(SupportsGet = true)]
    public int? CategoriaId { get; set; }

    [BindProperty(SupportsGet = true)]
    public int Pagina { get; set; } = 1;

    public async Task OnGetAsync()
    {
        // IQueryable representa una consulta que todavía no se ha ejecutado.
        // Conservamos la consulta original y añadimos filtros alrededor de ella.
        IQueryable<Pregunta> consulta = contexto.Preguntas
            .Include(pregunta => pregunta.Categoria);

        // SQLite puede aplicar directamente el filtro de clave foránea.
        if (CategoriaId.HasValue)
        {
            consulta = consulta.Where(
                pregunta => pregunta.CategoriaId == CategoriaId);
        }

        // Cargamos como máximo las 1.000 preguntas de la base para poder aplicar
        // en C# una normalización que ignore simultáneamente mayúsculas y tildes.
        List<Pregunta> resultados = await consulta.ToListAsync();

        if (!string.IsNullOrWhiteSpace(Busqueda))
        {
            // Trim elimina espacios accidentales al principio y al final.
            string textoBuscado = Normalizar(Busqueda.Trim());

            resultados = resultados
                .Where(pregunta =>
                    Normalizar(pregunta.Enunciado).Contains(textoBuscado))
                .ToList();
        }

        // Contamos después de filtrar para que la paginación sea correcta.
        TotalResultados = resultados.Count;

        // Ceiling redondea hacia arriba. Math.Max mantiene al menos una página
        // incluso cuando la búsqueda no devuelve ninguna pregunta.
        TotalPaginas = Math.Max(
            1,
            (int)Math.Ceiling(TotalResultados / (double)TamanoPagina));

        // Clamp corrige números de página negativos o superiores al último.
        Pagina = Math.Clamp(Pagina, 1, TotalPaginas);

        // Skip descarta las páginas anteriores y Take conserva solo 25 filas.
        Preguntas = resultados
            .OrderBy(pregunta => pregunta.Enunciado)
            .Skip((Pagina - 1) * TamanoPagina)
            .Take(TamanoPagina)
            .ToList();

        // El selector se carga siempre, independientemente de los resultados.
        Categorias = new SelectList(
            await contexto.Categorias
                .OrderBy(categoria => categoria.Nombre)
                .ToListAsync(),
            "Id",
            "Nombre");
    }

    private static string Normalizar(string texto)
    {
        // FormD separa una vocal de su tilde: "á" se convierte en "a" + tilde.
        string textoSeparado = texto.Normalize(NormalizationForm.FormD);

        // StringBuilder evita crear una cadena nueva en cada vuelta del bucle.
        StringBuilder resultado = new();

        foreach (char caracter in textoSeparado)
        {
            // NonSpacingMark identifica la tilde separada, que no copiamos.
            if (CharUnicodeInfo.GetUnicodeCategory(caracter) !=
                UnicodeCategory.NonSpacingMark)
            {
                // Convertimos el resto a minúsculas de forma independiente
                // de la configuración regional del equipo.
                resultado.Append(char.ToLowerInvariant(caracter));
            }
        }

        return resultado.ToString();
    }

    public async Task<IActionResult> OnPostEliminarAsync(int id)
    {
        // Buscamos la entidad por su clave primaria.
        Pregunta? pregunta = await contexto.Preguntas.FindAsync(id);

        // Si otro usuario ya la hubiera eliminado, no intentamos borrarla de nuevo.
        if (pregunta is not null)
        {
            // Remove marca la pregunta como eliminada dentro del contexto.
            contexto.Preguntas.Remove(pregunta);

            // SaveChangesAsync ejecuta el DELETE pendiente en SQLite.
            await contexto.SaveChangesAsync();

            TempData["Mensaje"] =
                "La pregunta se ha eliminado correctamente.";
        }

        // Conservamos los filtros y la página que estaban activos antes del borrado.
        return RedirectToPage(new
        {
            busqueda = Busqueda,
            categoriaId = CategoriaId,
            pagina = Pagina
        });
    }
}

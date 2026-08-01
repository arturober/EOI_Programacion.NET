using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TrivialApi.Data;
using TrivialApi.Models;

namespace TrivialApi.Pages.Preguntas;

// Esta primera versión muestra una selección limitada de preguntas.
public class IndexModel(TrivialContext contexto) : PageModel
{
    // La vista recorrerá esta lista para construir las filas de la tabla.
    public List<Pregunta> Preguntas { get; set; } = [];

    public async Task OnGetAsync()
    {
        // IQueryable representa una consulta que todavía no se ha ejecutado.
        // Include indica que también necesitaremos el nombre de la categoría.
        IQueryable<Pregunta> consulta = contexto.Preguntas
            .Include(pregunta => pregunta.Categoria);

        // OrderBy, Take y ToListAsync se traducen a SQL.
        // Limitamos el resultado a 25 filas para no mostrar las 1.000 preguntas.
        Preguntas = await consulta
            .OrderBy(pregunta => pregunta.Enunciado)
            .Take(25)
            .ToListAsync();
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

        // Volvemos al listado utilizando el patrón Post/Redirect/Get.
        return RedirectToPage();
    }
}

using AgendaTelefonos.Datos;
using AgendaTelefonos.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.Sqlite;

namespace AgendaTelefonos.Pages;

public class IndexModel : PageModel
{
    public List<Persona> Personas { get; private set; } = new List<Persona>();

    [BindProperty(SupportsGet = true)]
    public string Busqueda { get; set; } = "";

    public void OnGet()
    {
        using SqliteConnection conexion = BaseDatos.Inicializar();
        Personas = Persona.Listar(conexion, Busqueda);
    }

    public IActionResult OnPostEliminar(int id)
    {
        using SqliteConnection conexion = BaseDatos.Inicializar();

        Persona? persona = Persona.BuscarPorId(conexion, id);

        if (persona == null || !persona.Borrar(conexion))
        {
            TempData["Error"] = "El contacto ya no existe.";
        }
        else
        {
            TempData["Mensaje"] = "El contacto se ha eliminado correctamente.";
        }

        return RedirectToPage(new { busqueda = Busqueda });
    }
}

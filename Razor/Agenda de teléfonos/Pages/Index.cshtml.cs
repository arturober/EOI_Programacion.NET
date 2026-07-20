using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AgendaContactosWeb.Models;
using Microsoft.Data.Sqlite;

namespace Agenda_de_teléfonos.Pages;

public class IndexModel : PageModel
{
    public List<Persona> Personas { get; set; } = new List<Persona>();

    public void OnGet()
    {
        using SqliteConnection conexion = BaseDatos.Inicializar();
        Personas = Persona.Listar(conexion);
    }
}

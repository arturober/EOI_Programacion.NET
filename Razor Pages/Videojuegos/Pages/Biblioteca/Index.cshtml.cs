using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Videojuegos.Modelos;
using Videojuegos.Servicios;

namespace Videojuegos.Pages.Biblioteca;

// Lee la colección de SQLite y permite filtrarla sin llamar a RAWG.
[Authorize]
public class IndexModel : PageModel
{
    private readonly IBibliotecaServicio _biblioteca;
    private readonly UserManager<Usuario> _userManager;

    public IndexModel(
        IBibliotecaServicio biblioteca,
        UserManager<Usuario> userManager)
    {
        _biblioteca = biblioteca;
        _userManager = userManager;
    }

    public IReadOnlyList<VideojuegoUsuario> Elementos { get; private set; } = [];
    public IReadOnlyList<VideojuegoUsuario> Todos { get; private set; } = [];
    public string Filtro { get; private set; } = "todos";
    public string Orden { get; private set; } = "recientes";

    public async Task OnGetAsync(string? filtro, string? orden)
    {
        Filtro = NormalizarFiltro(filtro);
        Orden = NormalizarOrden(orden);

        string usuarioId = _userManager.GetUserId(User)!;
        Todos = await _biblioteca.ListarAsync(
            usuarioId,
            HttpContext.RequestAborted);

        IEnumerable<VideojuegoUsuario> consulta = Todos;

        if (Enum.TryParse(
            Filtro,
            ignoreCase: true,
            out EstadoVideojuego estado))
        {
            consulta = consulta.Where(elemento => elemento.Estado == estado);
        }

        consulta = Orden switch
        {
            "titulo" => consulta.OrderBy(
                elemento => elemento.Videojuego.Nombre),
            "lanzamiento" => consulta.OrderByDescending(
                elemento => elemento.Videojuego.FechaLanzamiento),
            "puntuacion" => consulta.OrderByDescending(
                elemento => elemento.PuntuacionPersonal),
            _ => consulta.OrderByDescending(
                elemento => elemento.FechaActualizadoUtc)
        };

        Elementos = consulta.ToList().AsReadOnly();
    }

    public int Contar(EstadoVideojuego estado) =>
        Todos.Count(elemento => elemento.Estado == estado);

    private static string NormalizarFiltro(string? filtro)
    {
        return filtro?.ToLowerInvariant() switch
        {
            "pendiente" => "Pendiente",
            "jugando" => "Jugando",
            "completado" => "Completado",
            "abandonado" => "Abandonado",
            _ => "todos"
        };
    }

    private static string NormalizarOrden(string? orden)
    {
        return orden?.ToLowerInvariant() switch
        {
            "titulo" => "titulo",
            "lanzamiento" => "lanzamiento",
            "puntuacion" => "puntuacion",
            _ => "recientes"
        };
    }
}

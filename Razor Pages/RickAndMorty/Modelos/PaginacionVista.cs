namespace RickAndMorty.Modelos;

// Reúne lo necesario para reutilizar el componente de paginación.
public class PaginacionVista
{
    public string PaginaRazor { get; set; } = "";
    public int PaginaActual { get; set; }
    public int TotalPaginas { get; set; }

    // Conserva los filtros al cambiar de página.
    public Dictionary<string, string> Parametros { get; set; } = [];
}

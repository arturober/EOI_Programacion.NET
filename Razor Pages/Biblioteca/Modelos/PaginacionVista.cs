namespace Biblioteca.Modelos;

// Reúne los datos que necesita el componente común de paginación.
public class PaginacionVista
{
    public int PaginaActual { get; set; }
    public int TotalPaginas { get; set; }
    public string PaginaRazor { get; set; } = "";
    public Dictionary<string, string> ValoresRuta { get; set; } = [];
}

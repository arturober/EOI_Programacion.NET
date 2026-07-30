namespace Biblioteca.Configuracion;

// Reúne en una sola clase la configuración del servicio externo.
public class OpenLibraryOpciones
{
    public const string Seccion = "OpenLibrary";

    public string NombreAplicacion { get; set; } = "BibliotecaRazor";
    public string Contacto { get; set; } = "";
    public string Idioma { get; set; } = "es";
    public int MinutosCache { get; set; } = 30;
    public int TamanoPagina { get; set; } = 20;

    // Open Library concede un límite mayor a peticiones identificadas.
    public bool TieneContactoReal =>
        !string.IsNullOrWhiteSpace(Contacto)
        && !Contacto.Contains("ejemplo", StringComparison.OrdinalIgnoreCase);
}

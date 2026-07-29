namespace Peliculas.Configuracion;

// Representa la sección Tmdb de la configuración.
public class TmdbOpciones
{
    public const string Seccion = "Tmdb";

    // Se utilizará el API Read Access Token como Bearer token.
    public string TokenAcceso { get; set; } = "";

    // TMDB traducirá títulos y descripciones al español.
    public string Idioma { get; set; } = "es-ES";

    // La región española afecta a estrenos y proveedores disponibles.
    public string Region { get; set; } = "ES";

    // Tiempo durante el que se reutilizan las respuestas descargadas.
    public int MinutosCache { get; set; } = 15;
}

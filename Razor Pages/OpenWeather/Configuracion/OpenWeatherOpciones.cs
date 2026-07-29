namespace OpenWeather.Configuracion;

// Representa la sección "OpenWeather" de la configuración.
// Es una clase sencilla para que el acceso a los valores sea claro y tipado.
public class OpenWeatherOpciones
{
    // Nombre de la sección que se buscará en appsettings.json.
    public const string Seccion = "OpenWeather";

    // La clave se proporcionará mediante secretos de usuario o una variable de entorno.
    public string ApiKey { get; set; } = "";

    // OpenWeather traducirá las descripciones meteorológicas al español.
    public string Idioma { get; set; } = "es";

    // Tiempo durante el que se reutiliza una respuesta ya descargada.
    public int MinutosCache { get; set; } = 10;
}

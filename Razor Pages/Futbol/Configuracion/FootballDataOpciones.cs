namespace Futbol.Configuracion;

// Representa la sección FootballData de la configuración.
public class FootballDataOpciones
{
    public const string Seccion = "FootballData";

    public string ApiKey { get; set; } = "";

    // La caché evita superar el límite del plan gratuito.
    public int MinutosCache { get; set; } = 15;
}

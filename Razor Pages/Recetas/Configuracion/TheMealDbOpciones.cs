namespace Recetas.Configuracion;

// Agrupa la configuración relacionada con el servicio externo.
public class TheMealDbOpciones
{
    public const string Seccion = "TheMealDb";

    // La clave 1 está autorizada para desarrollo y educación.
    public string ApiKey { get; set; } = "1";
    public int MinutosCache { get; set; } = 30;

    public string ApiKeyValida =>
        string.IsNullOrWhiteSpace(ApiKey) ? "1" : ApiKey.Trim();
}

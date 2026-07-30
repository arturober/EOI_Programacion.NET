namespace Videojuegos.Configuracion;

// Agrupa la configuración relacionada con el servicio externo.
public class RawgOpciones
{
    public const string Seccion = "Rawg";

    public string ApiKey { get; set; } = "";
    public int MinutosCache { get; set; } = 30;
    public int TamanoPagina { get; set; } = 20;

    public bool TieneApiKey =>
        !string.IsNullOrWhiteSpace(ApiKey)
        && !ApiKey.Contains("PEGA_AQUI", StringComparison.OrdinalIgnoreCase);
}

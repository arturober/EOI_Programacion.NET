namespace OpenFoodFacts.Configuracion;

// Agrupa la configuración utilizada al llamar al servicio externo.
public class OpenFoodFactsOpciones
{
    public const string Seccion = "OpenFoodFacts";

    // Open Food Facts pide identificar la aplicación con un contacto.
    public string Contacto { get; set; } = "contacto@example.com";
    public int MinutosCache { get; set; } = 30;
    public int TamanoPagina { get; set; } = 12;

    public string ContactoValido =>
        string.IsNullOrWhiteSpace(Contacto)
            ? "contacto@example.com"
            : Contacto.Trim();

    public int TamanoPaginaValido =>
        Math.Clamp(TamanoPagina, 4, 24);
}

namespace NasaExplorer.Configuracion;

// Representa la sección "Nasa" de la configuración.
public class NasaOpciones
{
    // La clave se configura con User Secrets y nunca se envía al navegador.
    public string ApiKey { get; set; } = string.Empty;

    // Este valor evita repetir peticiones idénticas durante una clase.
    public int MinutosCache { get; set; } = 10;
}

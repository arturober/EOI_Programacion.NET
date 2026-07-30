namespace NasaExplorer.Servicios;

// Convierte errores técnicos de APIs externas en mensajes entendibles por las páginas.
public class ApiExternaExcepcion(string mensaje, Exception? innerException = null)
    : Exception(mensaje, innerException)
{
}

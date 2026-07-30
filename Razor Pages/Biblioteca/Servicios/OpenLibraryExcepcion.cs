using System.Net;

namespace Biblioteca.Servicios;

// Convierte los fallos externos en mensajes comprensibles para el usuario.
public class OpenLibraryExcepcion : Exception
{
    public HttpStatusCode? CodigoEstado { get; }

    public OpenLibraryExcepcion(
        string mensaje,
        HttpStatusCode? codigoEstado = null)
        : base(mensaje)
    {
        CodigoEstado = codigoEstado;
    }
}

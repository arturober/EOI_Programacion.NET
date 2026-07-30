using System.Net;

namespace Videojuegos.Servicios;

// Convierte los fallos externos en mensajes comprensibles para el usuario.
public class RawgExcepcion : Exception
{
    public HttpStatusCode? CodigoEstado { get; }

    public RawgExcepcion(
        string mensaje,
        HttpStatusCode? codigoEstado = null)
        : base(mensaje)
    {
        CodigoEstado = codigoEstado;
    }
}

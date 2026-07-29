using System.Net;

namespace Peliculas.Servicios;

// Convierte los fallos de la API externa en mensajes comprensibles.
public class TmdbExcepcion : Exception
{
    public HttpStatusCode? CodigoEstado { get; }

    public TmdbExcepcion(
        string mensaje,
        HttpStatusCode? codigoEstado = null)
        : base(mensaje)
    {
        CodigoEstado = codigoEstado;
    }
}

using System.Net;

namespace Recetas.Servicios;

// Convierte los fallos externos en mensajes comprensibles.
public class TheMealDbExcepcion : Exception
{
    public HttpStatusCode? CodigoEstado { get; }

    public TheMealDbExcepcion(
        string mensaje,
        HttpStatusCode? codigoEstado = null)
        : base(mensaje)
    {
        CodigoEstado = codigoEstado;
    }
}

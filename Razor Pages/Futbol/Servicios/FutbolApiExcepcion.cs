using System.Net;

namespace Futbol.Servicios;

// Permite que las páginas muestren fallos de la API de forma comprensible.
public class FutbolApiExcepcion : Exception
{
    public FutbolApiExcepcion(
        string mensaje,
        HttpStatusCode? codigoEstado = null)
        : base(mensaje)
    {
        CodigoEstado = codigoEstado;
    }

    public HttpStatusCode? CodigoEstado { get; }
}

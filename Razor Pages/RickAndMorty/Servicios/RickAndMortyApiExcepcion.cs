using System.Net;

namespace RickAndMorty.Servicios;

// Permite mostrar fallos externos sin revelar excepciones técnicas.
public class RickAndMortyApiExcepcion : Exception
{
    public RickAndMortyApiExcepcion(
        string mensaje,
        HttpStatusCode? codigoEstado = null)
        : base(mensaje)
    {
        CodigoEstado = codigoEstado;
    }

    public HttpStatusCode? CodigoEstado { get; }
}

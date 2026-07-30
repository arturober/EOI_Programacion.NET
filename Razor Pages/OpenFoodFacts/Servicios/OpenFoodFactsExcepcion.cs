using System.Net;

namespace OpenFoodFacts.Servicios;

// Permite mostrar errores comprensibles sin filtrar detalles técnicos.
public class OpenFoodFactsExcepcion : Exception
{
    public OpenFoodFactsExcepcion(
        string mensaje,
        HttpStatusCode? codigoEstado = null)
        : base(mensaje)
    {
        CodigoEstado = codigoEstado;
    }

    public HttpStatusCode? CodigoEstado { get; }
}

using System.Net;

namespace OpenWeather.Servicios;

// Representa un error comprensible producido al comunicarse con OpenWeather.
public class OpenWeatherExcepcion : Exception
{
    public HttpStatusCode? CodigoEstado { get; }

    public OpenWeatherExcepcion(string mensaje, HttpStatusCode? codigoEstado = null)
        : base(mensaje)
    {
        CodigoEstado = codigoEstado;
    }
}

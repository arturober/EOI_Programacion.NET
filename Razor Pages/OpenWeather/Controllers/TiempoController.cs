using Microsoft.AspNetCore.Mvc;
using OpenWeather.Modelos;
using OpenWeather.Servicios;

namespace OpenWeather.Controllers;

[ApiController]
[Route("api")]
public class TiempoController : ControllerBase
{
    private readonly IOpenWeatherServicio _openWeather;

    public TiempoController(IOpenWeatherServicio openWeather)
    {
        _openWeather = openWeather;
    }

    // GET /api/lugares?texto=Alicante
    [HttpGet("lugares")]
    public async Task<ActionResult<IReadOnlyList<Lugar>>> BuscarLugares(
        string texto,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(texto) || texto.Trim().Length < 2)
        {
            return BadRequest(new
            {
                error = "Escribe al menos dos caracteres."
            });
        }

        try
        {
            IReadOnlyList<Lugar> lugares =
                await _openWeather.BuscarLugaresAsync(
                    texto, cancellationToken);

            return Ok(lugares);
        }
        catch (OpenWeatherExcepcion excepcion)
        {
            return StatusCode(CodigoHttp(excepcion), new
            {
                error = excepcion.Message
            });
        }
    }

    // GET /api/tiempo?lat=38.3452&lon=-0.4810&unidades=metrico
    [HttpGet("tiempo")]
    public async Task<ActionResult<InformeMeteorologico>> ObtenerTiempo(
        double lat,
        double lon,
        string? unidades,
        CancellationToken cancellationToken)
    {
        try
        {
            Lugar? lugar = await _openWeather.BuscarLugarPorCoordenadasAsync(
                lat, lon, cancellationToken);

            lugar ??= new Lugar
            {
                Nombre = "Ubicación seleccionada",
                Latitud = lat,
                Longitud = lon
            };

            InformeMeteorologico informe =
                await _openWeather.ObtenerInformeAsync(
                    lugar,
                    UnidadesExtensiones.DesdeTexto(unidades),
                    cancellationToken);

            return Ok(informe);
        }
        catch (OpenWeatherExcepcion excepcion)
        {
            return StatusCode(CodigoHttp(excepcion), new
            {
                error = excepcion.Message
            });
        }
    }

    private static int CodigoHttp(OpenWeatherExcepcion excepcion)
    {
        // Los errores externos se convierten en códigos sencillos de esta API.
        return excepcion.CodigoEstado is null
            ? StatusCodes.Status503ServiceUnavailable
            : (int)excepcion.CodigoEstado.Value;
    }
}

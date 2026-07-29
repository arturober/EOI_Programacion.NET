using MazmorraOnline.Dtos;
using MazmorraOnline.Services;
using Microsoft.AspNetCore.Mvc;

namespace MazmorraOnline.Controllers;

// Proporciona una API sencilla para consultar y modificar el juego.
[ApiController]
[Route("api")]
public class JuegoController : ControllerBase
{
    private readonly GestorJuego _gestorJuego;

    public JuegoController(GestorJuego gestorJuego)
    {
        _gestorJuego = gestorJuego;
    }

    [HttpPost("entrar")]
    public ActionResult<AccesoJuegoRespuesta> Entrar(
        EntrarJuegoPeticion peticion)
    {
        // Los errores de validación del gestor se devuelven como HTTP 400.
        try
        {
            return Ok(_gestorJuego.Entrar(peticion.Nombre));
        }
        catch (InvalidOperationException excepcion)
        {
            return BadRequest(new { mensaje = excepcion.Message });
        }
    }

    [HttpDelete("jugadores/{jugadorId}")]
    public IActionResult Salir(string jugadorId)
    {
        // HTTP 404 indica que el jugador ya no existía.
        if (!_gestorJuego.EliminarJugador(jugadorId))
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpGet("mapas")]
    public ActionResult<List<MapaDto>> ObtenerMapas()
    {
        // Los mapas incluyen sus filas para poder representarlos.
        return Ok(_gestorJuego.ObtenerMapas());
    }

    [HttpGet("clasificacion")]
    public ActionResult<List<ClasificacionJugadorDto>>
        ObtenerClasificacion()
    {
        // La clasificación solo incluye a los jugadores conectados.
        return Ok(_gestorJuego.ObtenerClasificacion());
    }

    [HttpGet("jugadores/{jugadorId}")]
    public ActionResult<JugadorDetalleDto> ObtenerJugador(
        string jugadorId)
    {
        // Se devuelve HTTP 404 si el identificador no existe.
        JugadorDetalleDto? jugador =
            _gestorJuego.ObtenerJugador(jugadorId);

        return jugador is null ? NotFound() : Ok(jugador);
    }

    [HttpGet("resultados")]
    public ActionResult<List<ResultadoRondaDto>> ObtenerResultados()
    {
        // El historial contiene como máximo las últimas diez rondas.
        return Ok(_gestorJuego.ObtenerResultados());
    }
}
